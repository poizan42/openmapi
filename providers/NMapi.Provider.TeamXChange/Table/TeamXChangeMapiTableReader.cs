//
// openmapi.org - NMapi C# Mapi API - TeamXChangeTableReader.cs
//
// Copyright 2008 VipCom GmbH, Topalis AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
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

namespace NMapi.Table {

	using System;
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop.MapiRPC;

	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Table;


	public class TeamXChangeMapiTableReader : TeamXChangeBase, IMapiTableReader
	{
		internal TeamXChangeMapiTableReader (long obj, TeamXChangeBase parent) : 
			base (obj, parent.session)
		{
		}

		public SPropTagArray GetTags ()
		{
			var prms = new TblData_GetTags_arg ();
			prms.obj = new HObject (obj);
			var res = MakeCall<TblData_GetTags_res, 
				TblData_GetTags_arg> (clnt.TblData_GetTags_1, prms);
			return res.pTags.Value;
		}

		public SRowSet GetRows (int cRows)
		{
			var prms = new TblData_GetRows_arg ();
			prms.obj = new HObject (obj);
			prms.cRows = cRows;

			var res = MakeCall<TblData_GetRows_res, 
				TblData_GetRows_arg> (clnt.TblData_GetRows_1, prms);
			return res.pRows.Value;
		}
	
	}

}
