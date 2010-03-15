//
// openmapi.org - NMapi C# Mapi API - OncProxySession.cs
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
using System.Net;
using System.Text;
using System.Collections.Generic;

using System.ServiceModel;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

using NMapi.Interop;
using NMapi.Interop.MapiRPC;

namespace NMapi.Server {

	/// <summary>
	///
	/// </summary>
	public sealed class OncProxySession : ProxySession
	{
		private OncProgressDispatcher progressDispatcher;
		private OncEventDispatcher eventDispatcher;
		private ReverseEventConnectionServer reverseEventConnectionServer;
		private IPAddress peerAddress;
		private StreamHelper streamHelper;
		private long currentHObjId = 0;
		private int peerPort;
		private StateLevel currentState = StateLevel.SessionConnected;
		private object stateLock = new object ();
		
		public override string Protocol {
			get { return "openmapi/oncrpc"; }
		}

		public override string Source {
			get { return PeerAddress.ToString (); }
		}

		public override string ClientName {
			get { throw new NotImplementedException ("Not yet implemented!"); }
		}

		public override bool IsAuthenticated {
			get { throw new NotImplementedException ("Not yet implemented!"); }			
		}

		public override bool IsLocal {
			get { throw new NotImplementedException ("Not yet implemented!"); }
		}

		public override bool AllowShellAttachment {
			get { return true; }
		}

		public override bool IsSecure {
			get { return false; }
		}

		public override bool IsPipelined {
			get { return false; }
		}

		public override bool IsPersistent {
			get { return true; }
		}

		public override bool RequiresSessionKey {
			get { return false; }
		}
		
		public override IEventDispatcher EventDispatcher {
			get { return eventDispatcher; }
		}
		
		#region txc specific		

		internal IPAddress PeerAddress {
			get { return peerAddress; }
		}

		internal int PeerPort {
			get { return peerPort; }
		}
		
		internal StreamHelper StreamHelper {
			get { return streamHelper; }
		}

		internal ReverseEventConnectionServer ReverseEventConnectionServer {
			get { return reverseEventConnectionServer; }
			set { reverseEventConnectionServer = value; }
		}
		
		public OncProgressDispatcher ProgressDispatcher {
			get { return progressDispatcher; }
		}

		#endregion

		public override CommonRpcObjRef CreateRefObj (object obj)
		{
			var refObj = new CommonRpcObjRef ();
			refObj.MapiObject = obj;
			refObj.RpcObject = ++currentHObjId;
			return refObj;
		}

		public OncProxySession (IPAddress ip, int port, 
			CommonRpcService decoratedRpc) : base (decoratedRpc)
		{
			this.eventDispatcher = new OncEventDispatcher (this);
			this.progressDispatcher = new OncProgressDispatcher (this);
			this.streamHelper = new StreamHelper (this);
			this.peerAddress = ip;
			this.peerPort = port;
			
		}
		
		public enum StateLevel
		{
			SessionConnected,
			TryLogin,
			LoggedIn,
			SessionClosed
		}

		public void CanRaiseStateLevel (StateLevel state)
		{
			CommonRaiseStateLevel (state, false);
		}
		
		public void RaiseStateLevel (StateLevel state)
		{
			CommonRaiseStateLevel (state, true);
		}
		
		private void CommonRaiseStateLevel (StateLevel state, bool doEnter)
		{
			lock (stateLock) {
				if ((int) currentState < (((int) state)-1))
					throw new MapiInvalidAccessTimeException ("Call not allowed in current session state.");
				if (doEnter && ((int) state) > ((int) currentState))
					currentState = state;
			}
		}
		
		public override void Kill ()
		{
			// TODO!
			throw new NotImplementedException ("Not yet implemented!");
		}

	}

}
