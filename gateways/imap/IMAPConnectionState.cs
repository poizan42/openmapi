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

namespace NMapi.Gateways.IMAP {

	public enum IMAPConnectionStates {NOT_AUTHENTICATED, AUTHENTICATED, SELECTED, LOGOUT};
	
	public class IMAPConnectionState
	{
		
		private IMAPConnectionStates currentState;
		private ClientConnection clientConnection;
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

		public IMAPConnectionStates CurrentState {
			get { return currentState; }
			set { currentState = value; }
		}
		
		public ClientConnection ClientConnection {
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

		internal NotificationHandler NotificationHandler {
			get { return notificationHandler; }
			set {
				if (notificationHandler != null)
					notificationHandler.Dispose ();
				notificationHandler = value;
			}
		}

		public IMAPConnectionState (TcpClient tcpClient)
		{
			currentState = IMAPConnectionStates.NOT_AUTHENTICATED;
			clientConnection = new ClientConnection (tcpClient);
			commandAnalyser = new CommandAnalyser (clientConnection);
			commandAnalyser.StateNotAuthenticated = this.StateNotAuthenticated;
			commandAnalyser.StateAuthenticated = this.StateAuthenticated;
			commandAnalyser.StateSelected = this.StateSelected;
			commandAnalyser.StateLogout = this.StateLogout;
			commandProcessor = new CommandProcessor (this);
			responseManager = new ResponseManager(this);
			folderMappingAgent = new FolderMappingAgent (this);
			ResetExpungeRequests ();
			ResetExistsRequests ();
			IMAPGatewayConfig config = IMAPGatewayConfig.read ();
			timeout = config.Imapconnectiontimeout;
			ResetTimeout ();
		}

		public void Close ()
		{
			if (notificationHandler != null)
				notificationHandler.Dispose ();
			
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
			serverConnection = new ServerConnection (host, user, password);
			folderMappingAgent = new FolderMappingAgent (this);
		}

		public void Disconnect()
		{
			if (serverConnection != null)
				serverConnection.Disconnect();
		 	serverConnection = null;
		}

		public void RunLoop()
		{
			if (clientConnection.DataAvailable () && !loopEnd) {
				ResetTimeout ();
				Trace.WriteLine("sldkfj");
				commandAnalyser.CheckCommand ();
				Queue q = commandAnalyser.CommandQueue;
				while (q.Count > 0) {
					Command cmd = (Command) q.Dequeue();
					Trace.WriteLine("command processing: \""+cmd.Command_name+"\"");						
					commandProcessor.ProcessCommand(cmd);
				}
			}
			ProcessTimeout ();
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
			lock (expungeRequests) {
				expungeRequests.Add (snli);
			}
		}

		public List<Response> ProcessNotificationResponses ()
		{
			Response r;
			List<Response> l = new List<Response> ();
			
			lock (expungeRequests) {
				foreach (SequenceNumberListItem snli in expungeRequests) {
					// instead of doing anything provoke a handling of existsRequests
					AddExistsRequest (new SequenceNumberListItem ());
					break;
/*					
					long sqn = (long) serverConnection.SequenceNumberList.IndexOfSNLI(snli);
					if (sqn > 0) {
						r = new Response (ResponseState.NONE, "EXPUNGE");
						r.Val = new ResponseItemText (sqn.ToString ());
						l.Add (r);
						serverConnection.SequenceNumberList.Remove (snli);
					}
*/				}
			
				expungeRequests = new SequenceNumberList ();

				// we keep the lock on the expungeRequests while processing the existsRequests
				lock (existsRequests) {
					if (existsRequests.Count != 0) {

						// save old SequenceNumberList
						SequenceNumberList snlOld = serverConnection.SequenceNumberList;

						// TODO: only append the missing lines from existsRequests + getting Additional Info from MAPI
						// build sequence number list
						serverConnection.BuildSequenceNumberList ();

						// fix UIDS in Messagesif missing or broken
						int recent = serverConnection.FixUIDsInSequenceNumberList ();
	
						// restore Notificationsubscription as currentFolderTable has changed
						new NotificationHandler (this);

						// as we only get a TableChanged-Notification if more than one Item is being deleted
						// we need to do this:
						// identify deleted items and create ExpungeResponses
						foreach (SequenceNumberListItem snliOld in snlOld.ToArray ()) {
							if (serverConnection.SequenceNumberList.Find ((x)=>x.UID == snliOld.UID) == null) {
								long sqn = snlOld.IndexOfSNLI (snliOld);
								if (sqn > 0) {
									r = new Response (ResponseState.NONE, "EXPUNGE");
									r.Val = new ResponseItemText (sqn.ToString ());
									l.Add (r);
									snlOld.Remove (snliOld); // remove item, that expunge sequence ids stay consistent
								}
							}
						}

						// EXISTS Responses
						r = new Response (ResponseState.NONE, "EXISTS");
						r.Val = new ResponseItemText(serverConnection.SequenceNumberList.Count.ToString ());
						l.Add (r);
	
						existsRequests = new SequenceNumberList ();
					}				
				}
				return (l);
			}
		}

		public void DoWork () 
		{
			while (!loopEnd) {
				Thread.Sleep(50);
				
	//				Trace.WriteLine("warte");
				lock (this){
					
					RunLoop();					
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
	}

}
