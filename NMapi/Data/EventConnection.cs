//
// openmapi.org - NMapi C# Mapi API - EventConnection.cs
//
// Copyright 2009 Topalis AG
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

namespace NMapi {

	using System;
	using System.Diagnostics;
	using NMapi.Flags;
	using NMapi.Properties;

	/// <summary>
	///  Represents an event connection.
	/// </summary>
	public struct EventConnection
	{
		private int connection;
		private bool initialized;
		
		/// <summary>
		///  
		/// </summary>
		public int Connection {
			get {
				if (!initialized)
					return -1;
				return connection;
			}
		}
		
		public EventConnection (int connection)
		{
			this.initialized = true;
			this.connection = connection;
		}
		
		// TODO: add equality comparison!!!, etc. ..
		
		public override string ToString ()
		{
			return "" + connection;
		}
		
		
	}

}
