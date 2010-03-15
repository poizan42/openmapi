//
// openmapi.org - NMapi C# Mapi API - ObjectEventSet.cs
//
// Copyright 2008-2009 Topalis AG
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

namespace NMapi.Events {


	// TODO: we need to ensure that event notifications are always properly unregistered!


	using System;
	using System.Collections.Generic;
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop;

	using NMapi.Flags;
	using NMapi.Table;

	/// <summary>Base class for Notification-EventArgs.</summary>
	/// <remarks></remarks>
	public class MapiNotificationEventArgs : EventArgs
	{
		private NotificationEventType eventType;

		public NotificationEventType EventType {
			get { return eventType; }
			set { eventType = value; }
		}
		
		public MapiNotificationEventArgs (NotificationEventType eventType)
		{
			this.eventType = eventType;
		}

	}

	/// <summary>
	///  EventArg-Wrapper for NewMailNotification.
	/// </summary>
	public class NewMailEventArgs : MapiNotificationEventArgs
	{
		private NewMailNotification noti;

		public NewMailNotification Notification {
			get { return noti; }
			set { noti = value; }
		}

		public NewMailEventArgs (NewMailNotification noti)
			: base (NotificationEventType.NewMail)
		{
			this.noti = noti;
		}
	}

	/// <summary>
	///  EventArg-Wrapper for ErrorNotification.
	/// </summary>
	public class ErrorEventArgs : MapiNotificationEventArgs
	{
		private ErrorNotification noti;

		public ErrorNotification Notification {
			get { return noti; }
			set { noti = value; }
		}

		public ErrorEventArgs (ErrorNotification noti)
			: base (NotificationEventType.CriticalError)
		{
			this.noti = noti;
		}
	}

	/// <summary>
	///  EventArg-Wrapper for NewMailNotification.
	/// </summary>
	public class TableEventArgs : MapiNotificationEventArgs
	{
		private TableNotification noti;

		public TableNotification Notification {
			get { return noti; }
			set { noti = value; }
		}

		public TableEventArgs (TableNotification noti)
			: base (NotificationEventType.TableModified)
		{
			this.noti = noti;
		}
	}

	/// <summary>
	///  EventArg-Wrapper for ObjectNotification.
	/// </summary>
	public class ObjectEventArgs : MapiNotificationEventArgs
	{
		private ObjectNotification noti;

		public ObjectNotification Notification {
			get { return noti; }
			set { noti = value; }
		}

		public ObjectEventArgs (ObjectNotification noti, 
			NotificationEventType eventType) : base (eventType)
		{
			this.noti = noti;
		}
	}

	/// <summary>
	///  EventArg-Wrapper for StatusObjectNotification.
	/// </summary>
	public class StatusObjectEventArgs : MapiNotificationEventArgs
	{
		private StatusObjectNotification noti;

		public StatusObjectNotification Notification {
			get { return noti; }
			set { noti = value; }
		}

		public StatusObjectEventArgs (StatusObjectNotification noti)
			: base (NotificationEventType.StatusObjectModified) 
		{
			this.noti = noti;
		}
	}

	/// <summary>
	///  EventArg-Wrapper for ExtendedNotification.
	/// </summary>
	public class ExtendedEventArgs : MapiNotificationEventArgs
	{
		private ExtendedNotification noti;

		public ExtendedNotification Notification {
			get { return noti; }
			set { noti = value; }
		}

		public ExtendedEventArgs (ExtendedNotification noti) 
			: base (NotificationEventType.Extended) {
			this.noti = noti;
		}
	}


	/// <summary>
	///  Provides an interface for events as delegates that may be used 
	///  alternatively to the classic AdviseSink-method. They are actived 
	///  indirectly through a special AdviseSink-Object.
	/// </summary>
	public class ObjectEventSet
	{
		private SBinary entryID;
		private IAdvisor advisor;
		private MapiPseudoAdviseSink adviseSink;
		private EventConnection connection;

		public ObjectEventSet (IAdvisor advisor, SBinary entryID)
		{
			this.advisor = advisor;
			this.entryID = entryID;
		}


		private void CheckRegisterAdviseSink ()
		{
			if (adviseSink == null) {
				// TODO: This should be improved for better performance!
				NotificationEventType eventMask = NotificationEventType.CriticalError |
						NotificationEventType.NewMail |
						NotificationEventType.ObjectCreated |
						NotificationEventType.ObjectDeleted |
						NotificationEventType.ObjectModified |
						NotificationEventType.ObjectMoved |
						NotificationEventType.ObjectCopied |
						NotificationEventType.SearchComplete |
						NotificationEventType.TableModified |
						NotificationEventType.StatusObjectModified |
						NotificationEventType.ReservedForMapi |
						NotificationEventType.Extended;
				adviseSink = new MapiPseudoAdviseSink (this);
				byte[] eid = null;
				if (entryID != null && entryID.ByteArray != null)
					eid = entryID.ByteArray;
				connection = advisor.Advise (eid, eventMask, adviseSink);
			}
		}

		private void CheckUnregisterAdviseSink ()
		{
			if (criticalError == null && objectCreated == null && 
				objectMoved == null && objectDeleted == null && 
				objectCopied == null && extended == null && 
				searchComplete == null && tableModified == null && 
				newMail == null)
			{
				advisor.Unadvise (connection);
				adviseSink = null;
			}
		}

		private EventHandler<ErrorEventArgs> criticalError;
		private EventHandler<ObjectEventArgs> objectCreated;
		private EventHandler<ObjectEventArgs> objectModified;
		private EventHandler<ObjectEventArgs> objectMoved;
		private EventHandler<ObjectEventArgs> objectDeleted;
		private EventHandler<ObjectEventArgs> objectCopied;
		private EventHandler<ExtendedEventArgs> extended;
		private EventHandler<ObjectEventArgs> searchComplete;
		private EventHandler<TableEventArgs> tableModified;
		private EventHandler<NewMailEventArgs> newMail;

