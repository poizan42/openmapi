//
// openmapi.org - NMapi C# Mapi API - IndigoMapiSession.cs
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

namespace NMapi.Provider.Indigo {

	using System;
	using System.ServiceModel;
	using System.ServiceModel.Channels;

	using NMapi.Events;
	using NMapi.Table;
	using NMapi.Provider.Indigo.Table;
	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Properties.Special;
	using NMapi.Provider.Indigo.Properties.Special;

//	using NMapi.WCF.Xmpp;

	public class IndigoMapiSession : IMapiSession
	{
		private VirtualEvents virtualEvents;
		private IndigoMapiObjRef proxiedSession;

		private IMsgStore privatemdb;
		private IMsgStore publicmdb;

		private IMapiOverIndigo proxy;

		bool isDisposed = false;

		public IMapiOverIndigo Proxy {
			get { return proxy; }
		}

		public IndigoMapiSession ()
			: this ("localhost")
		{
			
		}

		public IndigoMapiSession (string host)
			: this (host, 9000, "provider = 'test'; blah = x;z=y")
		{
			
		}

		public IndigoMapiSession (string host, int port, string connectionString)
		{
//			EndpointAddress ep = new EndpointAddress ("net.xmpp://" + host + ":" +
//							 port + "/IMapiOverIndigo/MapiOverIndigoService");

			EndpointAddress ep = new EndpointAddress ("net.tcp://" + host + ":" +
							 port + "/IMapiOverIndigo/MapiOverIndigoService");


//			EndpointAddress ep = new EndpointAddress ("http://" + host + ":" + 
//							 port + "/IMapiOverIndigo/MapiOverIndigoService");

			this.virtualEvents = new VirtualEvents (this);
			IndigoCallback callback = new IndigoCallback (virtualEvents);
			InstanceContext context = new InstanceContext (callback);


//			WSDualHttpBinding binding = new WSDualHttpBinding ();
//			XmppTransportBinding binding = new XmppTransportBinding ();
			NetTcpBinding binding = new NetTcpBinding ();

			this.proxy = DuplexChannelFactory<IMapiOverIndigo>.CreateChannel (
					context, binding, ep);

			try {
				this.proxiedSession = proxy.OpenSession (connectionString);
			} catch (EndpointNotFoundException e) {
				string msg = "Endpoint not found! - " + e.Message;
				throw new MapiException (msg, Error.NetworkError);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		~IndigoMapiSession ()
		{
			Dispose ();
		}

		public void Dispose ()
		{
			try {
				proxy.CloseSession ();
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
			isDisposed = true;
		}

		public void Close () 
		{
			Dispose ();
		}

		public void Logon (string host, string user, string password)
		{
			try {
				Proxy.IMapiSession_Logon (proxiedSession, host, user, password);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public int Advise (object sender, IndigoMapiObjRef obj, byte[] entryID, 
			NotificationEventType eventMask, IMapiAdviseSink sink)
		{
			return virtualEvents.Advise (sender, obj, entryID, eventMask, sink);
		}

		public void Unadvise (IndigoMapiObjRef obj, int connection)
		{
			virtualEvents.Unadvise (connection);			
		}

		/// <summary>
		///  Get the private (personal) store of the session.
		/// </summary>
		public IMsgStore PrivateStore {
			get {
				if (privatemdb == null) {
					try {
						IndigoMapiObjRef objRef = Proxy.IMapiSession_GetPrivateStore (proxiedSession);
						privatemdb = new IndigoMsgStore (objRef, this);
					} catch (FaultException<MapiIndigoFault> e) {
						throw new MapiException (e.Detail.Message, e.Detail.HResult);
					}
				}
				return privatemdb;
			}
		}

		/// <summary>
		///  Get the public store of the session.
		/// </summary>
		public IMsgStore PublicStore {
			get {
				if (publicmdb == null) {
					try {
						IndigoMapiObjRef objRef = Proxy.IMapiSession_GetPublicStore (proxiedSession);
						publicmdb = new IndigoMsgStore (objRef, this);
					} catch (FaultException<MapiIndigoFault> e) {
						throw new MapiException (e.Detail.Message, e.Detail.HResult);
					}
				}
				return publicmdb;
			}
		}

		/// <summary>
		///  Gets the ENTRYID of the user.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529399.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public byte [] Identity {
			get {
				return Proxy.IMapiSession_GetIdentity (proxiedSession);
			}
		}

		public string GetConfig (string category, string id, int flags)
		{
			try {
				return Proxy.IMapiSession_GetConfig (proxiedSession, category, id, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
		public string GetConfigNull (string category, string id, int flags)
		{
			try {
				return Proxy.IMapiSession_GetConfigNull (proxiedSession, category, id, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public IBase CreateObject (IBase parent, IndigoMapiObjRef obj, NMapiGuid iid, int propTag)
		{
			switch (obj.Type) {
				case Mapi.Folder:
					if (((IndigoBase)parent).MapiObjectType == Mapi.Store)
						return new IndigoMapiFolder (obj, (IndigoMsgStore) parent);
					return new IndigoMapiFolder (obj, ((IndigoMapiFolder) parent).Store);
				case Mapi.Message:
					return new IndigoMessage (obj, (IndigoBase) parent);
				case Mapi.Attach:
					return new IndigoAttach (obj, (IndigoMessage) parent);
				case Mapi.SimpleStream:
					return new IndigoStream (obj, (IndigoBase) parent, propTag);
				case Mapi.Table:
					return new IndigoMapiTable (obj, (IndigoMapiFolder) parent);
				case Mapi.TableReader:
					return new IndigoMapiTableReader (obj, (IndigoBase) parent);
				default:
					Console.Write ("unknown type ");
					Console.Write (obj.Type);
					Console.WriteLine ();
					throw new MapiException (Error.BadValue);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public IBase CreateObject (IBase parent, IndigoMapiObjRef obj, NMapiGuid iid)
		{
			return CreateObject (parent, obj, iid, (int) PropertyType.Null);
		}
	}

}
