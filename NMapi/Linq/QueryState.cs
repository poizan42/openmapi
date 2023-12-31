//
// openmapi.org - NMapi C# Mapi API - QueryState.cs
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
using System.Reflection;
using System.Collections.Generic;
using NMapi.Flags;
using NMapi.Table;

namespace NMapi.Linq {

	internal enum ScalarOperation
	{
		None,
		ElementAt,
		ElementAtOrDefault,
		First,
		FirstOrDefault,
		Last,
		LastOrDefault,
		Single,
		SingleOrDefault,
		Min,
		Max,
		Count,
		LongCount,
		Sum,
		Avg
	}

	internal enum CommandType
	{
		START,
		Where,
		OrderBy,
		OrderByDescending,
		ThenBy,
		ThenByDescending,
		Reverse,
		Select,
		Skip,
		Take
	}

	internal sealed class QueryState<MEntity>
	{
		private List<Restriction> andList;
		private List<SortOrder> orderByList;
		private CommandType currentCommand;
		private int offset;
		private int __amount;
		private ScalarOperation scalarOperation;
		private int scalarQueriedProperty;
		private bool readBackwards;

		//
		// This is usually ignored, except if used 
		// with some scalar operations like Sum () or Avg ().
		//
		public int ScalarQueriedProperty {
			get { return scalarQueriedProperty; }
			set { scalarQueriedProperty = value; }
		}

		public List<Restriction> AndList {
			get { return andList; }
		}

		public List<SortOrder> OrderByList {
			get { return orderByList; }
		}

		public CommandType CurrentCommand {
			get { return currentCommand; }
			set { currentCommand = value; }
		}

		public int Offset {
			get { return offset; }
			set { offset = value; }
		}

		public int Amount {
			get { return __amount; }
			set { __amount = value; }
		}

		public int ActualAmount {
			get {
				if (scalarOperation == ScalarOperation.None || 
					scalarOperation == ScalarOperation.Count ||
					scalarOperation == ScalarOperation.LongCount ||
					scalarOperation == ScalarOperation.Sum ||
					scalarOperation == ScalarOperation.Avg)
						return __amount;
				return 1;
			}
		}

		public ScalarOperation ScalarOperation {
			get { return scalarOperation; }
			set { scalarOperation = value; }
		}

		public bool ReadBackwards {
			get { return readBackwards; }
			set { readBackwards = value; }
		}


		public QueryState ()
		{
			andList = new List<Restriction> ();
			orderByList = new List<SortOrder> ();
			currentCommand = CommandType.START;
			scalarOperation = ScalarOperation.None;
			offset = 0;
			__amount = Int32.MaxValue;
			scalarQueriedProperty = -1;
			readBackwards = false;
		}


		public void AddWhereRestriction (Restriction restriction)
		{
			andList.Add (restriction);
		}

		public Restriction MergeRestrictions ()
		{
			if (andList.Count == 0)
				return null;
			if (andList.Count == 1)
				return andList [0];
			AndRestriction joined = new AndRestriction ();
			joined.Res = andList.ToArray ();
			return joined;
		}

/*
			AndRestriction connector = new AndRestriction ();

			Restriction root = new Restriction ();
			root.Rt = RestrictionType.And;
			root.Res.ResAnd = connector;

			for (int i=0;true;i++) {
				Restriction current = andList [i];

				Restriction[] children = new Restriction [2];
				children [0] = current;
				if (i == andList.Count-2)
					children [1] = andList [i+1];
				else {
					children [1] = new Restriction ();
					var and = new AndRestriction (); // next!
					children [1].Rt = RestrictionType.And;
					children [1].Res.ResAnd = and;
					connector.Res = children;
					connector = and;
				}
			}
			return root;
		}
*/
		public void PrependOrderBy (SortOrder sortOrder)
		{
			orderByList.Insert (0, sortOrder);
		}

		public void AddOrderBy (SortOrder sortOrder)
		{
			orderByList.Add (sortOrder);
		}

		public void ToggleReadBackwards ()
		{
			Console.WriteLine ("Toggle!");
			readBackwards = !readBackwards;
		}


		// ============================================
		//  For Debugging ...
		// ============================================

