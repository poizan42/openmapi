//
// openmapi.org - NMapi C# Mapi API - ProxySession.cs
//
// Copyright 2008 Topalis AG
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
using System.Collections.Generic;

namespace NMapi.Server {

	/// <summary>
	///   Represents session/connection from a client to the proxy.
	///   Usually an internal "connection" to a selected backend will 
	///   exist as well. ProxySessions are used to keep track of the 
	///   active Sessions and the resources.
	/// </summary>
	public abstract class ProxySession : MarshalByRefObject, IProxySession
	{
		private CommonRpcService rpc;
		private ObjectStore objectStore;
		private DateTime created;
		private string id;
		private string loginName;

		/// <summary>
		///  
		/// </summary>
		public CommonRpcService Rpc {
			get { return rpc; }
		}

		public ObjectStore ObjectStore {
			get { return objectStore; }
		}

		public ProxySession (CommonRpcService decoratedRpc)
		{
			this.rpc = decoratedRpc;
			this.objectStore = new ObjectStore (this);
		}

		/// <summary>
		///  The ID of the connection. This is globally unique.
		/// </summary>
		public string Id {
			get { return id; }
			set { id = value; }
		}
		
		/// <summary>
		///  The user name that has been used for the login.
		/// </summary>
		public string LoginName {
			get { return loginName; }
			set { loginName = value; }
		}
		
		/// <summary>
		///  The DateTime at the time the the Session was created.
		/// </summary>
		public DateTime InitDate {
			get { return created; }
		}

		/// <summary>
		///  The name of the protcol. Usually the form is "openmapi/somename".
		/// </summary>
		public abstract string Protocol {
			get;
		}

		/// <summary>
		///  A string describing the source of the connection, e.g. 
		///  a hostname or an ip address (or a jabber jid, etc.).
		/// </summary>
		public abstract string Source {
			get;
		}

		/// <summary>
		///  
		/// </summary>
		public abstract string ClientName {
			get;
		}

		/// <summary>
		///  
		/// </summary>
		public abstract bool IsAuthenticated {
			get;
		}

		/// <summary>
		///  
		/// </summary>
		public abstract bool IsLocal {
			get;
		}

		/// <summary>
		///  True if the proxy should be able to attach a MapiShell
		///  to the running connection.
		/// </summary>
		public abstract bool AllowShellAttachment {
			get;
		}

		/// <summary>
		///  True if the connection is considered to be secure.
		/// </summary>
		public abstract bool IsSecure {
			get;
		}

		/// <summary>
		///  True if calls are pipelined within the same session.
		/// </summary>
		public abstract bool IsPipelined {
			get;
		}
		
		/// <summary>
		///  True if the connection is persistent; Tcp would be an 
		///  example of this. Classic HTTP is a counter-example.
		/// </summary>
		public abstract bool IsPersistent {
			get;
		}

		/// <summary>
		///  True if the session keeps track of a session key itself to 
		///  map calls on the same instance to the same server-side object.
		/// </summary>
		public abstract bool RequiresSessionKey {
			get;
		}

		/// <summary></summary>
		public abstract CommonRpcObjRef CreateRefObj (object obj);

		/// <summary></summary>
		public abstract IEventDispatcher EventDispatcher {
			get;
		}
		
		internal Dictionary<IStream, int> streamToPropertyMap = new Dictionary<IStream, int> ();
		
		
		/// <summary>Kills the session.</summary>
		public abstract void Kill ();
		
		
		
		

/*
		/// <summary>
		///  Attaches a new MapiShell to the running connection.
		/// </summary>
		public IMapiShell AttachShell ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}
*/

	}

}
