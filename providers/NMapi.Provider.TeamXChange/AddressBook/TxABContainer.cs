//
// openmapi.org - NMapi C# Mapi API - TxABContainer.cs
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
using NMapi.Table;

namespace NMapi.Provider.TeamXChange.AddressBook {

	/// <summary>
	///
	/// </summary>
	public class TxABContainer : IMapiContainer
	{
		
		
		
		public void Dispose ()
		{
			// TODO
		}
		
		public void Close ()
		{
			// TODO
		}
		

		public IMapiProp CreateEntry (byte[] entryID, int createFlags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public void CopyEntries (EntryList entries, int UIParam, IMapiProgress progress, int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}
		

		public void DeleteEntries (EntryList entries, int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}
		

		public void ResolveNames (SPropTagArray propTagArray, int flags, AdrList adrList, int[] flagList)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}





		// IMapiContainer
		
		public IMapiTable GetContentsTable (int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public IMapiTableReader GetHierarchyTable(int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public IBase OpenEntry (byte [] entryID)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public IBase OpenEntry (
			byte [] entryID, NMapiGuid interFace,int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public void SetSearchCriteria (SRestriction restriction,
			EntryList containerList, int searchFlags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public GetSearchCriteriaResult GetSearchCriteria (int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		
		// IMapiProp
		
		public MapiError GetLastError (int hresult, int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public void SaveChanges (int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public SPropValue [] GetProps (SPropTagArray propTagArray, int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public SPropTagArray GetPropList (int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public IBase OpenProperty (int propTag)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public IBase OpenProperty (int propTag, NMapiGuid interFace,
			int interfaceOptions, int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public SPropProblemArray SetProps (SPropValue[] propArray)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public SPropProblemArray DeleteProps (SPropTagArray propTagArray)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public GetNamesFromIDsResult GetNamesFromIDs (
			SPropTagArray propTags, NMapiGuid propSetGuid, int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public SPropTagArray GetIDsFromNames (MapiNameId [] propNames, int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

	}

}