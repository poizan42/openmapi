//
// openmapi.org - NMapi C# Mapi API - BranchExecutor.cs
//
// Copyright 2008 Topalis AG
//
// Based on code by Matt Warren http://blogs.msdn.com/mattwar .
//
// Microsoft Public License (Ms-PL)
//
// This license governs use of the accompanying software. If you use the 
// software, you accept this license. If you do not accept the license, do not 
// use the software.
//
// 1. Definitions
//
// The terms "reproduce," "reproduction," "derivative works," and "distribution" 
// have the same meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to 
// the software. A "contributor" is any person that distributes its contribution 
// under this license. "Licensed patents" are a contributor's patent claims that 
// read directly on its contribution.
// 
// 2. Grant of Rights
//
// (A) Copyright Grant- Subject to the terms of this license, including the 
//     license conditions and limitations in section 3, each contributor grants 
//     you a non-exclusive, worldwide, royalty-free copyright license to re-
//     produce its contribution, prepare derivative works of its contribution, 
//     and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the 
//     license conditions and limitations in section 3, each contributor grants 
//     you a non-exclusive, worldwide, royalty-free license under its licensed 
//     patents to make, have made, use, sell, offer for sale, import, and/or 
//     otherwise dispose of its contribution in the software or derivative works 
//     of the contribution in the software.
// 
// 3. Conditions and Limitations
//
// (A) No Trademark License- This license does not grant you rights to use any 
//     contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that 
//     you claim are infringed by the software, your patent license from such 
//     contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all 
//     copyright, patent, trademark, and attribution notices that are present 
//     in the software.
// (D) If you distribute any portion of the software in source code form, you 
//     may do so only under this license by including a complete copy of this 
//     license with your distribution. If you distribute any portion of the 
//     software in compiled or object code form, you may only do so under a 
//     license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The 
//     contributors give no express warranties, guarantees or conditions. You 
//     may have additional consumer rights under your local laws which this 
//     license cannot change. To the extent permitted under your local laws, 
//     the contributors exclude the implied warranties of merchantability, 
//     fitness for a particular purpose and non-infringement.
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
