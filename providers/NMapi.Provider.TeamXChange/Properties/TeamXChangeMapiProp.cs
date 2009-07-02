//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMapiProp.cs
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

namespace NMapi.Properties {

	using System;
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;

	using NMapi.Flags;

	public class TeamXChangeMapiProp : TeamXChangeBase, IMapiProp
	{
		private MapiPropHelper helper;

		internal TeamXChangeMapiProp (long obj, TeamXChangeSession session) : base (obj, session)
		{
			this.helper = new MapiPropHelper (this);
		}

		public MapiError GetLastError (int hresult, int flags)
		{
			var arg = new MAPIProp_GetLastError_arg ();
			MAPIProp_GetLastError_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			try {
				res = clnt.MAPIProp_GetLastError_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);

			if ((flags & Mapi.Unicode) != 0) 
				return res.lpMapiErrorA.Value;
			else
				return res.lpMapiErrorW.Value;
		}

		public void SaveChanges (int flags)
		{
			var arg = new MAPIProp_SaveChanges_arg();
			MAPIProp_SaveChanges_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIProp_SaveChanges_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
		}

		public PropertyValue [] GetProps (PropertyTag[] propTagArray, int flags)
		{
			var arg = new MAPIProp_GetProps_arg();
			MAPIProp_GetProps_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpPropTagArray = new PropertyTagArrayPtrAdapter (propTagArray);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIProp_GetProps_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.props;
		}

		public PropertyTag[] GetPropList (int flags)
		{
			var arg = new MAPIProp_GetPropList_arg();
			MAPIProp_GetPropList_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIProp_GetPropList_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed  (res.hr))
				throw new MapiException (res.hr);
			return res.lpPropTagArray.Value;
		}

		public IBase OpenProperty (int propTag)
		{
			return OpenProperty (propTag, null, 0, 0);
		}

		public IBase OpenProperty (int propTag, NMapiGuid interFace,
			int interfaceOptions, int flags)
		{
			var arg = new MAPIProp_OpenProperty_arg(); 
			MAPIProp_OpenProperty_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulPropTag = propTag;
			arg.lpiid = new LPGuid (interFace);
			arg.ulInterfaceOptions = interfaceOptions;
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIProp_OpenProperty_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return session.CreateObject (this, res.obj.Value.Value, res.ulObjType, interFace, propTag);
		}

		public PropertyProblem[] SetProps (PropertyValue[] propArray)
		{
			var arg = new MAPIProp_SetProps_arg ();
			MAPIProp_SetProps_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.props = propArray;
			try {
				res = clnt.MAPIProp_SetProps_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException (e);
			}
			catch (OncRpcException e) {
				throw new MapiException (e);
			}
			// TODO: The next 2 lines have been commented-out, but this really is not 
			//       a correct fix. The problem here is, that warnings must be returned 
			//       in the hresult to the client but the PropertyProblem must still be 
			//       returned.
			
			//if (Error.CallHasFailed (res.hr))
			//	throw new MapiException (res.hr);
			return res.lpProblems.value;
		}

		public PropertyProblem[] DeleteProps (PropertyTag[] propTagArray)
		{
			var arg = new MAPIProp_DeleteProps_arg();
			MAPIProp_DeleteProps_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpPropTagArray = new PropertyTagArrayPtrAdapter (propTagArray);
			try {
				res = clnt.MAPIProp_DeleteProps_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.lpProblems.Value;
		}
	
		// NOT IMPLEMENTED: CopyTo ()
		// NOT IMPLEMENTED: CopyProps ()

		public GetNamesFromIDsResult GetNamesFromIDs (
			PropertyTag[] propTags, NMapiGuid propSetGuid, int flags)
		{
			var arg = new MAPIProp_GetNamesFromIDs_arg ();
			MAPIProp_GetNamesFromIDs_res res;
			GetNamesFromIDsResult ret = new GetNamesFromIDsResult ();
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpPropTags = new PropertyTagArrayPtrAdapter (propTags);
			arg.lpPropSetGuid = new LPGuid (propSetGuid);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIProp_GetNamesFromIDs_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException (e);
			}
			catch (OncRpcException e) {
				throw new MapiException (e);
			}
			if (Error.CallHasFailed  (res.hr))
				throw new MapiException (res.hr);

			ret.PropTags = res.lpPropTags.Value;
			ret.PropNames = new MapiNameId [res.names.Length];
			for (int idx = 0; idx < res.names.Length; idx++)
				ret.PropNames[idx] = res.names[idx].Value;
			return ret;
		}

		public PropertyTag[] GetIDsFromNames (MapiNameId [] propNames, int flags)
		{
			var arg = new MAPIProp_GetIDsFromNames_arg ();
			MAPIProp_GetIDsFromNames_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			arg.names = new LPMapiNameId [propNames.Length];
			for (int idx = 0; idx < propNames.Length; idx++)
				arg.names[idx] = new LPMapiNameId (propNames [idx]);
			try {
				res = clnt.MAPIProp_GetIDsFromNames_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException (e);
			}
			catch (OncRpcException e) {
				throw new MapiException (e);
			}
			if (Error.CallHasFailed  (res.hr))
				throw new MapiException (res.hr);
			return res.lpPropTags.Value;
		}

		public PropertyValue HrGetOneProp (int tag)
		{
			return helper.HrGetOneProp (tag);
		}

		public PropertyValue HrGetOnePropNull (int tag)
		{
			return helper.HrGetOnePropNull (tag);
		}

		public void HrSetOneProp (PropertyValue prop)
		{
			helper.HrSetOneProp (prop);
		}

		public void HrDeleteOneProp (int propTag)
		{
			helper.HrDeleteOneProp (propTag);
		}

		public PropertyValue HrGetNamedProp (MapiNameId mnid)
		{
			return helper.HrGetNamedProp (mnid);
		}

		public PropertyValue HrGetNamedProp (NMapiGuid guid, string name)
		{
			return helper.HrGetNamedProp (guid, name);
		}

		public PropertyValue HrGetNamedProp (NMapiGuid guid, int id)
		{
			return helper.HrGetNamedProp (guid, id);
		}

	}
}
