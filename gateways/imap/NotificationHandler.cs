// openmapi.org - NMapi C# IMAP Gateway - NotificationHandler.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Events;
using NMapi.Gateways.IMAP;


namespace NMapi.Gateways.IMAP {

	internal class NotificationHandler
	{
		private IMAPConnectionState imapConnectionState;
		private SBinary subscribeId;
		private IMapiTable mt;
		
		protected ServerConnection ServCon {
			get { return imapConnectionState.ServerConnection; }
		}
		
		public NotificationHandler (IMAPConnectionState state)
		{
			imapConnectionState = state;
			Subscribe ();
			state.NotificationHandler = this;			
		}

		private void CriticalErrorHandler (object sender, ErrorEventArgs ea)
		{
			Trace.WriteLine ("An Event has arrived");
		}

		private void ExtendedHandler (object sender, ExtendedEventArgs ea)
		{
			Trace.WriteLine ("An Event has arrived");
		}


		private void NewMailHandler (object sender, NewMailEventArgs ea)
		{
			Trace.WriteLine ("An Event has arrived");
		}


		private void ObjectCopiedHandler (object sender, ObjectEventArgs ea)
		{
			Trace.WriteLine ("An Event has arrived");
		}


		private void ObjectCreatedHandler (object sender, ObjectEventArgs ea)
		{
			Trace.WriteLine ("An Event has arrived");
		}


		private void ObjectDeletedHandler (object sender, ObjectEventArgs ea)
		{
		}

		private void ObjectModifiedHandler (object sender, ObjectEventArgs ea)
		{
			Trace.WriteLine ("An Event has arrived");
		}

		private void ObjectMovedHandler (object sender, ObjectEventArgs ea)
		{
		}


		private void SearchCompleteHandler (object sender, ObjectEventArgs ea)
		{
			Trace.WriteLine ("An Event has arrived");
		}

		private void TableModifiedHandler (object sender, TableEventArgs ea)
		{
			Trace.WriteLine ("An Event has arrived: " + ea.EventType + ":" + ea.Notification.TableEvent);
			Trace.WriteLine (ea.Notification.TableEvent.GetTypeCode ());
			
			switch (ea.Notification.TableEvent) {
			case TableNotificationType.Changed:
			case TableNotificationType.RowModified:
				TableChanged (ea.Notification);
				break;
			case TableNotificationType.Error:
				//PrepareReload (n.HResult);
				break;
			case TableNotificationType.Reload:
				//PrepareReload (null);
				break;
			case TableNotificationType.RowAdded:
				RowAdded (ea.Notification);
				break;
			case TableNotificationType.RowDeleted:
				RowDeleted (ea.Notification);
				break;
			}

		}

		private void RowDeleted (TableNotification n)
		{
			try {
			SBinary instanceKey = ((BinaryProperty) n.PropIndex).Value;

			SequenceNumberListItem snli = ServCon.SequenceNumberList.Find ((x)=> ServCon.CompareEntryIDs(x.InstanceKey.ByteArray, instanceKey.ByteArray));
			imapConnectionState.AddExpungeRequest (snli);
			} catch (Exception e) {
				Trace.WriteLine ("NotificationHandler.RowDeleted, Exception: " + e.Message);
				throw;
			}
		}
			
		private void RowAdded (TableNotification n)
		{
			// we evaluate which item was added
			// we only provoke a reevaluation of the Folder with Expunge and Exists Responses at the next command.
			try {
				imapConnectionState.AddExistsRequest (new SequenceNumberListItem ());
			} catch (Exception e) {
				Trace.WriteLine ("NotificationHandler.RowAdded, Exception: " + e.Message);
				throw;
			}
		}

		private void TableChanged (TableNotification n)
		{
			// we don't know if a delete or add or whatever happened.
			// we only provoke a reevaluation of the Folder with Expunge and Exists Responses at the next command.
			try {
				imapConnectionState.AddExistsRequest (new SequenceNumberListItem ());
			} catch (Exception e) {
				Trace.WriteLine ("NotificationHandler.TableChanged, Exception: " + e.Message);
				throw;
			}
		}
			
		private void Subscribe ()
		{
			mt = ServCon.CurrentFolderTable;
			mt.Events.TableModified += TableModifiedHandler;

/*			
			subscribeId = null;
			ServCon.Store.Events [subscribeId].CriticalError += CriticalErrorHandler;
			ServCon.Store.Events [subscribeId].Extended += ExtendedHandler;
			ServCon.Store.Events [subscribeId].NewMail += NewMailHandler;
			ServCon.Store.Events [subscribeId].ObjectCopied += ObjectCopiedHandler;
			ServCon.Store.Events [subscribeId].ObjectCreated += ObjectCreatedHandler;
			ServCon.Store.Events [subscribeId].ObjectDeleted += ObjectDeletedHandler;
			ServCon.Store.Events [subscribeId].ObjectModified += ObjectModifiedHandler;
			ServCon.Store.Events [subscribeId].ObjectMoved += ObjectMovedHandler;
			ServCon.Store.Events [subscribeId].SearchComplete += SearchCompleteHandler;
			ServCon.Store.Events [subscribeId].TableModified += TableModifiedHandler;	
*/
		}


		private void UnsubscribeEventHandlers ()
		{
			try {
				mt.Events.TableModified -= TableModifiedHandler;
			}
			catch (Exception) { }
	

/*
			ServCon.Store.Events [subscribeId].CriticalError -= CriticalErrorHandler;
			ServCon.Store.Events [subscribeId].Extended -= ExtendedHandler;
			ServCon.Store.Events [subscribeId].NewMail -= NewMailHandler;
			ServCon.Store.Events [subscribeId].ObjectCopied -= ObjectCopiedHandler;
			ServCon.Store.Events [subscribeId].ObjectCreated -= ObjectCreatedHandler;
			ServCon.Store.Events [subscribeId].ObjectDeleted -= ObjectDeletedHandler;
			ServCon.Store.Events [subscribeId].ObjectModified -= ObjectModifiedHandler;
			ServCon.Store.Events [subscribeId].ObjectMoved -= ObjectMovedHandler;
			ServCon.Store.Events [subscribeId].SearchComplete -= SearchCompleteHandler;
			ServCon.Store.Events [subscribeId].TableModified -= TableModifiedHandler;
*/
		}

		public void Dispose ()
		{
			UnsubscribeEventHandlers ();
		}
	

	}
}
