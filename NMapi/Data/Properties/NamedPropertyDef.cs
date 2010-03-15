//
// openmapi.org - NMapi C# Mapi API - NamedPropertyDef.cs
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

namespace NMapi {

	using System;
	using System.Text;
	using NMapi.Flags;
	using NMapi.Properties;

	/// <summary>
	///  Represents a definition of a named property that allows 
	///  typed access the associated property tag and, therefore also 
	///  to the property value.
	/// </summary>
	/// <example>
	///  var namedProp = NamedProperty.Blub;
	///  NamePropertyResolver resolver = mapiObject.ResolveNameProperties (namedProp);
	///  UnicodePropertyTag propTag = (StringPropertyTag) namedProp.CreateTag (resolver);
	///  UnicodeProperty uniProp = propTag.CreateValue ();
	/// </example>
	public abstract partial class NamedPropertyDef
	{
		private MapiNameId nameDefinition;

		/// <summary>
		///  Returns the (classic MAPI) named property definition.
		///  This is bascially the GUID + id or string-name and does not 
		///  include the information about the type of the property.
		/// </summary>
		public MapiNameId NameDefinition {
			get { return nameDefinition; }
		}

		/// <summary>
		///  Returns the Property Type associated with the named property.		
		/// </summary>		
		public abstract PropertyType Type { get; }

		/// <summary>
		///  
		/// </summary>
		/// <param name="nameid"></param>
		protected NamedPropertyDef (MapiNameId nameid)
		{
			this.nameDefinition = nameid;
		}
		
	}


}
