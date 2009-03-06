//
// openmapi.org - NMapi C# Mapi API - ReverseEventConnectionServer.cs
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
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

using System.Net;
using System.Net.Sockets;
using NMapi.Interop.MapiRPC;
using CompactTeaSharp;
using CompactTeaSharp.Server;

namespace NMapi.Server {

	public sealed class ReverseEventConnectionServer
	{
		private TcpClient tcpClient;
		private PseudoClient pseudoClient;
		private BaseOncRpcService service;
		
		public ReverseEventConnectionServer (
			BaseOncRpcService service, TcpClient tcpClient)
		{
			this.service = service;
			this.tcpClient = tcpClient;
		}
			
		internal class PseudoClient
		{
			private Queue<ClientEvent> queue;
			private TcpClient tcpClient;
			
			internal PseudoClient (TcpClient tcpClient)
			{
				this.queue = new Queue<ClientEvent> ();
				this.tcpClient = tcpClient;
			}
		
			internal void PushData (ClientEvType type, ClEvMapi realEvent, 
				ClEvProgress clientProgress)
			{
				ClientEvent clientEvent = new ClientEvent ();
				clientEvent.Type = type;
				clientEvent.Mapi = realEvent;
				clientEvent.Progress = clientProgress;
				lock (queue) {
					queue.Enqueue (clientEvent);
				}
			}
			
			int xid = new Random ().Next (0, 1000);
			
			internal void ProcessEvent ()
			{
				XdrVoid result = new XdrVoid ();
				ClientEvent ev = null;
				lock (queue) {
					if (queue.Count != 0)
						ev = queue.Dequeue ();
				}

				if (ev != null) {
					int program = 1;
					int version = 1;

					var sendingXdr = new XdrTcpEncodingStream (tcpClient, 8192);
					var callHeader = new OncRpcClientCallMessage (++xid, 
							program, version, 1, OncRpcClientAuthNone.AUTH_NONE);
					tcpClient.SendTimeout = 10;
					sendingXdr.BeginEncoding (null, 0);
					callHeader.XdrEncode (sendingXdr);
					ev.XdrEncode (sendingXdr);
					sendingXdr.EndEncoding ();

					if (ev.Mapi != null)
						Trace.WriteLine ("EVENT for connection '" + 
							ev.Mapi.ulConn + "' DELIVERED!");
				}
			}

		}
		
		private long RetrieveSessionHandle ()
		{
			byte[] sessionIdBigEndian = new byte [8];
			tcpClient.GetStream ().Read (sessionIdBigEndian, 0, 8);
			Array.Reverse (sessionIdBigEndian);
			return BitConverter.ToInt64 (sessionIdBigEndian, 0);
		}
		
		public void Handle ()
		{
			Trace.WriteLine ("Client connected on event channel!");
			long sessionId = RetrieveSessionHandle ();
			var proxySession = service.GetProxySessionBySessionId (sessionId);
			this.pseudoClient = new PseudoClient (tcpClient);
			
			// TODO: SYNCRONIZATION!
			proxySession.ReverseEventConnectionServer = this;
			
			while (true) {
				if (!tcpClient.Connected)
					return;
				pseudoClient.ProcessEvent ();
				Thread.Sleep (100); // TODO: bad!
			}
		}
		
		// TODO: Ready property - synchronized!
		
		public void PushEvent (ClEvMapi mapiEvent)
		{
			Trace.WriteLine ("Pushing Event!");
			pseudoClient.PushData (ClientEvType.CLEV_MAPI, mapiEvent, null);
			Trace.WriteLine ("DONE pushing Event!");
		}

		public void PushProgress (ClEvProgress mapiProgress)
		{
			Trace.WriteLine ("Pushing Progress!");
			pseudoClient.PushData (ClientEvType.CLEV_PROGRESS, null, mapiProgress);
		}
		
		public void Close ()
		{
			if (tcpClient != null) {
				try {
					try {
						if (tcpClient.Client != null)
							tcpClient.Client.Close ();
					} catch (Exception) {
						// ignore
					}
					tcpClient.Close ();
				} catch (Exception) {
					// ignore
				}
			}
		}

	}

}
