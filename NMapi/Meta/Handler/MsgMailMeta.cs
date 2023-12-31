//
// openmapi.org - NMapi C# Mapi API - MsgMailMeta.cs
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
	public class MsgMailMeta : IMetaHandler
	{
		private string GetStringOrUnknown (PropertyValue property)
		{
			if (property.Tag.Type == PropertyType.Error)
				return "<unknown>";
			return "'" + (string) property + "'";
		}
		
		public string GetSummary (IBase obj, 
			Dictionary<int, PropertyValue> someProps)
		{
			IMapiProp prop = obj as IMapiProp;
			if (obj != null) {
				var tags = PropertyTag.ArrayFromIntegers (
						Property.MessageClass, 
						Property.SenderEmailAddress,
						Property.Subject);

				PropertyValue[] values = prop.GetProps (tags, 0); // read-only
				string msgClass = GetStringOrUnknown (values [0]);
				string from = GetStringOrUnknown (values [1]);
				string subject = GetStringOrUnknown (values [2]);

				return msgClass + " (From : " + from + ", Subject: " + subject + ")";
			}
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
			if (obj == null)
				return MatchLevel.NoMatch;

			if (someProps.ContainsKey (Property.MessageClass) && 
				((string) someProps [Property.MessageClass]).StartsWith (MessageClasses.Ipm.Note))
					return MatchLevel.Match;

			return MatchLevel.NoMatch;
		}

	}

}
