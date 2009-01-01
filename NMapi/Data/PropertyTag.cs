//
// openmapi.org - NMapi C# Mapi API - PropertyTag.cs
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

namespace NMapi {

	using System;
	using NMapi.Flags;

	/// <summary>
	///  Wraps the property tag integer for stronger typing
	//   and easier access to Id and Type of the tag.
	/// </summary>
	public struct PropertyTag
	{
		private int propTag;

		public PropertyType Type {
			get {
				return PropertyTypeHelper.PROP_TYPE (propTag);
			}
		}

		public int Id {
			get {
				return PropertyTypeHelper.PROP_ID (propTag);
			}
		}

		public int Tag {
			get { return propTag; }
		}

		public PropertyTag (int pt)
		{
			propTag = pt;
		}
	}

}
