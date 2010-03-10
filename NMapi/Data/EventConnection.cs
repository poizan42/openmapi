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
		// TODO: Should be `uint'
		// See: `lpulConnection' at http://msdn.microsoft.com/en-us/library/cc842238.aspx
		private int connection;
		private bool initialized;

		/// <summary>
		///  
		/// </summary>
		public int Connection {
			get {
				return initialized ? connection : -1;

				// TODO: May be it's better to throw an exception!?
				if (initialized)
					return connection;

				throw new InvalidOperationException ("Connection not initialized!");
			}
		}

		public EventConnection (int connection)
		{
			this.initialized = true;
			this.connection = connection;
		}

		public override string ToString ()
		{
			return "" + connection;
		}
	}
}
