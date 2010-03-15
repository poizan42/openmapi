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

namespace NMapi.Rules {
	
	/// <summary>Represents a server-side rule.</summary>
	/// <remarks>
	///  <para>
	///   This class provides convenient access to the data of the rows of 
	///   a Rule-Table. 
	///  </para>
	///  <para>
	///   TODO: describe !!!
	///  </para>
	///  <para>
	///   This class is NOT part of the core MAPI API and, therefore also 
	///   NOT directly used in any network protocol.
	/// </para>
	/// </remarks>
	public sealed class Rule : ICloneable
	{
		/// <summary> TODO </summary>
		/// <value>The id of the rule, as seen in the Rule-Table.</value>
		public long Id { get; set; }
		
		/// <summary> TODO </summary>
		/// <value>The sequence data of the rule, as seen in the Rule-Table.</value>
		public int Sequence { get; set; }
		
		/// <summary> TODO </summary>
		/// <value>The state of the rule, as seen in the Rule-Table.</value>
		public int State { get; set; }
		
		/// <summary> TODO </summary>
		/// <value>The client-specific flags of the rule, as seen in the Rule-Table.</value>
		public int UserFlags { get; set; }

		/// <summary> TODO </summary>
		/// <value>The filter/restriction/condition of the rule, as seen in the Rule-Table.</value>
		public Restriction Condition { get; set; }

		/// <summary> TODO </summary>
		/// <value>The set of actions of the rule, as seen in the Rule-Table.</value>
		public Actions Actions { get; set; }

		/// <summary> TODO </summary>
		/// <value>The provider of the rule, as seen in the Rule-Table.</value>
		public string Provider { get; set; }

		/// <summary> TODO </summary>
		/// <value>The name of the rule, as seen in the Rule-Table.</value>
		public string Name { get; set; }
		
		/// <summary> TODO </summary>
		/// <value>The level of the rule, as seen in the Rule-Table.</value>
		public int Level { get; set; }
		
		/// <summary> TODO </summary>
		/// <value>The provider data of the rule, as seen in the Rule-Table.</value>
		public byte[] ProviderData { get; set; }
		
		
		/// <summary>Creates an uninitialized rule.</summary>
		public Rule ()
		{
		}

		/// <summary>Creates an array of properties that contain the data of the rule.</summary>
		/// <remarks>
		/// <para>
		///  The properties returned are exactly the properties found in the 
		///  row of a Rule-Table and may be used together with the interface to 
		///  modify that table.
		/// </para>
		/// <para>
		///  For a list of the properties present, <see cref="M:FromPropertyArray" />.
		/// </para>
		/// </remarks>
		/// <returns>An array of MAPI properties.</returns>
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
		
