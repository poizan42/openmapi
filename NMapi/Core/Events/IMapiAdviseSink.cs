//
// openmapi.org - NMapi C# Mapi API - IMapiAdviseSink.cs
//
// Copyright 2008 Topalis AG
//
// Author:    Johannes Roith <johannes@jroith.de>
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

namespace NMapi.Events {

	/// <summary>
	///  The IMAPIAdviseSink interface. This interface must be implemented 
	///   by classes that handle notifications. NMapi also supports events 
	///   through delegates, so please consider using them instead.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527094.aspx
	/// </remarks>
	public interface IMapiAdviseSink
	{
		/// <summary>
		///  This method serves as a callback-function when an event occurs.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527037.aspx
		/// </remarks>
		void OnNotify (Notification [] notifications);
	
	}

}
