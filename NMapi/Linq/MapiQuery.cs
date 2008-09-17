//
// openmapi.org - NMapi C# Mapi API - MapiQuery.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.Common;
using System.Reflection;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	/// <summary>
	///  The Query provider.
	/// </summary>
	public class MapiQuery<MEntity> : IDisposable, IQueryProvider, IEnumerable<MEntity>,  
			IQueryable<MEntity>, IQueryable, IEnumerable, 
			IOrderedQueryable<MEntity>, IOrderedQueryable,
			IBindingList, ICollection, ITypedList
	{
		private TableQuerier<MEntity> sharedQuerier;
		private QueryState<MEntity> state;
		private MapiContext context;
		private IMapiContainer container;
		private Expression expressionQuery;

		private ListChangedEventHandler onListChanged;
		private ListChangedEventArgs resetEvent = new ListChangedEventArgs (ListChangedType.Reset, -1);

		private void EnsureExecution ()
		{
			if (sharedQuerier == null)
				BaseExecute (this.expressionQuery, null, 0, true);
		}

		#region DataBinding

		/// <summary>
		///
		/// </summary>
		public void CopyTo (Array array, int index)
		{
			EnsureExecution ();
			int k = index;
			foreach (MEntity item in this) {
				array.SetValue (item, k);
				k++;
			}
		}

		/// <summary>
		///
		/// </summary>
		public int Count {
			get {
				EnsureExecution ();
				return sharedQuerier.Count;
			}
		}

		/// <summary>
		///
		/// </summary>
		public bool IsSynchronized {
			get { return false; }
		}

		/// <summary>
		///
		/// </summary>
		public object SyncRoot {
			get { return this; }
		}

		/// <summary>
		///
		/// </summary>
		public bool IsFixedSize {
			get { return false; }
		}

		/// <summary>
		///
		/// </summary>
		public bool IsReadOnly  {
			get { return false; }

		}

		/// <summary>
		///  Returns the object at the specified index.
		/// </summary>
		public object this [int index]  {
			get {
				//
				// This is the most important call for DataBinding!
				// Make sure this performs well!
				//
				EnsureExecution ();
				return sharedQuerier.SeekIndex (index);
			}
			set  {
				EnsureExecution ();
				if (value == null) {
					MEntity entity = (MEntity) value;
					if (entity != null)
						throw new NotImplementedException ("Not yet implemented.");
				}
				throw new ArgumentException ("value must be of type" + typeof(MEntity));
			}
		}

		/// <summary>
		///	 Adds an item to the IList. 
		/// </summary>
		public int Add (object value)
		{
			EnsureExecution ();
			throw new NotImplementedException ("Not yet implemented.");
		}

		/// <summary>
		///  Deletes all objects contained in the query.
		/// </summary>
		public void Clear ()
		{
			EnsureExecution ();
			foreach (MEntity entity in this)
				((IMapiEntity) entity).Delete ();
		}

		/// <summary>
		///  Determines whether the IList contains a specific value. 
		/// </summary>
		public bool Contains (object value)
		{
			Console.WriteLine ("DATABINDING: CONTAINS  called!");
			return ( IndexOf (value) != -1 );
		}

		/// <summary>
		///
		/// </summary>
		public int IndexOf (object value)
		{
			Console.WriteLine ("DATABINDING:INDEX-OF called!");

			if (value == null || value.GetType() != typeof (MEntity))
				return -1;
			EnsureExecution ();
			int i = 0;

			foreach (object current in this) {
				if (current == value)
					return i;
				i++;
			}

			return -1;
		}

		/// <summary>
		/// Inserts an item to the IList at the specified index. 
		/// </summary>
		public void Insert (int index, object value)
		{
			EnsureExecution ();
			throw new NotImplementedException ("Not yet implemented.");
		}

		/// <summary>
		///  DELETE?
		/// </summary>
		public void Remove (object value)
		{
			EnsureExecution ();
			IMapiEntity entity = value as IMapiEntity;
			if (entity != null)
				entity.Delete ();
			throw new ArgumentException ("value must be of type" + typeof(MEntity));
		}

		/// <summary>
		///  DELETE?
		/// </summary>
		public void RemoveAt (int index)
		{
			Remove (this [index]);
		}

		/// <summary>
		///
		/// </summary>
		protected virtual void OnListChanged (ListChangedEventArgs ev) 
		{
			if (onListChanged != null)
				onListChanged (this, ev);
		}

		/// <summary>
		///
		/// </summary>
		protected void OnClear () 
		{
		}

		/// <summary>
		///
		/// </summary>
		protected void OnClearComplete () 
		{
			OnListChanged (resetEvent);
		}

		internal void OnInsertComplete (MEntity element)
		{
			OnInsertComplete (IndexOf (element), element);
		}

		internal void OnRemoveComplete (MEntity element) 
		{
			OnRemoveComplete (IndexOf (element), element);
		}

		/// <summary>
		///
		/// </summary>
		protected void OnInsertComplete (int index, object value)
		{
			OnListChanged (new ListChangedEventArgs (ListChangedType.ItemAdded, index));
		}

		/// <summary>
		///
		/// </summary>
		protected void OnRemoveComplete (int index, object value) 
		{
			OnListChanged (new ListChangedEventArgs (ListChangedType.ItemDeleted, index));
		}

		/// <summary>
		///
		/// </summary>
		protected void OnSetComplete (int index, object oldValue, object newValue) 
		{
			if (oldValue != newValue)
				OnListChanged (new ListChangedEventArgs (ListChangedType.ItemAdded, index));
		}

		internal void MEntityChanged (IMapiEntity entity)
		{
			int index = IndexOf (entity);
			OnListChanged (new ListChangedEventArgs (ListChangedType.ItemChanged, index));
		}


		/// <summary>
		///  Always returns true.
		/// </summary>
		bool IBindingList.AllowEdit 
		{ 
			get { return true; }
		}

		/// <summary>
		///
		/// </summary>
		bool IBindingList.AllowNew 
		{ 
			get { return false; }
		}

		/// <summary>
		///  Always returns true.
		/// </summary>
		bool IBindingList.AllowRemove 
		{ 
			get { return true; }
		}

		/// <summary>
		///  Always returns true.
		/// </summary>
		bool IBindingList.SupportsChangeNotification 
		{ 
			get { return true; }
		}

		/// <summary>
		///  Always returns false.
		/// </summary>
		bool IBindingList.SupportsSearching
		{ 
			get { return false; }
		}

		/// <summary>
		///  Always returns false.
		/// </summary>
		bool IBindingList.SupportsSorting
		{
			get { return false; }
		}

		/// <summary>
		///
		/// </summary>
		public event ListChangedEventHandler ListChanged {
			add { onListChanged += value; }
			remove { onListChanged -= value; }
		}

		/// <summary>
		///  Appends a new (empty) object to the list.
		/// </summary>
		object IBindingList.AddNew ()
		{
		//	Customer c = new Customer (this.Count.ToString());
		//	List.Add (c);
		//	return c;
			throw new NotImplementedException ("TODO!");
		}

		/// <summary>
		///
		/// </summary>
		bool IBindingList.IsSorted
		{ 
			get { throw new NotSupportedException (); }
		}

		/// <summary>
		///
		/// </summary>
		ListSortDirection IBindingList.SortDirection 
		{ 
			get { throw new NotSupportedException (); }
		}

		/// <summary>
		///
		/// </summary>
		PropertyDescriptor IBindingList.SortProperty 
		{ 
			get { throw new NotSupportedException (); }
		}


		/// <summary>
		///
		/// </summary>
		void IBindingList.AddIndex (PropertyDescriptor property) 
		{
			throw new NotSupportedException ();
		}

		/// <summary>
		///
		/// </summary>
		void IBindingList.ApplySort (PropertyDescriptor property, 
			ListSortDirection direction)
		{
			throw new NotSupportedException (); 
		}

		/// <summary>
		///
		/// </summary>
		int IBindingList.Find (PropertyDescriptor property, object key)
		{
			throw new NotSupportedException (); 
		}

		/// <summary>
		///
		/// </summary>
		void IBindingList.RemoveIndex (PropertyDescriptor property)
		{
			throw new NotSupportedException (); 
		}

		/// <summary>
		///
		/// </summary>
		void IBindingList.RemoveSort ()
		{
			throw new NotSupportedException (); 
		}

		/// <summary>
		///
		/// </summary>
		protected virtual bool DataBindProperty (PropertyDescriptor prop)
		{
			foreach (Attribute attr in prop.Attributes) {
				var mpa = attr as MapiPropertyAttribute;
				if (mpa != null && mpa.AllowDataBinding)
					return true;
			}
			return false;			
		}

		/// <summary>
		///
		/// </summary>
		public PropertyDescriptorCollection GetItemProperties (PropertyDescriptor[] listAccessors)
		{
			var pdc =  new PropertyDescriptorCollection (new PropertyDescriptor [] {});
			var props = TypeDescriptor.GetProperties (typeof (MEntity));
			foreach (PropertyDescriptor prop in props) {
				if (DataBindProperty (prop))
					pdc.Add (prop);
			}
			return pdc;
		}

		/// <summary>
		///
		/// </summary>
		public string GetListName (PropertyDescriptor[] listAccessors)
		{
			return null; // TODO (only used by obsolete DataGrid)
		}

		#endregion DataBinding


		Expression IQueryable.Expression {
			get { return this.expressionQuery; }
		}

		Type IQueryable.ElementType {
			get { return typeof (MEntity); }
		}

		IQueryProvider IQueryable.Provider {
			get { return this; }
		}

		public IEnumerator<MEntity> GetEnumerator ()
		{
			Console.WriteLine ("enumerator called!");
			return (IEnumerator<MEntity>) Execute (this.expressionQuery, null, 0, true);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			Console.WriteLine ("enumerator called!");
			return (IEnumerator<MEntity>) Execute (this.expressionQuery, null, 0, true);
		}

		T IQueryProvider.Execute<T> (Expression expression)
		{
			return (T) this.Execute (expression, null, 0, false);
		}

		object IQueryProvider.Execute (Expression expression)
		{
			return Execute (expression, null, 0, true);
		}

		internal IEnumerable<MEntity> Proxy_SharedRangeFromBookmark (TableBookmark bookmark, int length)
		{
			return new TablePartView (this, bookmark, length, true);
		}

		internal IEnumerable<MEntity> Proxy_PrivateRangeFromBookmark (TableBookmark bookmark, int length)
		{
			return new TablePartView (this, bookmark, length, false);
		}

		internal TableBookmark Proxy_GetBookmarkBeginning ()
		{
			// TODO
			return null;
		}

		internal TableBookmark Proxy_GetBookmarkCurrent ()
		{
			// TODO
			return null;
		}

		internal TableBookmark Proxy_GetBookmarkEnd ()
		{
			// TODO
			return null;
		}

		internal TableBookmark Proxy_CreateBookmark ()
		{
			// TODO
			return null;
		}

		internal int Proxy_MoveCursor (int rows)
		{
			int moved = 0;
			Console.WriteLine ("MOVE CURSOR!"); // TODO
			return moved;
		}

		internal void Proxy_MoveCursorApprox (int n, int d)
		{
			Console.WriteLine ("MOVE CURSOR APPROX!"); // TODO
		}

		IQueryable<T> IQueryProvider.CreateQuery<T> (Expression expression)
		{
			Type t = typeof (T);

			Type[] interfaces = t.FindInterfaces ( (type, filterCriteria) => {
					if (type == typeof (IMapiEntity))
						return true;
					return false;
				}, null);

			if (interfaces.Length == 0)
				throw new ArgumentException ("T must implement IMapiEntity.");

			var defaultConstructor = t.GetConstructor (new Type [] {});
			if (defaultConstructor == null)
				throw new ArgumentException ("T must provide a default constructor.");
			if (!t.IsClass)
				throw new ArgumentException ("T must be a class.");

			return new MapiQuery<T> (context, container, expression);
		}

		IQueryable IQueryProvider.CreateQuery (Expression expression)
		{
			Type type = TypeSystem.GetElementType (expression.Type);
			try {
				var genericType = typeof (MapiQuery<>).MakeGenericType (type);
				return (IQueryable) Activator.CreateInstance (
					genericType, new object[] { this, expression }
				);
			} catch (TargetInvocationException e) {
				throw e.InnerException;
			}
		}

		public MapiQuery (MapiContext context, IMapiContainer container, 
			Expression expression) : this (context, container)
		{
			if (expression == null) 
				throw new ArgumentNullException ("expression");
			if (!typeof(IQueryable<MEntity>).IsAssignableFrom (expression.Type))
				throw new ArgumentOutOfRangeException ("expression");
			this.expressionQuery = expression;
		}

		public MapiQuery (MapiContext context, IMapiContainer container)			
		{
			this.context = context;
			this.container = container;
			this.expressionQuery = Expression.Constant (this);
		}

		public void BaseExecute (Expression expression, TableBookmark bookmark, 
			int length, bool isCollection)
		{
			Console.WriteLine ("base-execute called!");
			// We only execute the Interpreter at the time of the first foreach
			// Afterwards the state and as much data as possible is cached.
			if (state == null) {
				Interpreter<MEntity> interpreter = new Interpreter<MEntity> ();
				expression = new BranchExecutor ().Run (expression);
				interpreter.DoVisit (expression);
				state = interpreter.QueryState;
			}

			sharedQuerier = new TableQuerier<MEntity> (this, state, context, container);

			if (bookmark != null) {
				sharedQuerier.CurrentBookmark = bookmark;
				sharedQuerier.Length = length;
			} else {
				// TODO: reset? => Beginning ...
			}

			// state.Print ();
		}

		public object Execute (Expression expression, TableBookmark bookmark, 
			int length, bool isCollection)
		{
			BaseExecute (expression, bookmark, length, isCollection);

			if (!isCollection) {
				if (state.ScalarOperation != ScalarOperation.None) {
					switch (state.ScalarOperation) {
						case ScalarOperation.Count:
							return sharedQuerier.Count;
						case ScalarOperation.LongCount:
							return (long) sharedQuerier.Count;
						case ScalarOperation.Sum:
							return sharedQuerier.Sum;
						case ScalarOperation.Avg:
							return sharedQuerier.Average;
						case ScalarOperation.First:
							return sharedQuerier.First;
						case ScalarOperation.Last:
							return sharedQuerier.Last;
						case ScalarOperation.FirstOrDefault:
							return sharedQuerier.FirstOrDefault;
						case ScalarOperation.LastOrDefault:
							return sharedQuerier.LastOrDefault;
						case ScalarOperation.Min:
						case ScalarOperation.Max:
							return sharedQuerier.FirstScalarQueriedPropertyValue;
						default:
							sharedQuerier.MoveNext ();
							return sharedQuerier.Current;
					}
				}
				throw new Exception ("A single element was requested, " + 
					"but no supported call ( like First() ) " + 
					"was made to select one.");
			}
			else
				return new EntityEnumerator (sharedQuerier);

		}

		public void Dispose ()
		{

		}

		class TablePartView : IEnumerable<MEntity>, IEnumerable
		{
			private MapiQuery<MEntity> query;
			private TableQuerier<MEntity> privateQuerier;
			private bool useSharedQuerier;
			private TableBookmark bookmark;
			private int length;

			public TablePartView (MapiQuery<MEntity> query, TableBookmark bookmark, 
				int length, bool useSharedQuerier)
			{
				this.query = query;
				this.bookmark = bookmark;
				this.length = length;
				this.useSharedQuerier = useSharedQuerier;
			}

			public IEnumerator<MEntity> GetEnumerator ()
			{
				Console.WriteLine ("range enumerator called!");

				// 1. pass information to table querier (bookmark, length)
				// 2. adapt table querier

				// 3. return TablePartView for normal query as well ... 

				return (IEnumerator<MEntity>) query.Execute (query.expressionQuery, 
								bookmark, length, true);
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				Console.WriteLine ("range enumerator called!");
				return (IEnumerator<MEntity>) query.Execute (query.expressionQuery, 
							bookmark, length, true);
			}
		}

		class EntityEnumerator : IEnumerator<MEntity>, IEnumerator, IDisposable
		{
			private TableQuerier<MEntity> querier;

			internal EntityEnumerator (TableQuerier<MEntity> querier)
			{
				this.querier = querier;
			}

			public MEntity Current {
				get { return querier.Current; }
			}

			object IEnumerator.Current {
				get { return querier.Current; }
			}

			public bool MoveNext ()
			{
				return querier.MoveNext ();
			}

			public void Reset ()
			{
				querier.Reset ();
			}

			public void Dispose ()
			{
				// querier.Dispose ();
			}
		}
	}

}

