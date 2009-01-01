//
// openmapi.org - NMapi C# Mapi API - IAddrBook.cs
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

using System;

using NMapi;
using NMapi.Events;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.AddressBook {

	/// <summary>
	///
	/// </summary>
	public interface IAddrBook : IMapiProp
	{
		/// <summary>
		///  
		/// </summary>
		IBase OpenEntry (byte[] lpEntryID, NMapiGuid lpInterface, int flags);

		/// <summary>
		///  
		/// </summary>
		int CompareEntryIDs (byte[] lpEntryID1, byte[] lpEntryID2, int flags);

		/// <summary>
		///  
		/// </summary>
		int Advise (byte[] lpEntryID, int ulEventMask, IMapiAdviseSink lpAdviseSink);

		/// <summary>
		///  
		/// </summary>
		void Unadvise (int ulConnection);

		/// <summary>
		///  
		/// </summary>
		byte[] CreateOneOff (string name, string adrType, string address, int flags);

		/// <summary>
		///  
		/// </summary>
		byte[] NewEntry (int flags, byte[] lpEIDContainer, byte[] lpEIDNewEntryTpl); // first param "ULONG_PTR ulUIParam" removed!

		/// <summary>
		///  
		/// </summary>
		void ResolveName (int flags, string lpszNewEntryTitle, AdrList lpAdrList); // first param "ULONG_PTR ulUIParam" removed!

		/// <summary>
		///  
		/// </summary>
		byte[] GetPAB ();

		/// <summary>
		///  
		/// </summary>
		SRowSet GetSearchPath (int flags);

		/// <summary>
		///  
		/// </summary>
		void PrepareRecips (int flags, SPropTagArray propTagArray, AdrList recipList);	
	
	}

}