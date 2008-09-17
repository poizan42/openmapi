//
// openmapi.org - NMapi C# Mapi API - BranchExecutor.cs
//
// Copyright 2008 Topalis AG
//
// Based on code by Matt Warren http://blogs.msdn.com/mattwar .
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
using System.Linq;
using System.Linq.Expressions;

namespace NMapi.Linq {

	internal static class TypeSystem
	{
		internal static Type GetElementType (Type seqType)
		{
			Type ienum = FindIEnumerable (seqType);
			if (ienum == null)
				return seqType;
			return ienum.GetGenericArguments () [0];
		}

		private static Type FindIEnumerable (Type seqType)
		{
			if (seqType == null || seqType == typeof (string))
				return null;
			if (seqType.IsArray)
				return typeof(IEnumerable<>).MakeGenericType (seqType.GetElementType());
			if (seqType.IsGenericType) {
				foreach (Type arg in seqType.GetGenericArguments ()) {
					Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
					if (ienum.IsAssignableFrom (seqType))
						return ienum;
				}
			}
			Type[] ifaces = seqType.GetInterfaces ();
			if (ifaces != null && ifaces.Length > 0) {
				foreach (Type iface in ifaces) {
					Type ienum = FindIEnumerable (iface);
					if (ienum != null)
						return ienum;
				}
			}
			if (seqType.BaseType != null && seqType.BaseType != typeof (object))
				return FindIEnumerable (seqType.BaseType);
			return null;
		}

	}

	internal class BranchExecutor: ExpressionTransformer
	{
		private HashSet<Expression> candidates;

		public Expression Run (Expression expression)
		{
			this.candidates = new CandidateFinder ().Find (expression);
			return Visit (expression);
		}

		protected override Expression Visit (Expression expression)
		{
			if (expression != null) {
				if (!candidates.Contains (expression))
					return base.Visit (expression);
				if (expression.NodeType == ExpressionType.Constant)
					return expression;
				Delegate function = Expression.Lambda (expression).Compile ();
				return Expression.Constant (function.DynamicInvoke (null), expression.Type);
			}
			return null;
		}

		class CandidateFinder : ExpressionTransformer
		{
			private HashSet<Expression> candidates;
			private bool canBeEvaluated;

			private void Reset ()
			{
				candidates = new HashSet<Expression>();
				canBeEvaluated = true;
			}

			internal HashSet<Expression> Find (Expression expression)
			{
				Reset ();
				this.Visit (expression);
				return candidates;
			}

			protected override Expression Visit (Expression expression)
			{
				if (expression == null) {
					bool saveCanBeEvaluated = canBeEvaluated;
					canBeEvaluated = true;
					base.Visit (expression);
					if (canBeEvaluated) {
						if (expression.NodeType == ExpressionType.Parameter)
							canBeEvaluated = false;
						else
							candidates.Add (expression);
					}
					canBeEvaluated |= saveCanBeEvaluated;
				}
				return expression;
			}
		}
	}

}
