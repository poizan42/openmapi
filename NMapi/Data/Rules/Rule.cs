//
// openmapi.org - NMapi C# Mapi API - Rule.cs
//
// Copyright 2010 Topalis AG
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
using System.Text;
using System.IO;
using System.Runtime.Serialization;

using System.Diagnostics;
using CompactTeaSharp;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi {
	
	/// <summary></summary>
	public sealed class Rule
	{
		/// <summary></summary>
		public long Id { get; set; }
		
		/// <summary></summary>
		public int Sequence { get; set; }
		
		/// <summary></summary>
		public int State { get; set; }
		
		/// <summary></summary>
		public int UserFlags { get; set; }

		/// <summary></summary>
		public Restriction Condition { get; set; }

		/// <summary></summary>
		public Actions Actions { get; set; }

		/// <summary></summary>
		public string Provider { get; set; }

		/// <summary></summary>
		public string Name { get; set; }
		
		/// <summary></summary>
		public int Level { get; set; }
		
		/// <summary></summary>
		public byte[] ProviderData { get; set; }

		/// <summary></summary>
		/// <returns></returns>
		public PropertyValue[] ToPropertyArray ()
		{
			PropertyValue[] props = new PropertyValue [] {
				ExchangeProperty.Typed.RuleId.CreateValue (Id),
				ExchangeProperty.Typed.RuleSequence.CreateValue (Sequence),
				ExchangeProperty.Typed.RuleState.CreateValue (State),
				ExchangeProperty.Typed.RuleUserFlags.CreateValue (UserFlags),
				ExchangeProperty.Typed.RuleCondition.CreateValue (Condition),
				ExchangeProperty.Typed.RuleActions.CreateValue (Actions),
				ExchangeProperty.Typed.RuleProvider.CreateValue (Provider),
				ExchangeProperty.Typed.RuleName.CreateValue (Name),
				ExchangeProperty.Typed.RuleLevel.CreateValue (Level),
				ExchangeProperty.Typed.RuleProviderData.CreateValue (new SBinary (ProviderData))
			};
			return props;
		}
		
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="values"></param>
		public static Rule FromPropertyArray (PropertyValue[] values)
		{
			if (values == null)
				return null;
			
			Rule rule = new Rule ();

			foreach (var prop in values) {
				switch (prop.PropTag) {
					case ExchangeProperty.RuleId: /* TODO */ break;
					case ExchangeProperty.RuleSequence:/* TODO */ break;
					case ExchangeProperty.RuleState: /* TODO */ break;
					case ExchangeProperty.RuleUserFlags: /* TODO */ break;
					case ExchangeProperty.RuleCondition: /* TODO */ break;
					case ExchangeProperty.RuleActions: /* TODO */ break;
					case ExchangeProperty.RuleProvider: /* TODO */ break;
					case ExchangeProperty.RuleName: /* TODO */ break;
					case ExchangeProperty.RuleLevel: /* TODO */ break;
					case ExchangeProperty.RuleProviderData: /* TODO */ break;
				}	
			}

			// TODO: ensure that we got exactly the expected arguments / no duplicates ...
			
			return rule;
		}
		
		public override string ToString ()
		{
			// TODO
			return base.ToString ();
		}
		

	}	

}