//
// openmapi.org - NMapi C# Mapi API - IABContainer.cs
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
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.AddressBook {

	/// <summary>
	///
	/// </summary>
	public interface IABContainer : IMapiContainer
	{
		/// <summary>
		///  
		/// </summary>
		IMapiProp CreateEntry (byte[] entryID, int createFlags);
		
		/// <summary>
		///  
		/// </summary>
		void CopyEntries (EntryList entries, int UIParam, IMapiProgress progress, int flags);
		
		/// <summary>
		///  
		/// </summary>
		void DeleteEntries (EntryList entries, int flags);
		
		/// <summary>
		///  
		/// </summary>
		void ResolveNames (SPropTagArray propTagArray, int flags, AdrList adrList, int[] flagList);

	}

}