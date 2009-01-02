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
using NMapi.Interop;
using NMapi.Interop.MapiRPC;
using CompactTeaSharp;
using CompactTeaSharp.Server;

namespace NMapi.Server {

	public abstract class BaseOncRpcService : MAPIRPCServerStub
	{
		protected CommonRpcService commonRpcService;
		protected SessionManager sessionManager;
		protected Dictionary <string, OncProxySession> sessions;
		protected int eventPort;

		public BaseOncRpcService (CommonRpcService service, 
			SessionManager sman, IPAddress ip, int port) : base (ip, port)
		{
			this.sessionManager = sman;
			this.commonRpcService = service;
				
			var transport = transports [0] as OncRpcTcpServerTransport;
			if (transport == null)
				throw new Exception ("Transport is null!");
			transport.ConnectionClosed += ClientConnectionClosedHandler;
			
			this.sessions = new Dictionary <string, OncProxySession> ();
			this.eventPort = port+1;
			
			// Run listener on a new thread ...
			var listenerThread = new Thread (new ThreadStart (StartEventListener));
			listenerThread.Start ();
		}

		private void StartEventListener ()
		{
			TcpListener listener = new TcpListener (IPAddress.Any, eventPort);
			listener.Start ();
			while (true) {
				var tcpClient = listener.AcceptTcpClient ();

			//	tcpClient.ReceiveTimeout = timeout;
				tcpClient.NoDelay = true;
				
				int bufferSize = 8192; // TODO: OAOO !!
				
				if (tcpClient.ReceiveBufferSize < bufferSize)
					tcpClient.ReceiveBufferSize = bufferSize;
				if (tcpClient.SendBufferSize < bufferSize)	
					tcpClient.SendBufferSize = bufferSize;
				
				var evServer = new ReverseEventConnectionServer (this, tcpClient);
				var eventHandlerThread = new Thread (new ThreadStart (evServer.Handle));
				eventHandlerThread.Start ();
			}
		}

		private void ClientConnectionClosedHandler (object sender, ConnectionClosedEventArgs ea)
		{
			Trace.WriteLine ("Client closed connection!");
			string key = GetSessionKey (ea.IPAddress.ToString (), ea.Port);
			if (sessions.ContainsKey (key)) {
				sessionManager.UnregisterSession (sessions [key]);
				sessions [key] = null;
				Trace.WriteLine ("Session removed!!!");
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
