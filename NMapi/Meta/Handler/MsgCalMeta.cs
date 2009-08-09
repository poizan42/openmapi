//
// openmapi.org - NMapi C# Mapi API - MsgCalMeta.cs
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
using System.Collections;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;

namespace NMapi.Meta {

	/// <summary>
	///  
	/// </summary>
	public class MsgCalMeta : IMetaHandler
	{
		public string GetSummary (IBase obj, 
			Dictionary<int, PropertyValue> someProps)
		{
			return null;
		}

		public Hashtable GetKeyValueSummary (IBase obj, 
			Dictionary<int, PropertyValue> someProps)
		{
			return null;
		}

		public MatchLevel GetScore (IBase obj, 
			Dictionary<int, PropertyValue> someProps)
		{
			return MatchLevel.NoMatch;
		}

	}

}
