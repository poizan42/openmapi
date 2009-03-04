//
// openmapi.org - NMapi C# Mapi API - TableQuerier.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.Common;
using System.Reflection;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	internal class TableQuerier<MEntity> : IDisposable
	{
		private QueryState<MEntity> state;
		private MapiQuery<MEntity> query;
		private MapiContext context;
		private IMapiTable table;
		private IMapiContainer container;
		private TableBookmark bookmark;
		private int length;

		private const int MAX_BUFFER_SIZE = 20;

		private int bufferPosition, bufferSize;
		private MEntity[] buffer;
		private List<int> tags;
		private List<PropertyType> types;
		private List<PropertyInfo> propList;
		private MEntity current;
		private SRowSet rows;

		private Dictionary<string, int> bookmarks;
		private List<SBinary> entryIDList;
		private bool initialLoopComplete = false;

		internal TableQuerier (MapiQuery<MEntity> query, QueryState<MEntity> state, 
			MapiContext context, IMapiContainer container)
		{
			this.query = query;
			this.state = state;
			this.context = context;
			this.table = container.GetContentsTable (Mapi.Unicode);
			this.container = container;
			this.bookmarks = new Dictionary<string, int> ();
			this.entryIDList = new List<SBinary> ();

			this.bufferPosition = 0;
			this.bufferSize = 0;
			this.tags = new List<int> ();
			this.propList = new List<PropertyInfo> ();
			this.types = new List<PropertyType> ();

			PrepareProperties ();

			// register event handler!
			table.Events.TableModified += TableModifiedHandler;

			SetSelectedColumns ();

			table.Restrict (state.MergeRestrictions (), 0);

			SortOrderSet sOrderSet = new SortOrderSet ();
			sOrderSet.ASort = state.OrderByList.ToArray ();
			table.SortTable (sOrderSet, NMAPI.TBL_BATCH);

			bufferSize = Math.Min (state.Amount, MAX_BUFFER_SIZE);
		}

		private MEntity BuildEntity (SRow row)
		{
			Type type = typeof (MEntity);
			MEntity entity = (MEntity) Activator.CreateInstance (type);

			object result;
			for (int i = 0; i < tags.Count; i++) {
				result = row.Props [i].GetValueObj ();
				propList [i].SetValue (entity, result, null);
			}
			((IMapiEntity) entity).MarkAsUnchanged ();
			((IMapiEntity) entity).Context = context;
			((IMapiEntity) entity).InternalContainer = container;

			if (context.Contains (((IMapiEntity) entity).EntryId)) {
				// Console.WriteLine ("cache hit!");
				entity = (MEntity) context.GetBuffered (((IMapiEntity) entity).EntryId);
			}
			else
				context.Register (((IMapiEntity) entity));

			((IMapiEntity) entity).PropertyChanged += (sender, ea) => {
				query.MEntityChanged ((IMapiEntity) sender);
			};
			return entity;			
		}


		private void PrepareProperties ()
		{
			Type type = typeof (MEntity);

			PropertyInfo [] properties = type.GetProperties ();
			foreach (PropertyInfo pinfo in properties) {
				object [] attribs = pinfo.GetCustomAttributes (
						typeof (MapiPropertyAttribute), true);
				if (attribs.Length > 0) {
					var prop = attribs [0] as MapiPropertyAttribute;
					if (prop.LoadMode == LoadMode.PreFetch) {
						propList.Add (pinfo);
						tags.Add (prop.PropertyOrKind);
						types.Add (prop.Type);
					}
				}
			}		
		}

		private void SetSelectedColumns ()
		{
			table.SetColumns (new SPropTagArray (tags.ToArray ()), 0);			
		}

		//  The total number of items in the table.

		int? countCache = null;
		public int Count {
			get {
				if (countCache == null)
					countCache = table.GetRowCount (0);
				return (int) countCache;
			}
		}

		#region Only valid for scalar queries

		// The first item in the underlying table. 
		// "state.ReadBackward" is ignored.
		private MEntity PhysicalFirst {
			get {
				MEntity result;
				try {
					// Ensure correct start.
					table.SeekRow  (Bookmark.Beginning, 0);
					SRowSet rows = table.QueryRows (1, 0);
					if (rows.Count == 0)
						throw new MapiException ("No match!", Error.NotFound);
					result = BuildEntity (rows [0]);					
				} finally {
					Reset ();
				}
				return result;
			}
		}

		// The last item in the underlying table. 
		// "state.ReadBackward" is ignored.
		private MEntity PhysicalLast {
			get {
				MEntity result;
				try {
					// Ensure correct start.
					table.SeekRow  (Bookmark.End, 0);
					SRowSet rows = table.QueryRows (1, 0);
					if (rows.Count == 0)
						throw new MapiException ("No match!", Error.NotFound);
					result = BuildEntity (rows [0]);
				} finally {
					Reset ();
				}
				return result;
			}
		}

		// The first item that appears when enumerating over the query.
		// Throws Exception if table empty.
		internal MEntity First {
			get {
				if (state.ReadBackwards)
					return PhysicalLast;
				return PhysicalFirst;
			}
		}

		// The last item that appears when enumerating over the query.
		// Throws Exception if table empty.
		internal MEntity Last {
			get {
				if (state.ReadBackwards)
					return PhysicalFirst;
				return PhysicalLast;
			}
		}

		// The first item that appears when enumerating over the query.
		// Default if not found.
		internal MEntity FirstOrDefault {
			get {
				try {
					return First;
				} catch (MapiException e) {
					if (e.HResult == Error.NotFound)
						return default (MEntity);
					throw;
				}
			}
		}

		// The last item that appears when enumerating over the query.
		// Default if not found.
		internal MEntity LastOrDefault {
			get {
				try {
					return Last;
				} catch (MapiException e) {
					if (e.HResult == Error.NotFound)
						return default (MEntity);
					throw;
				}
			}
		}

		// IMPORTANT: IGNORES REVERSE!
		internal object FirstScalarQueriedPropertyValue {
			get {
				if (state.ScalarQueriedProperty == -1)
					throw new Exception ("No operand (property) " 
						+ "set for scalar operation!");
				object result;
				try {
					table.SetColumns (new SPropTagArray (state.ScalarQueriedProperty), 0);
					table.SeekRow  (Bookmark.Beginning, 0); // Ensure correct start.
					rows = table.QueryRows  (1, 0);
					if (rows.Count == 0)
						throw new MapiException ("No match!", Error.NotFound);
					PropertyType propType = PropertyTypeHelper.PROP_TYPE (state.ScalarQueriedProperty);
					result = rows [0].Props [0].GetValueObj ();

				} finally {
					SetSelectedColumns (); // reset
					Reset ();
				}
				return result;
			}
		}

		// Reverse doesn't matter
		private object SumCount (out int columnCount)
		{
			if (state.ScalarQueriedProperty == -1)
				throw new Exception ("No operand (property) " 
					+ "set for scalar operation!");
			columnCount = 0;

			object result = null;
			try {
				table.SetColumns (new SPropTagArray (state.ScalarQueriedProperty), 0);
				// Ensure correct start.
				table.SeekRow  (Bookmark.Beginning, 0);
				while (true) {
					rows = table.QueryRows (MAX_BUFFER_SIZE, 0);
					if (rows.Count == 0)
						break;
					foreach (SRow row in rows) {
						columnCount++;
						
						SPropValue prop = row.Props [0];
						
						if (result == null) {
							if (prop is IntProperty)
								result = new Int32 ();
							else if (prop is FloatProperty)
								result = new FloatProperty ();
							else if (prop is DoubleProperty)
								result = new DoubleProperty ();
							else if (prop is ShortProperty)
								result = new ShortProperty ();
						}
						
						if (prop is IntProperty)
							result = ((int) result) + ((IntProperty) row.Props [0]).Value;
						else if (prop is IntProperty)
							result = ((float) result) + ((FloatProperty) row.Props [0]).Value;
						else if (prop is IntProperty)
							result = ((double) result) + ((DoubleProperty) row.Props [0]).Value;
						else if (prop is IntProperty)
							result = ((short) result) + ((ShortProperty) row.Props [0]).Value;
						else
							throw new ArgumentException ("only numerical types can be aggregated.");
					}
				}
			} finally {
				SetSelectedColumns (); // reset
				Reset ();
			}
			return result;
		}

		//  Calculates  the sum of all the values a numeric property of all items 
		//  in the table. The order of the items doesn't matter, for this operation, 
		//  so "state.ReadBackwards" is ignored.
		internal object Sum {
			get {
				int ignore;
				return SumCount (out ignore);
			}
		}
		
		//  Calculates avg() for all the values a numeric property of all items 
		//  in the table. The order of the items doesn't matter, for this operation, 
		//  so "state.ReadBackwards" is ignored.
		internal object Average {
			get {
				int total;
				
				//return SumCount (out total) / total;
				throw new Exception ("UNCOMMENT LINE ABOVE AND FIX!"); // TODO
				
			}
		}

		#endregion Only valid for scalar queries


		#region experimental event handler

		private void TableModifiedHandler (object sender, TableEventArgs ea)
		{
			// Always reset the countCache, just in case ...
			countCache = null;

			TableNotification n = ea.Notification;
			switch (n.TableEvent) {
				case TableNotificationType.Changed:
					throw new NotSupportedException (
						"TableNotificationType.Changed " + 
						"is not yet supported!");
				case TableNotificationType.Error:
					PrepareReload (n.HResult);
				break;
				case TableNotificationType.Reload:
					PrepareReload (null);
				break;
				case TableNotificationType.RowAdded:
					RowAdded (n.PropIndex, n.PropPrior, n.Row);
				break;
				case TableNotificationType.RowDeleted:
					RowDeleted (n.PropIndex, n.Row);
				break;
				case TableNotificationType.RowModified:
					// Ignore; Changes to objects are handled by MapiContext.
				break;
				case TableNotificationType.RestrictDone:
				case TableNotificationType.SetColDone:
				case TableNotificationType.SortDone:
					// ignore
				break;
			}
		}

		private object mergeChangesLock = new Object ();
		private List<SBinary> entryCache = new List<SBinary> ();

		// index = PR_INSTANCE_KEY !

		public void PrepareReload (int? hresult)
		{
			// Something bad happened and a complete reload is required.
			throw new NotImplementedException ("Not implemented, yet.");
		}

		public void RowAdded (SPropValue index, SPropValue prevIndex, SRow props)
		{
			InvalidateIndexCache ();
			lock (entryCache) {
				Console.WriteLine ("Item added to table!");
				// 1. Create a new object and save in context
				// 2. Add EntryID to cache (after  prevIndex). --> entryIDList

				// 3. call OnRemoveComplete on query... 
			}

			var entity = BuildEntity (props);
			query.OnInsertComplete (entity);

		}

		public void RowDeleted (SPropValue index, SRow props)
		{
			InvalidateIndexCache ();
			lock (entryCache) {
				// 1. Notify context. => Reference counting?
				// 2. Check if not already deleted.
				// 3. Update local EntryID cache -> entryIDList

				// 3. call OnInsertComplete on query... 

				Console.WriteLine ("Item deleted from table!");
			}

			var entity = BuildEntity (props);
			query.OnRemoveComplete (entity);
		}

		#endregion

		internal int Length {
			get { return length; }
			set { length = value; }
		}

		internal TableBookmark CurrentBookmark {
			get {
				return bookmark;
			}
			set {
				table.SeekRow (value.Id, 0);
				bookmark = value;
			}
		}

		public MEntity Current {
			get { 
				return this.current;
			}
		}

		private int ReverseIndexIfRequired (int index)
		{
			int position = index;
			if (state.ReadBackwards)
				position = Count - position;
			return position;
		}


		// TODO: locking!

		private Dictionary<int, MEntity> indexCache = new Dictionary<int, MEntity> ();
		private Dictionary<MEntity, int> reverseIndexCache = new Dictionary<MEntity, int> ();

		public int FindIndex (MEntity entity)
		{
			if (reverseIndexCache.ContainsKey (entity))
				return reverseIndexCache [entity];

			var restriction = new PropertyRestriction ();
			
			var bprop = new BinaryProperty ();
			bprop.PropTag = Property.EntryId;
			bprop.Value = ((IMapiEntity) entity).EntryId;
			restriction.Prop = bprop;
			
			restriction.PropTag = Property.EntryId;
			restriction.RelOp = RelOp.Eq;

			table.FindRow (restriction, Bookmark.Beginning, 0);
			int index = ReverseIndexIfRequired (table.QueryPosition ().Row); // TODO: Only TXC, probably ...
			reverseIndexCache [entity] = index;
			return index;
		}


		public void InvalidateIndexCache ()
		{
			indexCache.Clear ();
			reverseIndexCache.Clear ();
		}

		public MEntity SeekIndex (int index)
		{
			index = ReverseIndexIfRequired (index);

			if (indexCache.ContainsKey (index))
				return indexCache [index];

			// FIXME: This works with TeamXChange, but might 
			//        return an incorrect result with other servers.
			table.SeekRowApprox (index, Count);

			int flags = 0;
			var rows = table.QueryRows (5, flags);
			MEntity first = default (MEntity);
			bool foundFirst = false;
			if (rows.Count > 0) {
				int index2 = index;
				foreach (SRow row in rows) {
					var entity = BuildEntity (row);
					indexCache [index2] = entity;
					reverseIndexCache [entity] = index2;
					if (!foundFirst) {
						first = entity;
						foundFirst = true;
					}
					index2++;
				}
			}
			return first;
		}

		private int enumeratorCurrentBkmark;

		// Respects reverse.
		public bool MoveNext ()
		{
			if (initialLoopComplete) {
				// if at start, add new items or remove old ones!

				// return from buffer...
			}

			lock (entryCache) {
				// On second run:
				//  - access entry-cache
				//  - select elements from context!
			}

			if (buffer == null || bufferPosition == bufferSize) {
				if (buffer == null) {
					enumeratorCurrentBkmark = Bookmark.Beginning;
					if (state.ReadBackwards)
						enumeratorCurrentBkmark = Bookmark.End;
				}
				else
					enumeratorCurrentBkmark = Bookmark.Current;
				bufferPosition = 0;
				int flags = 0;
				int maxRead = bufferSize;
				if (state.ReadBackwards) {
					flags = flags | NMAPI.TBL_NOADVANCE;
					int moved  = -1 * table.SeekRow (enumeratorCurrentBkmark, -bufferSize);
					maxRead = moved;
					if (moved <= 0) {
						initialLoopComplete = true;
						Reset ();
						return false;
					}
				} else
					table.SeekRow (enumeratorCurrentBkmark, 0);

				rows = table.QueryRows (maxRead, flags);
				buffer = new MEntity [bufferSize];
				if (rows.Count == 0) {
					initialLoopComplete = true;
					Reset ();
					return false;
				}

				int bindex = 0;
				if (state.ReadBackwards) { // Count from back ...
					int lastIndex = bufferSize-1;
					int emptyAtEnd = bufferSize-rows.Count;
					bindex = lastIndex - emptyAtEnd;
				}
				foreach (SRow row in rows) {
					MEntity entity = BuildEntity (row);
					buffer [bindex] = entity;
//					entryIDList.Add (entity.EntryId);
					if (state.ReadBackwards)
						bindex--;
					else
						bindex++;
				}
			}
			if (buffer [bufferPosition] == null) {
				Reset ();
				return false;
			}
			this.current = buffer [bufferPosition];
			bufferPosition++;
			return true;
		}

		public void Reset ()
		{
			bufferPosition = 0;
			buffer = null;
			table.SeekRow  (Bookmark.Beginning, 0);
		}

		public void Dispose ()
		{
			foreach (int bm in bookmarks.Values)
				table.FreeBookmark (bm);
			table.FreeBookmark (enumeratorCurrentBkmark);
		}
	}

}