		private string ResolveObjectPropertyName (int propTag)
		{
			Type type = typeof (MEntity);
			PropertyInfo[] list = type.GetProperties ();
			foreach (PropertyInfo pinfo in list) {
				object [] attribs = pinfo.GetCustomAttributes (
						typeof (MapiPropertyAttribute), true);
				var prop = attribs [0] as MapiPropertyAttribute;
				if (prop == null)
					throw new NotSupportedException (
						"Properties used in query must have " + 
						"a MapiProperty-Attribute.");
				if (prop.PropertyOrKind == propTag)
					return "object." + pinfo.Name;
			}


			return "" + propTag;
		}

		private void PrintJoinRestrictions (Restriction[] restrictions, string separator)
		{
			Console.Write ("(");
			for (int i=0;i<restrictions.Length;i++) {
				PrintRestriction (restrictions [i]);
				if (i != restrictions.Length-1)
					Console.Write (separator);
			}
			Console.Write (")");
		}

		private void PrintAnd (AndRestriction andRes)
		{
			PrintJoinRestrictions (andRes.Res, " && ");
		}

		private void PrintNot (NotRestriction notRes)
		{
			Console.Write ("! (");
			PrintRestriction (notRes.Res);
			Console.Write (")");
		}

		private void PrintOr (OrRestriction orRes)
		{
			PrintJoinRestrictions (orRes.Res, " || ");
		}

		private void PrintPropConst (PropertyRestriction propRes)
		{
			Console.Write ("(" + ResolveObjectPropertyName (propRes.PropTag));
			string opName = "UNKNOWN_OPERATOR";
			switch (propRes.RelOp) {
				case RelOp.LessThan: opName = "<"; break;
				case RelOp.LessThanOrEqual: opName = "=<"; break;
				case RelOp.GreaterThan: opName = ">"; break;
				case RelOp.GreaterThanOrEqual: opName = ">="; break;
				case RelOp.Equal: opName = "=="; break;
				case RelOp.NotEqual: opName = "!="; break;
				case RelOp.RegEx: opName = "REGEX"; break;
			}
			Console.Write (" " + opName + " ");
			Console.Write ("\"" + propRes.Prop.GetValueObj () + "\")");
		}

		private void PrintPropProp (ComparePropsRestriction propRes)
		{
			Console.Write ("PrintPropProp() NOT IMPLEMENTED!");		
		}

		public void PrintRestriction (Restriction res)
		{
			if (res == null) {
				Console.Write ("NULL");
				return;
			}
			if (res is AndRestriction) PrintAnd ((AndRestriction) res);
			else if (res is NotRestriction) PrintNot ((NotRestriction) res);
			else if (res is OrRestriction) PrintOr ((OrRestriction) res);
			else if (res is PropertyRestriction) PrintPropConst ((PropertyRestriction) res);
			else if (res is ComparePropsRestriction) PrintPropProp ((ComparePropsRestriction) res);
			else
				throw new NotSupportedException ("unknown Restriction!");
		}

		private void PrintOrderBy ()
		{
			string sortType;
			for (int i=0; i<orderByList.Count;i++) {
				SortOrder sorder = orderByList [i];
				switch (sorder.Order) {
					case TableSort.Ascend:
						sortType = "ascending";
					break;
					case TableSort.Descend:
						sortType = "descending";
					break;
					case TableSort.Combine:
						sortType = "combined";
					break;
					default:
						throw new NotSupportedException ("unknown sort type!");
				}
				string propName = ResolveObjectPropertyName (sorder.PropTag);
				Console.Write (propName + " (" + sortType + ")");
				if (i != orderByList.Count-1)
					Console.Write (", ");
			}

		}

		public void Print ()
		{
			Console.WriteLine ("\n\nRestriction: ");
			PrintRestriction (MergeRestrictions ());
			Console.WriteLine ();
			Console.WriteLine ("\nOrderBy: ");
			PrintOrderBy ();
			Console.WriteLine ();
			Console.WriteLine ("\nOffset: " + offset);
			Console.WriteLine ("\nActualAmount: " + ActualAmount);
			Console.WriteLine ("\nScalarOperation: " + scalarOperation);
			Console.WriteLine ("\nReadBackwards: " + readBackwards);
		}

	}
}
