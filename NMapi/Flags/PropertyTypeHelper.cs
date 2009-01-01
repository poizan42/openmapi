//
// openmapi.org - NMapi C# Mapi API - PropertyType.cs
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

using System.IO;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	public class PropertyTypeHelper
	{
		public const int PROP_TYPE_MASK  = 0x0000FFFF;
		public const int PROP_ID_NULL    = 0;
		public const int PROP_ID_INVALID = 0xFFFF;

		public static PropertyType PROP_TYPE (int propTag)
		{
			return (PropertyType) (propTag & PROP_TYPE_MASK);
		}

		public static int PROP_TAG (PropertyType propType, int propID)
		{
			return (propID << 16) | ((int) propType);
		}	

		public static int PROP_ID (int propTag)
		{
			return propTag >> 16;
		}

		public int CHANGE_PROP_TYPE (int propTag, int propType)
		{
			return (int) (0xFFFF0000 & propTag) | propType;
		}

	}

}
