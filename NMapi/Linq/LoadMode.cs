//
// openmapi.org - NMapi C# Mapi API - LoadMode.cs
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

namespace NMapi.Linq {

	/// <summary>
	///  Defines how data is loaded from the server.
	/// </summary>
	public enum LoadMode
	{

		/// <summary>
		///  
		/// </summary>
		PreFetch,

		/// <summary>
		///  
		/// </summary>
		Lazy,

		/// <summary>
		///  
		/// </summary>
		Stream
	}
}
