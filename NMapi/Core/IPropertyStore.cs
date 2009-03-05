//
// openmapi.org - NMapi C# Mapi API - IPropertyStore.cs
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

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi {

	public interface IPropertyStore
	{
		void DeleteProperty (SBinary mapiObject, int propTag, PropertyValue value);
		void SetProperty (SBinary mapiObject, int propTag, PropertyValue value);
		void SetProperties (SBinary mapiObject, int[] propTag, PropertyValue[] values);
		void GetProperty (SBinary mapiObject, int propTag);
		Dictionary<int, PropertyValue> GetProperties (SBinary mapiObject, int[] propTag);
		Dictionary<int, PropertyValue> GetAllProperties (SBinary mapiObject);
	}

}
