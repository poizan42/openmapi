//
// openmapi.org - NMapi C# Mapi API - IMapiSession.cs
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
	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Properties.Special;

	/// <summary>
	///  The representation of a NMapi session. This is the starting
	///  point for every use of NMapi. The first call must be the 
	///  <see cref="M:MapiSession.Logon()">MAPISession.Logon</see> method.
	/// </summary>
	public interface IMapiSession : IDisposable
	{
		void Dispose ();
		void Close () ;
		void Logon (string host, string user, string password);
		IMsgStore PrivateStore { get; }
		IMsgStore PublicStore { get; }
		byte [] Identity { get; }
		string GetConfig (string category, string id, int flags);
		string GetConfigNull (string category, string id, int flags);
	
	}

}
