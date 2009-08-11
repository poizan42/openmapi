// openmapi.org - NMapi C# IMAP Gateway - IMAPConnectionState.cs
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
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using NMapi.Format.Mime;

namespace NMapi.Gateways.IMAP {

	public enum IMAPConnectionStates {NOT_AUTHENTICATED, AUTHENTICATED, SELECTED, LOGOUT};
	
	public class IMAPConnectionState
	{
		
		private IMAPConnectionStates currentState;
		private AbstractClientConnection clientConnection;
		private ServerConnection serverConnection;
		private CommandAnalyser commandAnalyser;
		private CommandProcessor commandProcessor;
		private ResponseManager responseManager;
		private SequenceNumberList expungeRequests;
		private SequenceNumberList existsRequests;
		private NotificationHandler notificationHandler;
		private FolderMappingAgent folderMappingAgent;
		private bool loopEnd = false;
		private DateTime timeoutStamp;
		private string timeout;
		private int id;
		private IMAPGatewayConfig config;
		private MimeCacheObject cacheObject;
		
		private static int idLast;
		private static Object lockObject = new Object ();

		public static Object LockObject {
			get { return lockObject; }
		}
		
		public IMAPConnectionStates CurrentState {
			get { return currentState; }
			set { currentState = value; }
		}
		
		public AbstractClientConnection ClientConnection {
			get { return clientConnection; }
		}
		
		public ServerConnection ServerConnection {
			get { return serverConnection; }
		}
		
		public CommandAnalyser CommandAnalyser {
			get { return commandAnalyser; }
		}
		
		public CommandProcessor CommandProcessor {
			get { return commandProcessor; }
		}
		
		public ResponseManager ResponseManager {
			get { return responseManager; }
		}

		public FolderMappingAgent FolderMappingAgent {
			get { return folderMappingAgent; }
		}

		public IMAPGatewayConfig Config {
			get { return config; }
		}

		internal NotificationHandler NotificationHandler {
			get { return notificationHandler; }
			set {
				if (notificationHandler != null)
					notificationHandler.Dispose ();
				notificationHandler = value;
			}
		}

		public IMAPConnectionState (TcpClient tcpClient) :
			this (new ClientConnection (tcpClient))
		{
		}

		public IMAPConnectionState (AbstractClientConnection clientConn)
		{
			id = idLast++;
			currentState = IMAPConnectionStates.NOT_AUTHENTICATED;
			if (clientConn != null) {
				clientConnection = clientConn;
				clientConnection.LogInput = this.Log;
				clientConnection.LogOutput = this.Log;
				commandAnalyser = new CommandAnalyser (clientConnection);
				commandAnalyser.StateNotAuthenticated = this.StateNotAuthenticated;
				commandAnalyser.StateAuthenticated = this.StateAuthenticated;
				commandAnalyser.StateSelected = this.StateSelected;
				commandAnalyser.StateLogout = this.StateLogout;
			}
			commandProcessor = new CommandProcessor (this);
			responseManager = new ResponseManager(this);
			folderMappingAgent = new FolderMappingAgent (this);
			ResetExpungeRequests ();
			ResetExistsRequests ();
			config = IMAPGatewayConfig.read ();
			timeout = config.Imapconnectiontimeout;
			ResetTimeout ();
		}

		public void Close ()
		{
			Disconnect ();
			
			if (clientConnection != null)
				clientConnection.Close ();
			
			loopEnd = true;
		}
		
		private bool StateNotAuthenticated ()
		{
			return currentState == IMAPConnectionStates.NOT_AUTHENTICATED;
		}
		
		private bool StateAuthenticated ()
		{
			return currentState == IMAPConnectionStates.AUTHENTICATED;
		}
		
		private bool StateSelected ()
		{
			return currentState == IMAPConnectionStates.SELECTED;
		}
		
		private bool StateLogout ()
		{
			return currentState == IMAPConnectionStates.LOGOUT;
		}
		
		public void InitServerConnection (string host, string user, string password)
		{
			Disconnect ();
			
			serverConnection = new ServerConnection (this, host, user, password);
			folderMappingAgent = new FolderMappingAgent (this);
		}

		public void Disconnect()
		{
			if (notificationHandler != null)
				notificationHandler.Dispose ();
			notificationHandler = null;
			
			if (serverConnection != null)
				serverConnection.Disconnect();
		 	serverConnection = null;
		}

