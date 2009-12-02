//
// openmapi.org - NMapi C# Mapi API - IMapiPropExtender.cs
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

namespace NMapi {

	/*
	 *  The purpose of these extension methods is to explore how to improve on
	 *  top of the classic MAPI API. These methods might become the core API
	 *  later and we might want to remove the old/backend interface methods,
	 *  moving them one layer down.
	 *
	 *  Suggestions are welcome, the API may be changed.
	 *
	 *  Goals:
	 *   - eliminate flags
	 *   - only support unicode. ANSI is not supported!
	 *   - Arrays as "params"
	 *   - more readable method names.
	 *   - add helper methods for working with sinle properties etc.
	 *   - merge helper methods
	 *   - add new useful methods.
	 *   - make more robust.
	 *
	 */

	/// <summary>
	///  Provides some extension methods on IMapiProp.
	/// </summary>
	public static class IMapiPropExtender
	{

		/// <summary>
		///  Get a set of properties for the current object.
		/// In case of string properties the data is returned as unicode.
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="tags">Property tags of the requested properties.</param>
		/// <returns>An array of property values, which correspond to the requested tags.</returns>
		/// <exception cref="MapiInvalidParameterException">An empty tag array or null has been passed.</exception>
		public static PropertyValue[] GetProperties (this IMapiProp prop, params PropertyTag[] tags)
		{
			return prop.GetProps (tags, Mapi.Unicode);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="tag"></param>
		/// <returns>The property value of the requested tag.</returns>
		/// <exception cref="MapiInvalidParameterException">An empty tag array or null has been passed.</exception>
		public static PropertyValue GetProperty (this IMapiProp prop, PropertyTag tag)
		{
			return prop.GetProperties (tag) [0];
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="tag"></param>
		/// <returns></returns>
		/// <exception cref="MapiInvalidParameterException">An empty tag array or null has been passed.</exception>
		public static PropertyValue GetPropertyOrNull (this IMapiProp prop, PropertyTag tag)
		{
			PropertyValue value = prop.GetProperty (tag);
			if (value is ErrorProperty)
				return null;
			return value;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static PropertyProblem[] SetProperties (this IMapiProp prop, params PropertyValue[] values)
		{
			return prop.SetProps (values);
		}

		/// <summary>
		///  Sets a single property on the object.
		///  If the operation succeeds null is returned. Otherwise an object of
		///  type PropertyProblem is returned, that described the problem.
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="value">An object that identifies the property and also contains the value to be set.</param>
		/// <returns>
		///  If the property has been set successfully, null is returned.
		///  Otherwise a PropertyProblem object is returned that contains
		///  additional information on the problem that occured.
		/// </returns>
		public static PropertyProblem SetProperty (this IMapiProp prop, PropertyValue value)
		{
			PropertyProblem[] problems = prop.SetProperties (value);
			return GetFirstProblemOrNull (problems);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="tags"></param>
		/// <returns></returns>
		public static PropertyProblem[] DeleteProperties (this IMapiProp prop, params PropertyTag[] tags)
		{
			return prop.DeleteProps (tags);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static PropertyProblem DeleteProperty (this IMapiProp prop, PropertyTag tag)
		{
			PropertyProblem[] problems = prop.DeleteProperties (tag);
			return GetFirstProblemOrNull (problems);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="mnid"></param>
		/// <returns></returns>
		public static PropertyValue GetNamedProperty (this IMapiProp prop, MapiNameId mnid)
		{
			// TODO!
			throw new NotImplementedException ("Not yet impleemnted!");
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="guid"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static PropertyValue GetNamedProperty (this IMapiProp prop, NMapiGuid guid, string name)
		{
			// TODO!
			throw new NotImplementedException ("Not yet impleemnted!");
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="guid"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static PropertyValue GetNamedProperty (this IMapiProp prop, NMapiGuid guid, int id)
		{
			// TODO!
			throw new NotImplementedException ("Not yet impleemnted!");
		}


		/*

		KEEP_OPEN_READONLY
		Changes should be committed and the object should be kept open for reading. No additional changes will be made.

		MAPI_DEFERRED_ERRORS
		Allows SaveChanges to return successfully, possibly before the changes have been fully committed.

		*/


		/// <summary>
		///  Saves the changes while closing the object for write access. (TODO: correct?)
		/// </summary>
		/// <exception cref="MapiNoAccessException"></exception>
		/// <exception cref="MapiObjectChangedException"></exception>
		/// <exception cref="MapiObjectDeletedException"></exception>
		public static void SaveChanges (this IMapiProp prop)
		{
			prop.SaveChanges (0);
		}


		/// <summary>
		///
		/// </summary>
		/// <exception cref="MapiNoAccessException"></exception>
		/// <exception cref="MapiObjectChangedException"></exception>
		/// <exception cref="MapiObjectDeletedException"></exception>
		public static void SaveChanges (this IMapiProp prop, bool keepOpenWriteable)
		{
			int flags = (keepOpenWriteable) ? NMAPI.KEEP_OPEN_READWRITE : 0;
			prop.SaveChanges (flags);
		}


		/// <summary>
		///
		/// </summary>
		/// <exception cref="MapiNoAccessException"></exception>
		/// <exception cref="MapiObjectChangedException"></exception>
		/// <exception cref="MapiObjectDeletedException"></exception>
		public static void ForceSaveChanges (this IMapiProp prop)
		{
			prop.SaveChanges (NMAPI.FORCE_SAVE);
		}

		/// <summary>
		///  Returns a list of properties that exist on the object;
		///  String properties are returned as UnicodeProperty objects.
		/// </summary>
		/// <exception cref="MapiBadCharWidthException">Thrown if the OpenMapi-Provider does not support Unicode.</exception>
		public static PropertyTag[] GetPropertyList (this IMapiProp prop)
		{
			return prop.GetPropList (Mapi.Unicode);
		}

		/// <summary>
		///  Open a property as an object for readonly access.
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="tag">The property tag of the property to be opened.</param>
		/// <exception cref="MapiInterfaceNotSupportedException"></exception>
		/// <exception cref="MapiNoAccessException"></exception>
		/// <exception cref="MapiNoSupportException"></exception>
		/// <exception cref="MapiNotFoundException"></exception>
		/// <exception cref="MapiInvalidParameterException"></exception>
		public static IBase OpenProperty (this IMapiProp prop, PropertyTag tag)
		{
			return prop.OpenProperty (tag, AccessMode.Read);
		}

		/// <summary>
		///  Open a property as an object using the specified access permissions.
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="tag">The property tag of the property to be opened.</param>
		/// <param name="mode"></param>
		/// <exception cref="MapiInterfaceNotSupportedException"></exception>
		/// <exception cref="MapiNoAccessException"></exception>
		/// <exception cref="MapiNoSupportException"></exception>
		/// <exception cref="MapiNotFoundException"></exception>
		/// <exception cref="MapiInvalidParameterException"></exception>
		public static IBase OpenProperty (this IMapiProp prop, PropertyTag tag, AccessMode mode)
		{
			return prop.OpenProperty (tag, mode, false);
		}

		/// <summary>
		///  Open a property as an object using the specified access permissions.
		///  If "create" is set to true, create the property if it does not exist
		///  (and update it if it does).
		/// </summary>
		/// <param name="prop">A IMapiProp object.</param>
		/// <param name="tag">The property tag of the property to be opened.</param>
		/// <param name="mode"></param>
		/// <param name="create"></param>
		/// <exception cref="MapiInterfaceNotSupportedException"></exception>
		/// <exception cref="MapiNoAccessException"></exception>
		/// <exception cref="MapiNoSupportException"></exception>
		/// <exception cref="MapiNotFoundException"></exception>
		/// <exception cref="MapiInvalidParameterException"></exception>
		public static IBase OpenProperty (this IMapiProp prop, PropertyTag tag, AccessMode mode, bool create)
		{
			int flags = ((create) ? Mapi.Create : 0) |
						((create || mode == AccessMode.ReadWrite) ? Mapi.Modify : 0);
			return prop.OpenProperty (tag.Tag, null, 0, flags);
		}


		// TODO: the same stuff with the "interface" parameter ...




/*
		MAPI_DEFERRED_ERRORS
*/

		/// <summary>
		///  This enumeration is used by the OpenMapi API only. It is not designed to be
		///  passed to lower-level MAPI layers.
		/// </summary>
		public enum AccessMode
		{
			Read,
			ReadWrite
		}




		private static PropertyProblem GetFirstProblemOrNull (PropertyProblem[] problems)
		{
			if (problems == null || problems.Length == 0)
				return null;
			return problems [0];
		}

		public static T Get <T> (this IMapiProp message, MapiNameId name) where T : PropertyValue
		{
			var tags = message.GetPropList (Mapi.Unicode);
			var props = message.GetProps (tags, 0);

			var names = new MapiNameId [] {
				name
			};

			var id = message.GetIDsFromNames (names, Mapi.Create) [0];
			int index = PropertyValue.GetArrayIndex (props, id.Tag);
			return PropertyValue.GetArrayProp (props, index) as T;
		}

		public static T Get <T> (this IMapiProp message, int id, NMapiGuid guid) where T : PropertyValue
		{
			var name = new NumericMapiNameId (id);
			name.Guid = guid;
			return message.Get <T> (name);
		}

		public static T Get <T> (this IMapiProp message, string id, NMapiGuid guid) where T : PropertyValue
		{
			var name = new StringMapiNameId (id);
			name.Guid = guid;
			return message.Get <T> (name);
		}

		public static PropertyProblem Set <T, U, V> (this IMapiProp message, MapiNameId name, PropertyType type, V val) where T : PropertyValue, IPropertyValue <V>, new () where U : PropertyTag
		{
			var prop = new T ();

			var names = new MapiNameId [] {
				name
			};

			var id = message.GetIDsFromNames (names, Mapi.Create) [0];
			var tag = id.AsType (type) as U;
			prop.PropTag = tag.Tag;
			prop.Value = val;
			return message.SetProperty (prop);
		}

		public static PropertyProblem Set <T, U, V> (this IMapiProp message, int id, NMapiGuid guid, PropertyType type, V val) where T : PropertyValue, IPropertyValue <V>, new () where U : PropertyTag
		{
			var name = new NumericMapiNameId (id);
			name.Guid = guid;
			return message.Set <T, U, V> (name, type, val);
		}

		public static PropertyProblem Set <T, U, V> (this IMapiProp message, string id, NMapiGuid guid, PropertyType type, V val) where T : PropertyValue, IPropertyValue <V>, new () where U : PropertyTag
		{
			var name = new StringMapiNameId (id);
			name.Guid = guid;
			return message.Set <T, U, V> (name, type, val);
		}
	}
}

// vi:set noexpandtab:
