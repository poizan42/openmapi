//
// openmapi.org - NMapi C# Mapi API - TeamXChangeTableReader.cs
//
// Copyright 2008 VipCom AG
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
	using RemoteTea.OncRpc;
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
			TblData_GetTags_arg arg = new TblData_GetTags_arg();
			TblData_GetTags_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			try {
				res = clnt.TblData_GetTags_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return res.pTags.Value;
		}
		public SRowSet GetRows (int cRows)
		{
			TblData_GetRows_arg arg = new TblData_GetRows_arg();
			TblData_GetRows_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.cRows = cRows;
			try {
				res = clnt.TblData_GetRows_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return res.pRows.Value;
		}
	
	}

}