		public void RunLoop()
		{
			if (!loopEnd && clientConnection.DataAvailable ()) {
				ResetTimeout ();
				Log ("sldkfj");
				commandAnalyser.CheckCommand ();
				if(commandAnalyser == null) loopEnd = true;
				else {
					Queue q = commandAnalyser.CommandQueue;
					while (q.Count > 0) {
						Command cmd = (Command) q.Dequeue();
						Log ("command processing: \""+cmd.Command_name+"\"");						
						commandProcessor.ProcessCommand(cmd);
					}
				}
			}
			ProcessTimeout ();
		}

		public void AddExistsRequestDummy ()
		{
			lock (existsRequests) {
					existsRequests.Add (new SequenceNumberListItem ());
			}
		}

		public void AddExistsRequest (SequenceNumberListItem snli)
		{
			lock (existsRequests) {
					existsRequests.Add (snli);
			}
		}

		public void ResetExistsRequests ()
		{
			if (existsRequests != null) {
				lock (existsRequests) {
					existsRequests = new SequenceNumberList ();
				}
			} else {
				existsRequests = new SequenceNumberList ();
			}
		}
		
		public void ResetExpungeRequests ()
		{
			if (expungeRequests != null) {
				lock (expungeRequests) {
					expungeRequests = new SequenceNumberList ();
				}
			} else {
				expungeRequests = new SequenceNumberList ();
			}			
		}

		public void AddExpungeRequest (SequenceNumberListItem snli)
		{
Log ("AddExpungeRequest :"+snli.UID);
			lock (expungeRequests) {
				expungeRequests.Add (snli);
			}
		}

		public List<Response> ProcessNotificationResponses ()
		{
			Response r;
			List<Response> l = new List<Response> ();
			SequenceNumberList localExpungeRequests = null;
			bool bExistsRequests = false;
Log ( "ProcessNotificationRespo01");

			// keep locks only to get local copies of the info and to reset the lists
			// otherwise we create deadlocks with incoming notifications, as soon as we
			// start to create missing uids for messages.
			lock (expungeRequests) {
				localExpungeRequests = expungeRequests;				
				if (localExpungeRequests.Count > 0) ResetExpungeRequests ();
			}


			
Log ( "ProcessNotificationRespo02");
			lock (existsRequests) {
				bExistsRequests = existsRequests.Count > 0;
				if (bExistsRequests) ResetExistsRequests ();
			}

			// instead of doing anything simulate the existance of an existsRequests
			if (localExpungeRequests.Count > 0) bExistsRequests = true;
/*			foreach (SequenceNumberListItem snli in localExpungeRequests) {
				ulong sqn = (long) serverConnection.SequenceNumberList.IndexOfSNLI(snli);
				if (sqn > 0) {
					r = new Response (ResponseState.NONE, "EXPUNGE");
					r.Val = new ResponseItemText (sqn.ToString ());
					l.Add (r);
					serverConnection.SequenceNumberList.Remove (snli);
				}
			}
*/			


Log ( "ProcessNotificationRespo03 " + bExistsRequests);
			if (bExistsRequests && serverConnection != null) {
Log ( "ProcessNotificationRespo04");

				// save old SequenceNumberList
				SequenceNumberList snlOld = serverConnection.FolderHelper.SequenceNumberList;
				// save size of old list;
				int snlOldLength = snlOld.Count;

Log ( "ProcessNotificationRespo05" + notificationHandler);
				if (notificationHandler != null)
					notificationHandler.Dispose();

				// TODO: only append the missing lines from existsRequests + getting Additional Info from MAPI

Log ( "ProcessNotificationRespo1");
				serverConnection.FolderHelper.RebuildSequenceNumberListPlusUIDFix ();
Log ( "ProcessNotificationRespo2");
				
				// restore Notificationsubscription as currentFolderTable has changed
				Log ("new NotificationHandler");
				new NotificationHandler (this);
Log ( "ProcessNotificationRespo3");
				
				// do Expunge handling
				// as we only get a TableChanged-Notification if more than one Item is being deleted
				// we need to do this:
				// identify deleted items and create ExpungeResponses
				Log ("do Expunge handling");
				SequenceNumberListItem snliNew = null;
				foreach (SequenceNumberListItem snliOld in snlOld.ToArray ()) {
					snliNew = serverConnection.FolderHelper.SequenceNumberList.Find ((x)=>x.UID == snliOld.UID);
					if (snliNew == null) {
						long sqn = snlOld.IndexOfSNLI (snliOld);
						if (sqn > 0) {
							r = new Response (ResponseState.NONE, "EXPUNGE");
							r.Val = new ResponseItemText (sqn.ToString ());
							l.Add (r);
							snlOld.Remove (snliOld); // remove item, that expunge sequence ids stay consistent
						}
					}
					
				}
Log ( "ProcessNotificationRespo4");
				
				// do Flag changes
				Log ("do Flag changes");
				foreach (SequenceNumberListItem snliOld in snlOld.ToArray ()) {
					snliNew = serverConnection.FolderHelper.SequenceNumberList.Find ((x)=>x.UID == snliOld.UID);
					if (snliNew != null) {
						Log ("uid: " + snliNew.UID);								
						Log ("checkFlags: " + snliNew.MessageFlags + ":" + snliOld.MessageFlags);								
						Log ("MsgStatus: " + snliNew.MsgStatus + ":" + snliOld.MsgStatus);								
						Log ("FlagStatus: " + snliNew.FlagStatus + ":" + snliOld.FlagStatus);								
						Log ("AdditionalFlags: " + 
							snliNew.GetAdditionalFlagsAsString () + ":" + snliOld.GetAdditionalFlagsAsString ());								
						if (!FlagHelper.FlagsEqual (snliNew, snliOld)) {
							r = new Response (ResponseState.NONE, "FETCH");
							r.Val = new ResponseItemText (snlOld.IndexOfSNLI (snliOld).ToString ());
							r.AddResponseItem (new ResponseItemList ()
							    .AddResponseItem ("UID")
							    .AddResponseItem (snliNew.UID.ToString ())
								.AddResponseItem ("FLAGS")
								.AddResponseItem (new FlagHelper (snliNew).ResponseItemListFromFlags ()));
							l.Add (r);
						}
					}
					
				}
Log ( "ProcessNotificationRespo5");

				if (bExistsRequests) {
					// EXISTS Responses
					r = new Response (ResponseState.NONE, "EXISTS");
					r.Val = new ResponseItemText(serverConnection.FolderHelper.SequenceNumberList.Count.ToString ());
					l.Add (r);
				}
			}				
Log ( "ProcessNotificationRespo6");
			return (l);
		}