		/// <summary>
		///
		/// </summary>
		public event EventHandler<ErrorEventArgs> CriticalError {
			add {
				CheckRegisterAdviseSink ();
				criticalError += value;
			}
			remove {
				criticalError -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler<ObjectEventArgs> ObjectModified {
			add {
				CheckRegisterAdviseSink ();
				objectModified += value;
			}
			remove {
				objectModified -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler<ObjectEventArgs> ObjectCreated {
			add {
				CheckRegisterAdviseSink ();
				objectCreated += value;
			}
			remove {
				objectCreated -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler<ObjectEventArgs> ObjectMoved {
			add {
				CheckRegisterAdviseSink ();
				objectMoved += value;
			}
			remove {
				objectMoved -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler<ObjectEventArgs> ObjectDeleted {
			add {
				CheckRegisterAdviseSink ();
				objectDeleted += value;
			}
			remove {
				objectDeleted -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler<ObjectEventArgs> ObjectCopied {
			add {
				CheckRegisterAdviseSink ();
				objectCopied += value;
			}
			remove {
				objectCopied -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler<ExtendedEventArgs> Extended {
			add {
				CheckRegisterAdviseSink ();
				extended += value;
			}
			remove {
				extended -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler<ObjectEventArgs> SearchComplete {
			add {
				CheckRegisterAdviseSink ();
				searchComplete += value;
			}
			remove {
				searchComplete -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler<TableEventArgs> TableModified {
			add {
				CheckRegisterAdviseSink ();
				tableModified += value;
			}
			remove {
				tableModified -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler<NewMailEventArgs> NewMail {
			add {
				CheckRegisterAdviseSink ();
				newMail += value;
			}
			remove {
				newMail -= value;
				CheckUnregisterAdviseSink ();
			}
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnCriticalError (ErrorEventArgs ea)
		{
			if (criticalError != null)
				criticalError (this, ea);
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnObjectModified (ObjectEventArgs ea)
		{
			if (objectModified != null)
				objectModified (this, ea);
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnObjectCreated (ObjectEventArgs ea)
		{
			if (objectCreated != null)
				objectCreated (this, ea);
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnObjectMoved (ObjectEventArgs ea)
		{
			if (objectMoved != null)
				objectMoved (this, ea);
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnObjectDeleted (ObjectEventArgs ea)
		{
			if (objectDeleted != null)
				objectDeleted (this, ea);
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnObjectCopied (ObjectEventArgs ea)
		{
			if (objectCopied != null)
				objectCopied (this, ea);
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnExtended (ExtendedEventArgs ea)
		{
			if (extended != null)
				extended (this, ea);
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnSearchComplete (ObjectEventArgs ea)
		{
			if (searchComplete != null)
				searchComplete (this, ea);
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnTableModified (TableEventArgs ea)
		{
			if (tableModified != null)
				tableModified (this, ea);
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="ea"></param>
		public virtual void OnNewMail (NewMailEventArgs ea)
		{
			if (newMail != null)
				newMail (this, ea);
		}


		class MapiPseudoAdviseSink : IMapiAdviseSink
		{
			private ObjectEventSet eventSet;

			public MapiPseudoAdviseSink (ObjectEventSet eventSet)
			{
				this.eventSet = eventSet;
			}

			public void OnNotify (Notification [] notifications)
			{
				foreach (var noti in notifications) {
					switch (noti.EventType) {
						case NotificationEventType.CriticalError:
							eventSet.OnCriticalError (
								new ErrorEventArgs ((ErrorNotification) noti));
						break;
						case NotificationEventType.NewMail:
							eventSet.OnNewMail (
								new NewMailEventArgs ((NewMailNotification) noti));
						break;
						case NotificationEventType.ObjectCreated:
							eventSet.OnObjectCreated (
								new ObjectEventArgs ((ObjectNotification) noti,
								NotificationEventType.ObjectCreated));
						break;
						case NotificationEventType.ObjectDeleted:
							eventSet.OnObjectDeleted (
								new ObjectEventArgs ((ObjectNotification) noti,
								NotificationEventType.ObjectDeleted));
						break;
						case NotificationEventType.ObjectModified:
							eventSet.OnObjectModified (
								new ObjectEventArgs ((ObjectNotification) noti,
								NotificationEventType.ObjectModified));
						break;
						case NotificationEventType.ObjectMoved:
							eventSet.OnObjectMoved (
								new ObjectEventArgs ((ObjectNotification) noti,
								NotificationEventType.ObjectMoved));
						break;
						case NotificationEventType.ObjectCopied:
							eventSet.OnObjectCopied (
								new ObjectEventArgs ((ObjectNotification) noti,
								NotificationEventType.ObjectCopied));
						break;
						case NotificationEventType.SearchComplete:
							eventSet.OnSearchComplete (
								new ObjectEventArgs ((ObjectNotification) noti,
								NotificationEventType.SearchComplete));
						break;
						case NotificationEventType.TableModified:
							eventSet.OnTableModified (
								new TableEventArgs ((TableNotification) noti));
						break;
//						case NotificationEventType.StatusObjectModified:
//							eventSet.OnStatusObjectModified (
//								new NewMailEventArgs (noti.info.StatObj));
//						break;
//						case NotificationEventType.ReservedForMapi:
//							//TODO: construct args
//							eventSet.OnCriticalError (args);
//						break;
						case NotificationEventType.Extended:
							eventSet.OnExtended (
								new ExtendedEventArgs ((ExtendedNotification) noti));
						break;
					}
				}
			}
		}

	}


}
