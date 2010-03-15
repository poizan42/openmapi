//
// openmapi.org - NMapi C# Mapi API - MapiFactoryAttribute.cs
//
// Copyright 2008-2009 Topalis AG
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

namespace NMapi {

	/// <summary>
	///  When applied to a class, indicates that the class implements 
	///  an OpenMapi/NMapi provider.
	/// </summary>
	/// <remarks>
	///  It also provides information like the 
	///  type and version of the provider.
	/// </remarks>
	public sealed class MapiFactoryAttribute : Attribute
	{
		private readonly string name;
		private readonly MapiProviderType type;
		private readonly string description;
		private readonly Version version;
		private readonly string license;
		private readonly string[] authors;

		/// <summary>
		///  A string containing the qualified name of the provider that is 
		///  used when an dynamic selection of a provider at runtime is required.
		/// </summary>
		public string Name {
			get { return name; }
		}

		/// <summary>
		///  Indicates the type of the provider.
		/// </summary>
		public MapiProviderType ProviderType {
			get { return type; }
		}

		/// <summary>
		///  A short description that can be displayed to users.
		/// </summary>
		public string Description {
			get { return description; }
		}

		/// <summary>
		///  The current version of the provider.
		/// </summary>
		public Version Version {
			get { return version; }
		}

		/// <summary>
		///  Can be used to specify the name of the license of the provider.
		/// </summary>
		public string License {
			get { return license; }
		}

		/// <summary>
		///  An array of strings, each containing the name of an author.
		/// </summary>
		public string[] Authors {
			get { return authors; }
		}

		/// <summary></summary>
		/// <remarks></remarks>
		/// param name=""></param>			
		public MapiFactoryAttribute (string name)// : this (name, 
//			MapiProviderType.Unknown, null, null, null, null)
		{
			this.name = name;
		}
	
		/// <summary></summary>
		/// <remarks></remarks>
		/// param name="name"></param>
		/// param name="type"></param>
		/// param name="description"></param>
		/// param name="version"></param>
		/// param name="license"></param>
		/// param name="authors"></param>
		public MapiFactoryAttribute (string name, MapiProviderType type, string description, 
			int[] version, string license, params string[] authors)
		{
			this.name = name;
			this.type = type;
			this.description = description;
/*			if (version == null)
				this.version = new Version (0, 0, 0, 0);
			else {
				switch (version.Length) {
					case 0: this.version = new Version (0, 0, 0, 0); break;
					case 1: this.version = new Version (version [0], 0); break;
					case 2: this.version = new Version (version [0], version [1]); break;
					case 3: this.version = new Version (version [0], version [1], version [2]); break;
					case 4:
					default: this.version = new Version (version [0], version [1], version [2], version [3]); break;
				}
			}*/
			this.license = license;
			this.authors = authors;
		}

	}

}