		public void DoWork () 
		{
			while (!loopEnd) {
				Thread.Sleep(5);
				
				// lock execution against the execution of a notification request (see NotificationHandler)
//				lock (this){
				try {
					RunLoop();
				} catch (Exception e) {
					Log ("IMAPConnectionState.DoWork (): Exception occured: " + e.Message);
					Log (e.StackTrace);
				}
			}
		}

		public void ResetTimeout ()
		{
			timeoutStamp = DateTime.Now;
		}

		public void ProcessTimeout ()
		{
			int minutes;
			try {
				minutes = Convert.ToInt32 (timeout);
			} catch (Exception) {
				minutes = 30;
			}
			
			DateTime x = timeoutStamp.AddMinutes (minutes);

			if (x < DateTime.Now) {
				responseManager.AddResponse (new Response (ResponseState.NONE, "BYE").AddResponseItem ("IMAPGateway connection timeout", ResponseItemMode.ForceAtom));
				Close ();
			}
		}

		public void Log (string text)
		{
			Log (text, null);
		}
		
		public void Log (string text, string tag)
		{
			DateTime now = DateTime.Now;
			Console.Out.WriteLine (now.Year.ToString ().PadLeft (2,'0') + 
			                 now.Month.ToString ().PadLeft (2,'0') + 
			                 now.Day.ToString ().PadLeft (2,'0') + 
			                 "-" + 
			                 now.Hour.ToString ().PadLeft (2,'0') + 
			                 now.Minute.ToString ().PadLeft (2,'0') + 
			                 now.Second.ToString ().PadLeft (2,'0') + 
			                 "." + 
			                 now.Millisecond.ToString ().PadLeft (3,'0') +
			                 "||" + id.ToString ().PadLeft (3) +
			                 "||" + ((serverConnection != null) ? serverConnection.User : "" ).ToString ().PadLeft (3) +
			                 "||" + text);
		}

		// Cache handling, use for handling of full messages only!!!
		public void SetCache (SBinary entryId, MimeMessage message) {
			if (entryId != null && message != null)
				cacheObject = new MimeCacheObject (entryId, message);
		}

		public MimeMessage GetCache (SBinary entryId) {
			if (cacheObject != null && serverConnection.CompareEntryIDs (entryId.ByteArray, cacheObject.EntryId.ByteArray)) {
				return cacheObject.Message;
			}
			return null;
		}

		public void ClientConnectionSend (string s)
		{
			if (clientConnection != null)
				clientConnection.Send (s);
			else
				Log (s);
		}
		
	}

	public class MimeCacheObject
	{
		private SBinary entryId;
		private MimeMessage message;

		public SBinary EntryId{ 
			get { return entryId; } 
			set { entryId = value; } 
		}

		public MimeMessage Message{
			get { return message; }
			set { message = value; }
		}

		public MimeCacheObject (SBinary entryId, MimeMessage message) {
			this.entryId = entryId;
			this.message = message;
		}
	}

}
