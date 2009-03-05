//
// openmapi.org - NMapi C# Mapi API - MapiContext.cs
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
using NMapi.Events;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	/// <summary>
	///   Unit-Of-work-style class that processes queries and keeps track 
	///   of the returned entities. Similiar to Linq-to-SQL's DataContext.
	/// </summary>	
	public class MapiContext : IDisposable
	{
		private IMapiSession session;
		private Dictionary<SBinary, IMapiEntity> entryIDBuffer;
		private bool objectTrackingEnabled;

		/// <summary>
		///  
		/// </summary>
		public MapiContext (IMapiSession session)
		{
			this.session = session;
			entryIDBuffer = new  Dictionary<SBinary, IMapiEntity> ();
			objectTrackingEnabled = false;
		}

		/// <summary>
		///  Gives access to the Contents-Table of the specified container.
		///  Mapi.Unicode is passed to the underlying MapiTable, so 
		///  Columns containing string data will be returned as unicode.
		/// </summary>
		public MapiQuery<T> GetQuery<T> (IMapiContainer container)
			where T: class,IMapiEntity, new ()
		{
			if (container == null)
				throw new ArgumentNullException (
					"Can't open a table on a null-object.");
			EnforceSameSession (container);
			var table = container.GetContentsTable (Mapi.Unicode);

			EnforceSameSession (table);
			return new MapiQuery<T> (this, container);
		}

		private void EnforceSameSession (IBase obj)
		{
		}

		/// <summary>
		///  
		/// </summary>
		public IMapiEntity Create<T> () where T: IMapiEntity, new ()
		{
			throw new NotImplementedException ("Not yet implemented.");

			//IMapiEntity entity = new T ();
			//Register (entity);
			//return entity;
		}

		/// <summary>
		///  
		/// </summary>
		public void Delete (IMapiEntity entity)
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		/// <summary>
		///  
		/// </summary>
		internal bool Contains (SBinary eid)
		{
			return entryIDBuffer.ContainsKey (eid);
		}

		/// <summary>
		///  
		/// </summary>
		internal IMapiEntity GetBuffered (SBinary eid)
		{
			return entryIDBuffer [eid];
		}

		/// <summary>
		///  
		/// </summary>
		internal void Unregister (IMapiEntity entity)
		{
			UnsubscribeEventHandlers (entity);
			entryIDBuffer.Remove (entity.EntryId);
		}

		/// <summary>
		///  
		/// </summary>
		internal void Register (IMapiEntity entity)
		{
			if (entryIDBuffer.ContainsKey (entity.EntryId))
				return;

			// FIXME
			//  - More selective subscription
			//  - Support public store
			Subscribe (session.PrivateStore, entity);
			entryIDBuffer [entity.EntryId] = entity;
		}

		private void Subscribe (IMsgStore store, IMapiEntity entity)
		{
			store.Events [entity.EntryId].CriticalError += CriticalErrorHandler;
			store.Events [entity.EntryId].Extended += ExtendedHandler;
			store.Events [entity.EntryId].NewMail += NewMailHandler;
			store.Events [entity.EntryId].ObjectCopied += ObjectCopiedHandler;
			store.Events [entity.EntryId].ObjectCreated += ObjectCreatedHandler;
			store.Events [entity.EntryId].ObjectDeleted += ObjectDeletedHandler;
			store.Events [entity.EntryId].ObjectModified += ObjectModifiedHandler;
			store.Events [entity.EntryId].ObjectMoved += ObjectMovedHandler;
			store.Events [entity.EntryId].SearchComplete += SearchCompleteHandler;
			store.Events [entity.EntryId].TableModified += TableModifiedHandler;	
		}


		private void CriticalErrorHandler (object sender, ErrorEventArgs ea)
		{
		}

		private void ExtendedHandler (object sender, ExtendedEventArgs ea)
		{
		}


		private void NewMailHandler (object sender, NewMailEventArgs ea)
		{
		}


		private void ObjectCopiedHandler (object sender, ObjectEventArgs ea)
		{
		}


		private void ObjectCreatedHandler (object sender, ObjectEventArgs ea)
		{
		}


		private void ObjectDeletedHandler (object sender, ObjectEventArgs ea)
		{
		}

		private void ObjectModifiedHandler (object sender, ObjectEventArgs ea)
		{
			SBinary entryId = new SBinary (ea.Notification.EntryID);
			if (entryId == null)
				return;
			if (!entryIDBuffer.ContainsKey (entryId))
				return;
			IMapiEntity entity = entryIDBuffer [entryId];

			PropertyTag[] changedProps = ea.Notification.PropTagArray;
			bool localChangesMade = entity.Update (changedProps);

			MapiEntityEventArgs args = new MapiEntityEventArgs (entity, 
				ea.Notification, localChangesMade);
			entity.OnModified (args);
		}

		private void ObjectMovedHandler (object sender, ObjectEventArgs ea)
		{
		}


		private void SearchCompleteHandler (object sender, ObjectEventArgs ea)
		{
		}

		private void TableModifiedHandler (object sender, TableEventArgs ea)
		{
		}


		/// <summary>
		///  
		/// </summary>
		public void SubmitChanges ()
		{
			foreach (var pair in entryIDBuffer)
				if (pair.Value != null)
					((IMapiEntity) pair.Value).Save ();
		}
	
		/// <summary>
		///  
		/// </summary>
		public bool ObjectTrackingEnabled
		{
			get { return objectTrackingEnabled; }
			set {
				throw new NotImplementedException ("Not yet implemented!");
			}
		}

		/// <summary>
		///  
		/// </summary>
		public void Refresh ()
		{
			throw new NotImplementedException ("Not yet implemented!");
		}

		private void UnsubscribeEventHandlers (IMapiEntity entity)
		{
			// FIXME: Support public store

			var store = session.PrivateStore;

			store.Events [entity.EntryId].CriticalError -= CriticalErrorHandler;
			store.Events [entity.EntryId].Extended -= ExtendedHandler;
			store.Events [entity.EntryId].NewMail -= NewMailHandler;
			store.Events [entity.EntryId].ObjectCopied -= ObjectCopiedHandler;
			store.Events [entity.EntryId].ObjectCreated -= ObjectCreatedHandler;
			store.Events [entity.EntryId].ObjectDeleted -= ObjectDeletedHandler;
			store.Events [entity.EntryId].ObjectModified -= ObjectModifiedHandler;
			store.Events [entity.EntryId].ObjectMoved -= ObjectMovedHandler;
			store.Events [entity.EntryId].SearchComplete -= SearchCompleteHandler;
			store.Events [entity.EntryId].TableModified -= TableModifiedHandler;
		}

		public void Dispose ()
		{
			foreach (IMapiEntity entity in entryIDBuffer.Values)
				UnsubscribeEventHandlers (entity);
		}
	
	
	}
}
