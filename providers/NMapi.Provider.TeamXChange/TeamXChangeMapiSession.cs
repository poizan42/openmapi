//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMapiSession.cs
//
// Copyright 2008 VipCom AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
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

namespace NMapi {

	using System;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;
	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Properties.Special;

	/// <summary>
	///  The representation of a NMapi session. This is the starting
	///  point for every use of NMapi. The first call must be the 
	///  <see cref="M:MapiSession.Logon()">MAPISession.Logon</see> method.
	/// </summary>
	public class TeamXChangeMapiSession : IMapiSession
	{
		bool isDisposed = false;
		internal TeamXChangeSession session;
		private TeamXChangeMsgStore privatemdb;
		private TeamXChangeMsgStore publicmdb;

		public TeamXChangeMapiSession () 
		{
		}

		~TeamXChangeMapiSession ()
		{
			Dispose ();
		}

		/// <summary>
		///  Call Dispose when you are finished using the
		///  MapiSession-Class in order to release network resources.
		///  After calling the method the class is in an unusable state. 
		///  You should release all refernces to it, so it can be garbage-collected.
		/// </summary>
		public void Dispose ()
		{
			if (isDisposed)
				return;

			if (privatemdb != null) {
				privatemdb.Close2 ();
				privatemdb = null;
			}
			if (publicmdb != null) {
				publicmdb.Close2 ();
				publicmdb = null;
			}
			if (session != null) {
				session.Close ();
				session = null;
			}

			isDisposed = true;
		}

		/// <summary>
		///  This is the same as <see cref="M:MapiSession.Dispose()">Dispose ()</see>
		/// </summary>
		public void Close () 
		{
			Dispose ();
		}
		
		/// <summary>
		///  Logon to the server.
		/// </summary>
		/// <param name="host">The hostname of the server</param>
		/// <param name="user">The user name</param>
		/// <param name="password">The password of the user</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void Logon (string host, string user, string password)
		{
			Logon2 (host, -1, Common.SessionFlags.LogonIsUMapi, user, password, 65001, 0); // TODO: pass sessionFlags???
		}

		/// <summary>
		///  Logon to the server.
		/// </summary>
		/// <param name="host">The hostname of the server</param>
		/// <param name="user">The user name</param>
		/// <param name="password">The password of the user</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void Logon (string host, int sessionFlags, string user, string password, int codePage)
		{
			Logon2 (host, -1, Common.SessionFlags.LogonIsUMapi, user, password, codePage, 0); // TODO: pass sessionFlags???
		}

		/// <summary>
		///  Logon to the server.
		/// </summary>
		/// <param name="host">The hostname of the server</param>
		/// <param name="user">The user name</param>
		/// <param name="password">The password of the user</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void Logon (string host, int port, string user, string password)
		{
			Logon2 (host, port, Common.SessionFlags.LogonIsUMapi, user, password, 65001, 0);
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		private void Logon2 (string host, int port, int sessionFlags, string user, string password, int codePage, int localeId)
		{
			try {
				if (port != -1)
					session = new TeamXChangeSession (host, port);
				else
					session = new TeamXChangeSession (host);
				session.Logon2 (user, password, sessionFlags, codePage, localeId);
				privatemdb = (TeamXChangeMsgStore) OpenStore (Mdb.Write, null, false);
				publicmdb  = (TeamXChangeMsgStore) OpenStore (Mdb.Write, null, true);
			} 
			catch (MapiException e) {
				Console.WriteLine (e);
				if (privatemdb != null) {
					privatemdb.Close2 ();
					privatemdb = null;
				}
				if (publicmdb != null) {
					publicmdb.Close2 ();
					publicmdb = null;
				}
				throw;
			}
		}

		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public IMsgStore OpenStore (Mdb flags, string user, bool isPublic) 
		{
			return session.OpenStore (flags, user, isPublic);
		}
		
		/// <summary>
		///  Get the private (personal) store of the session.
		/// </summary>
		public IMsgStore PrivateStore {
			get { return privatemdb; }
		}

		/// <summary>
		///  Get the public store of the session.
		/// </summary>
		public IMsgStore PublicStore {
			get { return publicmdb; }
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
				IMsgStore store = (privatemdb != null) ? privatemdb : publicmdb;
				var propHelper = new MapiPropHelper (store);
				BinaryProperty binProp = ((BinaryProperty) propHelper.HrGetOneProp (Property.IdentityEntryId));
				return binProp.Value.lpb;
			}
		}
	
		/// <summary>
		///  Gets a server config variable.
		/// </summary>
		/// <param name="category">The category of the variable</param>
		/// <param name="id">The id of the variable </param>
		/// <param name="flags">Mapi.Unicode to get it as unicode string.</param>
		/// <returns>The value of the variable.</returns>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public string GetConfig (string category, string id, int flags)
		{
			TeamXChangeSession session;
			session = (privatemdb != null) ? privatemdb.session : publicmdb.session;
			return session.GetConfig (category, id, flags);
		}
	
		/// <summary>
		///  Gets a server config variable.
		/// </summary>
		/// <param name="category">The category of the variable</param>
		/// <param name="id">The id of the variable </param>
		/// <param name="flags">Mapi.Unicode to get it as unicode string.</param>
		/// <returns>The value of the variable or null if not found.</returns>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public string GetConfigNull (string category, string id, int flags)
		{
			try {
				return GetConfig (category, id, flags);
			}
			catch (MapiException e) {
				if (e.HResult == Error.NotFound)
					return null;
				throw e;
			}
		}
		
			
		public Address ResolveEntryID (byte [] eid)
		{
			return session.ResolveEntryID (eid);
		}

		public Address ResolveSmtpAddress (string smtpaddress, string displayname)
		{
			return session.ResolveSmtpAddress (smtpaddress, displayname);
		}
		
		/// <summary>
		///  Register a unique client id for synchronization.
		///  Also puts store into cached mode
		/// </summary>
		/// <param  name="id">A unique client id, preferable a uuid</param>
		public void RegisterSyncClientID (byte[] id)
		{
			session.RegisterSyncClientID (id);
		}
		
		
		
	}

}
