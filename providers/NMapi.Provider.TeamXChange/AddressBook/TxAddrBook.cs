//
// openmapi.org - NMapi C# Mapi API - TxAddrBook.cs
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
	public class TxAddrBook : IMapiProp
	{
		
		
		public void Dispose ()
		{
			// TODO
		}
		
		public void Close ()
		{
			// TODO
		}
		

/*



			public ABUSERDATA ABGetUserData(byte [] eid) throws MAPIException {
			public ABUSERDATA ABGetUserDataBySmtpAddress(String adr) throws MAPIException {
			public ABUSERDATA ABGetUserDataByInternalAddress(String adr) throws MAPIException {

			private byte [] makeSearchKey(String prefix, String address)

			public ADDRESS ResolveEntryID(byte [] eid)
			public ADDRESS ResolveSmtpAddress(String smtpaddress, String displayname)



			public class ADDRESS {

				public byte [] eid;
				public String  displayName;
				public String  internalAddress;
				public String  addrType;

				public String  smtpAddress;
				public byte [] searchKey;
			}




			struct ABUSERDATA
			{
			        LPWSTR pwszId;
			        LPWSTR pwszDisplay;
			        LPWSTR pwszAdrType;
			        LPWSTR pwszSmtpAdr;
			        LPWSTR pwszIntAdr;
			        SBinary eid;
			        SBinary searchKey;
			};

			typedef ABUSERDATA *LPABUSERDATA;

			struct ABUSERLIST
			{
			        ABUSERDATA *pData;
			        ABUSERLIST *pNext;
			};

			typedef ABUSERLIST *LPABUSERLIST;


*/




		public IBase OpenEntry (byte[] lpEntryID, NMapiGuid lpInterface, int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public int CompareEntryIDs (byte[] lpEntryID1, byte[] lpEntryID2, int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public int Advise (byte[] lpEntryID, int ulEventMask, IMapiAdviseSink lpAdviseSink)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public void Unadvise (int ulConnection)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public byte[] CreateOneOff (string name, string adrType, string address, int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public byte[] NewEntry (int flags, byte[] lpEIDContainer, byte[] lpEIDNewEntryTpl)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public void ResolveName (int flags, string lpszNewEntryTitle, AdrList lpAdrList)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public byte[] GetPAB ()
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public RowSet GetSearchPath (int flags)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
		}

		public void PrepareRecips (int flags, PropertyTag[] propTagArray, AdrList recipList)
		{
			throw new NotImplementedException ("Not implemented by the TXC provider.");
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

		public PropertyValue [] GetProps (PropertyTag[] propTagArray, int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public PropertyTag[] GetPropList (int flags)
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

		public PropertyProblem[] SetProps (PropertyValue[] propArray)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public PropertyProblem[] DeleteProps (PropertyTag[] propTagArray)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public GetNamesFromIDsResult GetNamesFromIDs (
			PropertyTag[] propTags, NMapiGuid propSetGuid, int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}

		public PropertyTag[] GetIDsFromNames (MapiNameId [] propNames, int flags)
		{
			throw new NotSupportedException ("Not supported by the TXC provider.");
		}


	}

}