//
// openmapi.org - NMapi C# Mapi API - TxABSession.cs
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

namespace NMapi.Provider.TeamXChange.AddressBook {

	/// <summary>
	///
	/// </summary>
	public class TxABSession : IBase
	{
		
		
		
		public void Dispose ()
		{
			// TODO
		}
		
		public void Close ()
		{
			// TODO
		}
		
		
		public MapiError GetLastError (int hresult, int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}
		
		public void Logoff (int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}
		
		public IBase OpenEntry (byte[] entryID, NMapiGuid interFace, int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public int CompareEntryIDs (byte[] entryID1, byte[] entryID2, int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public int Advise (byte[] lpEntryID, NotificationEventType eventMask, IMapiAdviseSink sink)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public void Unadvise (int connection)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public IBase OpenStatusEntry (NMapiGuid interFace, int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}
		
		public void GetOneOffTable (int flags, IMapiTable table)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public void PrepareRecips (int flags, SPropTagArray propTagArray, AdrList recipList)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}
		
	}

}