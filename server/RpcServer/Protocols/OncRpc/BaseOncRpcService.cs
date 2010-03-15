//
// openmapi.org - NMapi C# Mapi API - BaseOncRpcService.cs
//
// Copyright 2009 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using NMapi.Interop;
using NMapi.Interop.MapiRPC;
using CompactTeaSharp;
using CompactTeaSharp.Server;

namespace NMapi.Server {

	public abstract class BaseOncRpcService : MAPIRPCServerStub, IDisposable
	{
		protected static BooleanSwitch oncServerTrace = new BooleanSwitch ("oncServerTrace", 
												"configured in application config file!");
		
		protected CommonRpcService commonRpcService;
		protected SessionManager sessionManager;
		protected Dictionary <string, OncProxySession> sessions;
		protected int eventPort;
		
		protected bool disposed;
		
		private volatile bool closing;
		private TcpListener listener;
		private Thread listenerThread;
		
		private SslStore sslParams;
		
		public BaseOncRpcService (CommonRpcService service, 
			SessionManager sman, IPAddress ip, int port, 
			SslStore sslParams) : base (ip, port, true, sslParams)
		{
			this.sessionManager = sman;
			this.commonRpcService = service;
			this.sslParams = sslParams;
			
			var transport = transports [0] as OncRpcTcpServerTransport;
			if (transport == null)
				throw new ArgumentNullException ("transport");
			transport.ConnectionClosed += ClientConnectionClosedHandler;
			
			this.sessions = new Dictionary <string, OncProxySession> ();
			this.eventPort = port+1;
			
			// Run listener on a new thread ...
			this.listenerThread = new Thread (new ThreadStart (StartEventListener));
			this.listenerThread.Start ();
		}
		
		/// <summary></summary>
		public virtual void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		/// <summary></summary>
		protected virtual void Dispose (bool disposing)
		{
			if (disposed)
				return;

			closing = true;
			
			// shut down the event-thread.
			if (listenerThread != null) {

				try {
					if (listenerThread != null && listener != null) {
						try {
							listener.Stop ();
						} catch (SocketException) {
							throw; // TODO !
						}
						// TODO: we wait for the thread to stop here ...
						listenerThread.Join ();
					}
				} catch (ThreadStateException) {
					// ignore.
				}
			}
			
			try {
				base.StopRpcProcessing ();
			} catch (Exception e) {
				// ignore ...
				Console.WriteLine (e);
			}
			
			disposed = true;
		}
		
		~BaseOncRpcService ()
		{
			if (!disposed)
				Dispose (false);
		}

		// TODO: current SSH can connect on port 8000 and 8001 (because of SSL). 
		//       The same if true for HTTPS (which will result in a download of the first server-response (the ID [= 2L] of the session-object))
		// We need to ensure that such invalid attempts fail, droping the connection if no proper RPC calls are coming in.
		// We also need to check that the calls coming in are correct. The easiest thing to do, would be to check a header, of course.
		// 
		// Furthermore, even if the clients DO speak proper MAPI/ONC, they could attempt to send garbage.
		// We must deal with this by:
		//  - blocking any requests that are not in our set of known calls.
		//  - blocking any calls that are made while the mapi-session has not been opened.
		//  - blocking MOST calls (except admin*, session*) that are made when no login call has been made.
		//  - verifying all data that is processed in the server-part. (session-objects, data-maps, etc.)
		//  - ensuring that invalid ONC encoding/decoding only brings down a session, never the server.
		
		private void StartEventListener ()
		{
			this.listener = new TcpListener (IPAddress.Any, eventPort);
			listener.Start ();
			while (!closing) {
				TcpClient tcpClient = null;

				try {
					tcpClient = listener.AcceptTcpClient ();
				} catch (SocketException) {
					// If we are closing the service, we will get an 
					// expected exception ...
					if (closing)
						return;
					throw;
				}
				
//				tcpClient.ReceiveTimeout = timeout;
				tcpClient.NoDelay = true;
				
				int bufferSize = 8192; // TODO: OAOO !!
				
				if (tcpClient.ReceiveBufferSize < bufferSize)
					tcpClient.ReceiveBufferSize = bufferSize;
				if (tcpClient.SendBufferSize < bufferSize)
					tcpClient.SendBufferSize = bufferSize;
				
				var evServer = new ReverseEventConnectionServer (this, tcpClient, sslParams);
				var eventHandlerThread = new Thread (new ThreadStart (evServer.Handle));
				eventHandlerThread.Start ();
			}
		}

		private void ClientConnectionClosedHandler (object sender, ConnectionClosedEventArgs ea)
		{
			if (oncServerTrace.Enabled)
				Trace.WriteLine ("Client closed connection!");

			string key = GetSessionKey (ea.IPAddress.ToString (), ea.Port);
			if (sessions.ContainsKey (key)) {
				var session = sessions [key];
				// attempt to close all backend objects
				try {
					session.ObjectStore.CloseAll ();
				} finally {
					if (session.ReverseEventConnectionServer != null)
						session.ReverseEventConnectionServer.Close ();
					sessionManager.UnregisterSession (session);
					sessions.Remove (key);			
					if (oncServerTrace.Enabled)
						Trace.WriteLine ("Session removed!!!");
				}
			}
		}
		
		protected long RegisterMapiSession (Request request, string dataString)
		{
			var session = request.ProxySession as OncProxySession;
			var mapiSessionCommonRpcObj = session.Rpc.OpenSession (request, dataString);
			return (long) request.ProxySession.ObjectStore.MapObject (mapiSessionCommonRpcObj.MapiObject).RpcObject;
		}
		
		protected string GetSessionKey (string ip, int port)
		{
			return ip + "_" + port;
		}
		
		protected OncProxySession GetProxySessionForConnection (OncRpcCallInformation call)
		{
			string key = GetSessionKey (call.PeerAddress.ToString (), call.PeerPort);
			return sessions [key];
		}

		protected void LogException (Exception e)
		{
			Console.WriteLine (e);
		}
		
		// TODO: TERRIBLE WORK-AROUND!! THIS ONLY WORKS WITH EXACTLY ONE SESSION CONNECTED
		//		and also has some security problems!
		public OncProxySession GetProxySessionBySessionId (long sessionId)
		{
			foreach (var first in sessions)
				return first.Value;
			return null;
		}
		
	}

}
