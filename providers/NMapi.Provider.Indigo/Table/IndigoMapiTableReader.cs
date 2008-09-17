//
// openmapi.org - NMapi C# Mapi API - IndigoMapiTableReader.cs
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

namespace NMapi.Provider.Indigo.Table {

	using System;
	using System.IO;
	using System.ServiceModel;

	using NMapi.Flags;
	using NMapi.Table;
	using NMapi.Properties;
	using NMapi.Properties.Special;

	public class IndigoMapiTableReader : IndigoBase, IMapiTableReader
	{
		internal IndigoMapiTableReader (IndigoMapiObjRef obj, IndigoBase parent) : 
			base (obj, parent.session)
		{
		}

		public SPropTagArray GetTags ()
		{
			try {
				return session.Proxy.IMapiTableReader_GetTags (obj);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
		public SRowSet GetRows (int cRows)
		{
			try {
				return session.Proxy.IMapiTableReader_GetRows (obj, cRows);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
	
	}

}