		/// <summary>Constructs a new Rule from an array of properties.</summary>
		/// <remarks>
		///  <para>
		///    For this to work the certain properties need to be present 
		///    exactly once. Other properties may be present but are ignored and 
		///    not made part of the rule. If you pass in the data that you obtained 
		///    from a rule table, these properties WILL be present.
		///  </para>
		///  <para>
		///   The required properties are:
		///  
		///  <list type="table">
		///   <listheader>
		///     <term>Required Properties</term>
		///     <description>The properties that are expected exactly once.</description>
		///   </listheader>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleId</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleId" />. For information 
		///      on how the property is used in this class, <see cref="M:Id">.
		///     </description>
		///   </item>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleSequence</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleSequence" />. For information 
		///      on how the property is used in this class, <see cref="M:Sequence">.
		///     </description>
		///   </item>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleState</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleId" />. For information 
		///      on how the property is used in this class, <see cref="M:State">.
		///     </description>
		///   </item>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleUserFlags</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleUserFlags" />. For information 
		///      on how the property is used in this class, <see cref="M:UserFlags">.
		///     </description>
		///   </item>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleCondition</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleCondition" />. For information 
		///      on how the property is used in this class, <see cref="M:Condition">.
		///     </description>
		///   </item>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleActions</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleActions" />. For information 
		///      on how the property is used in this class, <see cref="M:Actions">.
		///     </description>
		///   </item>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleProvider</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleProvider" />. For information 
		///      on how the property is used in this class, <see cref="M:Provider">.
		///     </description>
		///   </item>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleName</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleName" />. For information 
		///      on how the property is used in this class, <see cref="M:Name">.
		///     </description>
		///   </item>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleLevel</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleLevel" />. For information 
		///      on how the property is used in this class, <see cref="M:Level">.
		///     </description>
		///   </item>
		///   <item>
		///     <term>ExchangeProperty.Typed.RuleProviderData</term>
		///     <description>
		///      <see cref="T:ExchangeProperty.Typed.RuleProviderData" />. For information 
		///      on how the property is used in this class, <see cref="M:ProviderData">.
		///     </description>
		///   </item>
		///  </list>
		///  </para>
		/// </remarks>
		/// <param name="values">
		///  An array of properties that must contain the set of required 
		///  properties, mentioned in the remarks. 
		/// </param>
		/// <returns>
		///   A class representing the rule. If the <paramref name="values" /> 
		///   parameter was null, the result that is returned is null instead.
		/// </returns>
		/// <exception cref="ArgumentException">
		///  The property array does not contain all required properties or 
		///  some of them are contained in the array multiple times.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///  The <paramref name="values" /> parameter was null.
		/// </exception>
		public static Rule FromPropertyArray (PropertyValue[] values)
		{
			if (values == null)
				throw new ArgumentNullException ("values");
			
			int duplicateCheck = 0;
			Action<int> CheckDuplicateAndMark = (field) => {
				if ((duplicateCheck & (1 << field)) != 0)
					throw new ArgumentException ("duplicate.");
				else
					duplicateCheck |= (1 << field);
			};

			Rule rule = new Rule ();

			foreach (var prop in values) {
				if (prop == null)
					continue;
				
				switch (prop.PropTag) {
					case ExchangeProperty.RuleId:
						CheckDuplicateAndMark (0);
						rule.Id = (long) prop;
					break;
					case ExchangeProperty.RuleSequence:
						CheckDuplicateAndMark (1);
						rule.Sequence = (int) prop;
					break;
					case ExchangeProperty.RuleState:
						CheckDuplicateAndMark (2);
						rule.State = (int) prop;
					break;
					case ExchangeProperty.RuleUserFlags:
						CheckDuplicateAndMark (3);
						rule.UserFlags = (int) prop;
					break;
					case ExchangeProperty.RuleCondition:
						CheckDuplicateAndMark (4);
						rule.Condition = (Restriction) prop;
					break;
					case ExchangeProperty.RuleActions:
						CheckDuplicateAndMark (5);
						rule.Actions = (Actions) prop;
					break;
					case ExchangeProperty.RuleProvider:
						CheckDuplicateAndMark (6);
						rule.Provider = (string) prop;
					break;
					case ExchangeProperty.RuleName:
						CheckDuplicateAndMark (7);
						rule.Name = (string) prop;
					break;
					case ExchangeProperty.RuleLevel:
						CheckDuplicateAndMark (8);
						rule.Level = (int) prop;
					break;
					case ExchangeProperty.RuleProviderData:
						CheckDuplicateAndMark (9);
						rule.ProviderData = (byte[]) prop;
					break;
				}
			}
						
			// check that all properties were found (= 10 lower bits are set).
			if (duplicateCheck != 0x3FF)
				throw new ArgumentException ("Expected properties missing.");

			return rule;
		}
		

		
		// TODO: equality ...
		
		
		// TODO: equality (Same ruleId).
		
		
		/// <summary>Implementation the ICloneable interface.</summary>
		/// <remarks>Returns a deep copy of the Rule.</remarks>
		/// <returns>The copy of the Rule object.</returns>
		public object Clone ()
		{
			// TODO!
			throw new NotImplementedException ("Not yet implemented!");
		}

		public override string ToString ()
		{
			return "{Rule: Id 0x" + Id.ToString ("X") + "}";
		}
		

	}	

}