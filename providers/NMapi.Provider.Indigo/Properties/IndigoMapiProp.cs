//
// openmapi.org - NMapi C# Mapi API - IndigoMapiProp.cs
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

namespace NMapi.Provider.Indigo.Properties {

	using System;
	using System.IO;
	using System.ServiceModel;
	using NMapi.Properties;

	using System.ServiceModel;

	using NMapi.Flags;

	public class IndigoMapiProp : IndigoBase, IMapiProp
	{
		internal IndigoMapiProp (IndigoMapiObjRef obj, IndigoMapiSession session) : base (obj, session)
		{
		}

		public MapiError GetLastError (int hresult, int flags)
		{
			try {
				return session.Proxy.IMapiProp_GetLastError (obj, hresult, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void SaveChanges (int flags)
		{
			try {
				session.Proxy.IMapiProp_SaveChanges (obj, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
	
		public SPropValue [] GetProps (SPropTagArray propTagArray, int flags)
		{
			try {
				return session.Proxy.IMapiProp_GetProps (obj, propTagArray, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public SPropTagArray GetPropList (int flags)
		{
			try {
				return session.Proxy.IMapiProp_GetPropList (obj, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public IBase OpenProperty (int propTag)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMapiProp_OpenProperty (obj, propTag);
				return session.CreateObject (this, objRef, null, propTag);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public IBase OpenProperty (int propTag, NMapiGuid interFace,
			int interfaceOptions, int flags)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMapiProp_OpenProperty_4 (obj, propTag, interFace, interfaceOptions, flags);
				return session.CreateObject (this, objRef, interFace, propTag);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public SPropProblemArray SetProps (SPropValue[] propArray)
		{
			try {
				return session.Proxy.IMapiProp_SetProps (obj, propArray);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public SPropProblemArray DeleteProps (SPropTagArray propTagArray)
		{
			try {
				return session.Proxy.IMapiProp_DeleteProps (obj, propTagArray);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public GetNamesFromIDsResult GetNamesFromIDs (
			SPropTagArray propTags, NMapiGuid propSetGuid, int flags)
		{
			try {
				return session.Proxy.IMapiProp_GetNamesFromIDs (obj, propTags, propSetGuid, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public SPropTagArray GetIDsFromNames (MapiNameId [] propNames, int flags)
		{
			try {
				return session.Proxy.IMapiProp_GetIDsFromNames (obj, propNames, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
		
	}
}
