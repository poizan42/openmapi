//
// openmapi.org - NMapi C# Mapi API - MetaManager.cs
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

using System.ServiceModel;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.Meta {

	/// <summary>
	///  
	/// </summary>
	public sealed class MetaManager
	{
		private List<IMetaHandler> handlers = new List<IMetaHandler> ();

		public MetaManager ()
		{
			RegisterMeta (new MsgCalMeta ());
			RegisterMeta (new MsgMailMeta ());
			RegisterMeta (new MsgTaskMeta ());
			RegisterMeta (new MsgUnknownMeta ());
			RegisterMeta (new PropMeta ());
		}

		public string GetSummary (IBase obj)
		{
			var someProps = new Dictionary<int, SPropValue> ();

			if (obj is IMapiProp) {
				var propTags = new SPropTagArray (
					Property.EntryId, Property.MessageClass);

				SPropValue[] values = ((IMapiProp) obj).GetProps (propTags, Mapi.Unicode);
				int i = 0;
				foreach (var val in propTags.PropTagArray)
					someProps [val] = values [i++]; // TODO: all values???
			}

			MatchLevel current = MatchLevel.NoMatch;
			IMetaHandler bestHandler = null;

			foreach (var handler in handlers) {
				MatchLevel level = handler.GetScore (obj, someProps);
				if (level > current) {
					current = level;
					bestHandler = handler;
				}
			}
			return bestHandler.GetSummary (obj, someProps);
		}

		public void RegisterMeta (IMetaHandler handler)
		{
			if (!handlers.Contains (handler))
				handlers.Add (handler);
		}
	}

}
