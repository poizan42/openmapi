//
// openmapi.org - NMapi C# Mapi API - MapiPropHelper.cs
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

namespace NMapi.Properties {

	using System;
	using System.IO;
	using RemoteTea.OncRpc;
	using NMapi.Interop;

	using NMapi.Flags;

	public class MapiPropHelper
	{
		private IMapiProp imapiProp;

		public MapiPropHelper (IMapiProp imapiProp)
		{
			this.imapiProp = imapiProp;
		}

		/// <summary>
		///   A helper to get one property.
		/// </summary>
		/// <param name="tag">The property to get.</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public SPropValue HrGetOneProp (int tag)
		{
			SPropTagArray tags = new SPropTagArray ();
			SPropValue [] props;
		
			tags.PropTagArray = new int [1];
			tags.PropTagArray [0] = tag;
			props = imapiProp.GetProps (tags, 0);
			if (PropertyTypeHelper.PROP_TYPE (props[0].PropTag) == PropertyType.Error)
				throw new MapiException (props[0].Value.err);
			return props[0];
		}

		/// <summary>
		///  Same as <see cref="M:IMapiProp.HrGetOneProp()">HrGetOneProp</see>, 
		///  but returns null in the case of not found.
		/// </summary>
		/// <param name="tag">The property to get.</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public SPropValue HrGetOnePropNull (int tag)
		{
			try {
				return HrGetOneProp (tag);
			}
			catch (MapiException e) {
				if (e.HResult == Error.NotFound)
					return null;
				throw;
			}
		}

		/// <summary>
		///  A helper to set one property. 
		/// </summary>
		/// <param name="prop">The property to set.</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void HrSetOneProp (SPropValue prop)
		{
			SPropValue [] props = new SPropValue [] {prop};
			SPropProblemArray problems = imapiProp.SetProps(props);
			if (problems.AProblem.Length > 0)
				throw new MapiException (problems.AProblem [0].SCode);
		}

		/// <summary>
		///  A helper to delete one property. 
		/// </summary>
		/// <param name="propTag">The property to delete.</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void HrDeleteOneProp (int propTag)
		{
			int [] tags = new int [] { propTag };
			var problems = imapiProp.DeleteProps (new SPropTagArray (tags));
			if (problems.AProblem.Length > 0)
				throw new MapiException (problems.AProblem[0].SCode);
		}
	
		/// <summary>
		///  A helper to get one named property. If you have more than one named
		///  property please use <see cref="M:IMapi'Prop.GetIDsFromNames()">GetIDsFromNames</see>.
		///  Strings are returned as unicode.
		/// </summary>
		/// <param name="mnid">The MapiNameId structure describing the property.</param>
		/// <returns>The property value.</returns>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public SPropValue HrGetNamedProp (MapiNameId mnid)
		{
			MapiNameId []  mnids = new MapiNameId [] { mnid };
			SPropValue []  props = imapiProp.GetProps (imapiProp.GetIDsFromNames (mnids, NMAPI.MAPI_CREATE),
				         Mapi.Unicode);
			if (PropertyTypeHelper.PROP_TYPE (props[0].PropTag) == PropertyType.Error)
				throw new MapiException (props[0].Value.err);
			return props[0];
		}

		/// <summary>
		///  A helper to get one named property. If you have more than one named 
		///  property please use <see cref="M:IMapi'Prop.GetIDsFromNames()">GetIDsFromNames</see>.
		///  Strings are returned as unicode.
		/// </summary>
		/// <param name="guid">The namespace/guid</param>
		/// <param name="name">The name</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public SPropValue HrGetNamedProp (NMapiGuid guid, string name)
		{
			MapiNameId nmid = new MapiNameId ();
			nmid.UlKind = MnId.String;
			nmid.Guid = guid;
			nmid.UKind.StrName = name;
			return HrGetNamedProp (nmid);
		}

		/// <summary>
		///  A helper to get one named property. If you have more than one named 
		///  property please use <see cref="M:IMapi'Prop.GetIDsFromNames()">GetIDsFromNames</see>.
		///  Strings are returned as unicode.
		/// </summary>
		/// <param name="guid">The namespace/guid</param>
		/// <param name="id">The identifier</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public SPropValue HrGetNamedProp (NMapiGuid guid, int id)
		{
			MapiNameId nmid = new MapiNameId ();
			nmid.UlKind = MnId.Id;
			nmid.Guid = guid;
			nmid.UKind.ID = id;
			return HrGetNamedProp (nmid);
		}

	}
}
