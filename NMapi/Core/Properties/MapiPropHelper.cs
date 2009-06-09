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
	using CompactTeaSharp;
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
		public PropertyValue HrGetOneProp (int tag)
		{
			PropertyTag[] tags = PropertyTag.ArrayFromIntegers (tag);
			PropertyValue [] props = imapiProp.GetProps (tags, 0);

			ErrorProperty errProp = props [0] as ErrorProperty;
			if (errProp != null)
				throw new MapiException (errProp.Value);
			return props [0];
		}

		/// <summary>
		///  Same as <see cref="M:IMapiProp.HrGetOneProp()">HrGetOneProp</see>, 
		///  but returns null in the case of not found.
		/// </summary>
		/// <param name="tag">The property to get.</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public PropertyValue HrGetOnePropNull (int tag)
		{
			try {
				return HrGetOneProp (tag);
			} catch (MapiException e) {
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
		public void HrSetOneProp (PropertyValue prop)
		{
			PropertyValue [] props = new PropertyValue [] { prop };
			PropertyProblem[] problems = imapiProp.SetProps (props);
			if (problems.Length > 0)
				throw new MapiException (problems [0].SCode);
		}

		/// <summary>
		///  A helper to delete one property. 
		/// </summary>
		/// <param name="propTag">The property to delete.</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void HrDeleteOneProp (int propTag)
		{
			PropertyTag [] tags = PropertyTag.ArrayFromIntegers (propTag);
			var problems = imapiProp.DeleteProps (tags);
			if (problems.Length > 0)
				throw new MapiException (problems [0].SCode);
		}
	
		/// <summary>
		///  A helper to get one named property. If you have more than one named
		///  property please use <see cref="M:IMapi'Prop.GetIDsFromNames()">GetIDsFromNames</see>.
		///  Strings are returned as unicode.
		/// </summary>
		/// <param name="mnid">The MapiNameId structure describing the property.</param>
		/// <returns>The property value.</returns>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public PropertyValue HrGetNamedProp (MapiNameId mnid)
		{
			MapiNameId []  mnids = new MapiNameId [] { mnid };
			PropertyValue []  props = imapiProp.GetProps (
					imapiProp.GetIDsFromNames (mnids, Mapi.Create),
					Mapi.Unicode);			
			ErrorProperty errProp = props [0] as ErrorProperty;
			if (errProp != null)
				throw new MapiException (errProp.Value);
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
		public PropertyValue HrGetNamedProp (NMapiGuid guid, string name)
		{
			StringMapiNameId nmid = new StringMapiNameId (name);
			nmid.Guid = guid;
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
		public PropertyValue HrGetNamedProp (NMapiGuid guid, int id)
		{
			NumericMapiNameId nmid = new NumericMapiNameId (id);
			nmid.Guid = guid;
			return HrGetNamedProp (nmid);
		}

	}
}
