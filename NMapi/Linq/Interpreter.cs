//
// openmapi.org - NMapi C# Mapi API - Interpreter.cs
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
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Linq {

	internal class Interpreter<MEntity> : ExpressionTransformer
	{
		private QueryState<MEntity> qstate;
		private Restriction currentRestriction;
		private bool currentRestrictionUsed;

		public QueryState<MEntity> QueryState {
			get { return qstate; }
		}

		internal Interpreter ()
		{
			this.qstate = new QueryState<MEntity> ();
			this. currentRestriction = null;
		}

		// start ...
		internal Expression DoVisit (Expression e)
		{
			return Visit (e);
		}

		private Expression RemoveQuotes (Expression e)
		{
			while (e.NodeType == ExpressionType.Quote)
				e = ((UnaryExpression) e).Operand;
			return e;
		}

		protected override MethodCallExpression VisitMethodCall (MethodCallExpression mce)
		{
			if (mce.Method.DeclaringType == typeof (Queryable)) {
				// visit predecessor (in chain)!
				base.Visit (mce.Arguments [0]);

				switch (mce.Method.Name) {
					case "Select":	VisitSelectMethod (mce); break;
					case "Where":	VisitWhereMethod (mce); break;
					case "OrderBy":	VisitOrderByMethod (mce); break;
					case "ThenBy":	VisitThenByMethod (mce); break;
					case "OrderByDescending": VisitOrderByDescendingMethod (mce); break;
					case "ThenByDescending": VisitThenByDescendingMethod (mce); break;
					case "Reverse": VisitReverseMethod (mce); break;
					case "Skip":	VisitSkipMethod (mce); break; // Can be a problem ...
					case "Take":	VisitTakeMethod (mce); break; // Can be a problem ...
					case "ElementAt": VisitElementAtMethod (mce); break;
					case "ElementAtOrDefault": VisitElementAtOrDefaultMethod (mce); break;
					case "First":	VisitFirstLastEtcMethod (mce); break;
					case "FirstOrDefault": VisitFirstLastEtcMethod (mce); break;
					case "Last":	VisitFirstLastEtcMethod (mce); break;
					case "LastOrDefault": VisitFirstLastEtcMethod (mce); break;
					case "Single":	VisitFirstLastEtcMethod (mce); break;
					case "SingleOrDefault": VisitFirstLastEtcMethod (mce); break;
					case "Min":	VisitMinMethod (mce); break;
					case "Max":	VisitMaxMethod (mce); break;	
					case "Count":	VisitCountMethod (mce); break;	
					case "LongCount": VisitLongCountMethod (mce); break;
					case "Sum":	VisitSumMethod (mce); break;	
					case "Average":	VisitAverageMethod (mce); break;
					default:
						throw new NotSupportedException ("The method '" + 
							mce.Method.Name + "' is not supported");
				}
				return mce;
			}
			switch (mce.Method.Name) {
				case "Contains": return VisitContainsMethod (mce);
				// first (condition) or == x -> startswith
				// last (condition) or == x-> endswith
				// elementat (condition) or == x -> reg_ex? e,g. "?????X"
			}
			throw new NotSupportedException ("The method '" + 
				mce.Method.Name + "' is not supported");
		}

		protected override Expression VisitMemberAccess (MemberExpression memberExpression)
		{
			if (memberExpression.Expression != null && 
				memberExpression.Expression.NodeType == ExpressionType.Parameter)
					return new PropertyExpression (memberExpression.Member.Name);
			throw new NotSupportedException ("The member '" + 
				memberExpression.Member.Name + "' is not supported");
		}

		protected override UnaryExpression VisitUnary (UnaryExpression unaryExpr)
		{
			switch (unaryExpr.NodeType) {
				case ExpressionType.Not:
					if (qstate.CurrentCommand == CommandType.Where) {
						currentRestriction = new NotRestriction ();
						currentRestriction = ((NotRestriction) currentRestriction).Res;
						currentRestrictionUsed = true;
						this.Visit (unaryExpr.Operand);
					}
					else
						throw new NotSupportedException ("Unary expressions " + 
							"are only supported inside where-clauses!");
				break;
				default:
					throw new NotSupportedException ("The unary operator '" + 
						unaryExpr.NodeType + "' is not supported");
			}
			return unaryExpr;
		}

		private Expression VisitSelectMethod (MethodCallExpression methodCallExpr)
		{
			return methodCallExpr;
			// TODO throw new NotImplementedException ("Not implemented!");
		}

		protected Expression VisitWhereMethod (MethodCallExpression methodCallExpr)
		{
			LambdaExpression lambda = (LambdaExpression) 
					RemoveQuotes (methodCallExpr.Arguments[1]);

			qstate.CurrentCommand = CommandType.Where;
			currentRestrictionUsed = false;
			this.Visit (lambda.Body);

			if (currentRestrictionUsed)
				qstate.AddWhereRestriction (currentRestriction);
			currentRestriction = null;
			return methodCallExpr;
		}


		private Expression VisitOrderByShared (
			MethodCallExpression methodCallExpr, TableSort sortOrder)
		{
			LambdaExpression lambda = (LambdaExpression) RemoveQuotes (methodCallExpr.Arguments[1]);

			Expression expr = this.Visit (lambda.Body);
			PropertyExpression propExpr = expr as PropertyExpression;

			if (propExpr == null)
				throw new NotSupportedException ("Only Properties " + 
					"on the object are allowed in orderby-clauses!");

			MapiPropertyAttribute prop = GetMapiPropertyAttribute (propExpr.Name);

			SortOrder sOrder = new SortOrder ();
			sOrder.Order = TableSort.Descend;
			sOrder.PropTag = prop.PropertyOrKind;
			qstate.AddOrderBy (sOrder);
			return methodCallExpr;
		}

		private Expression VisitOrderByMethod (MethodCallExpression methodCallExpr)
		{
			qstate.CurrentCommand = CommandType.OrderBy;
			return VisitOrderByShared (methodCallExpr, TableSort.Ascend);
		}

		private Expression VisitThenByMethod (
			MethodCallExpression methodCallExpr)
		{
			qstate.CurrentCommand = CommandType.ThenBy;
			return VisitOrderByShared (methodCallExpr, TableSort.Ascend);
		}

		private Expression VisitOrderByDescendingMethod (
			MethodCallExpression methodCallExpr)
		{
			qstate.CurrentCommand = CommandType.OrderByDescending;
			return VisitOrderByShared (methodCallExpr, TableSort.Descend);
		}

		private Expression VisitThenByDescendingMethod (
			MethodCallExpression methodCallExpr)
		{
			qstate.CurrentCommand = CommandType.ThenByDescending;
			return VisitOrderByShared (methodCallExpr, TableSort.Descend);
		}

		private Expression VisitReverseMethod (
			MethodCallExpression methodCallExpr)
		{
			qstate.ToggleReadBackwards ();
			return methodCallExpr;
		}

		private Expression VisitElementAtMethod (MethodCallExpression methodCallExpr)
		{
			throw new NotImplementedException ("Not implemented!");
		}

		private Expression VisitElementAtOrDefaultMethod (
			MethodCallExpression methodCallExpr)
		{
			throw new NotImplementedException ("Not implemented!");
		}

		private void SetScalarOp (ScalarOperation op)
		{
			if (qstate.ScalarOperation != ScalarOperation.None)
				throw new NotSupportedException ("Only one scalar operation is allowed!");
			qstate.ScalarOperation = op;
		}

		private void SetScalarOpWithParam (ScalarOperation op, Expression param)
		{
			LambdaExpression lambda = (LambdaExpression) RemoveQuotes (param);

			Expression expr = this.Visit (lambda.Body);
			PropertyExpression propExpr = expr as PropertyExpression;
			if (propExpr == null)
				throw new NotSupportedException ("Only Properties on the " + 
					"object are allowed in scalar operations that take parameters!");

			MapiPropertyAttribute prop = GetMapiPropertyAttribute (propExpr.Name);
			qstate.ScalarQueriedProperty = prop.PropertyOrKind;

			SetScalarOp (op);
		}


		private Expression VisitFirstLastEtcMethod (MethodCallExpression methodCallExpr)
		{
			switch (methodCallExpr.Method.Name) {
				case "First":
					SetScalarOp (ScalarOperation.First);
				break;
				case "FirstOrDefault":
					SetScalarOp (ScalarOperation.FirstOrDefault);
				break;
				case "Last":
					SetScalarOp (ScalarOperation.Last);
				break;
				case "LastOrDefault":
					SetScalarOp (ScalarOperation.LastOrDefault);
				break;
				case "Single":
					SetScalarOp (ScalarOperation.Single);
				break;
				case "SingleOrDefault":
					SetScalarOp (ScalarOperation.SingleOrDefault);
				break;
			}
			return methodCallExpr;
		}

		private SortOrder BuildMiniMaxSortOrder (
			MethodCallExpression methodCallExpr, TableSort sortOrder)
		{
			if (methodCallExpr.Arguments.Count < 2)
				throw new ArgumentException ("Min ()/Max () requires an argument!");

			LambdaExpression lambda = (LambdaExpression) 
				RemoveQuotes (methodCallExpr.Arguments[1]);

			Expression expr = this.Visit (lambda.Body);
			PropertyExpression propExpr = expr as PropertyExpression;

			if (propExpr == null)
				throw new NotSupportedException ("Only Properties on " + 
					"the object are allowed in min/max calls!");

			MapiPropertyAttribute prop = GetMapiPropertyAttribute (propExpr.Name);

			SortOrder sOrder = new SortOrder ();
			sOrder.Order = sortOrder;
			sOrder.PropTag = prop.PropertyOrKind;
			return sOrder;
		}

		private Expression VisitMinMethod (MethodCallExpression methodCallExpr)
		{
			SortOrder sOrder = BuildMiniMaxSortOrder (methodCallExpr, TableSort.Ascend);
			qstate.PrependOrderBy (sOrder);
			SetScalarOpWithParam (ScalarOperation.Min, methodCallExpr.Arguments[1]);
			return methodCallExpr;
		}

		private Expression VisitMaxMethod (MethodCallExpression methodCallExpr)
		{
			SortOrder sOrder = BuildMiniMaxSortOrder (methodCallExpr, TableSort.Descend);
			qstate.PrependOrderBy (sOrder);
			SetScalarOpWithParam (ScalarOperation.Max, methodCallExpr.Arguments[1]);
			return methodCallExpr;
		}

		private Expression VisitCountMethod (MethodCallExpression methodCallExpr)
		{
			SetScalarOp (ScalarOperation.Count);
			return methodCallExpr;
		}

		private Expression VisitLongCountMethod (MethodCallExpression methodCallExpr)
		{
			SetScalarOp (ScalarOperation.LongCount);
			return methodCallExpr;
		}

		private Expression VisitAverageMethod (MethodCallExpression methodCallExpr)
		{
			SetScalarOpWithParam (ScalarOperation.Avg, methodCallExpr.Arguments[1]);
			return methodCallExpr;
		}

		private Expression VisitSumMethod (MethodCallExpression methodCallExpr)
		{
			SetScalarOpWithParam (ScalarOperation.Sum, methodCallExpr.Arguments[1]);
			return methodCallExpr;
		}


		private Expression VisitTakeMethod (MethodCallExpression methodCallExpr)
		{
			Expression tmp = this.Visit (methodCallExpr.Arguments[1]);
			ConstantExpression takeConstant = tmp as ConstantExpression;
			if (takeConstant == null)
				throw new NotSupportedException (
					"Argument must be constant and not null!");

			// We add this, so multiple take methods can be used.
			qstate.Amount += (int) takeConstant.Value;
			return methodCallExpr;
		}

		private Expression VisitSkipMethod (MethodCallExpression methodCallExpr)
		{
			Expression tmp = this.Visit (methodCallExpr.Arguments [1]);
			ConstantExpression skipConstant = tmp as ConstantExpression;
			if (skipConstant == null)
				throw new NotSupportedException (
					"Argument must be constant and not null!");
			qstate.Offset = (int) skipConstant.Value;
			return methodCallExpr;
		}

		private MethodCallExpression VisitContainsMethod (MethodCallExpression methodCallExpr)
		{
			// Assumption: should follow here ...

			MemberExpression memberExpr = methodCallExpr.Object as MemberExpression;
			MapiPropertyAttribute prop = GetMapiPropertyAttribute (memberExpr.Member.Name);

			// Assumption: a constant should follow here ...

			Expression tmp = this.Visit (methodCallExpr.Arguments [0]);
			ConstantExpression matchStrExpr = tmp as ConstantExpression;
			if (matchStrExpr == null)
				throw new NotSupportedException ("Argument must " + 
					"be constant and not null!");

			currentRestriction = new ContentRestriction ();
			((ContentRestriction) currentRestriction).FuzzyLevel = FuzzyLevel.Substring;
			((ContentRestriction) currentRestriction).PropTag = (int) prop.Type;
			((ContentRestriction) currentRestriction).Prop = MakePropertyValue (prop.Type, matchStrExpr.Value);
			
			currentRestrictionUsed = true;

			return methodCallExpr;
		}

		protected override BinaryExpression VisitBinary (BinaryExpression binaryExpr)
		{
			switch (binaryExpr.NodeType) {
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					if (qstate.CurrentCommand == CommandType.Where)
						ConstructAndRestriction (binaryExpr);
					else
						throw new NotSupportedException (
							"Binary expressions are only " + 
							"supported inside where-clauses!");
				break;
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					if (qstate.CurrentCommand == CommandType.Where)
						ConstructOrRestriction (binaryExpr);
					else
						throw new NotSupportedException ("Binary expressions " + 
							"are only supported inside where-clauses!");
				break;
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
					if (qstate.CurrentCommand == CommandType.Where)
						ConstructRelationOpRestriction (binaryExpr);
					else
					throw new NotSupportedException ("Binary expressions " + 
						"are only supported inside where-clauses!");
				break;
				case ExpressionType.Modulo:
					// modulo is used to match a regular expression
					if (qstate.CurrentCommand == CommandType.Where)
						ConstructRegExRestriction (binaryExpr);
					else
					throw new NotSupportedException ("Binary expressions " + 
						"are only supported inside where-clauses!");
				break;
				default:
					throw new NotSupportedException ("The binary operator '" + 
						binaryExpr.NodeType + "' is not supported");
			}

			return binaryExpr;
		}

		private void ConstructRelationOpRestriction (BinaryExpression binaryExpr)
		{
			RelOp relOp = RelOp.Equal;
			switch (binaryExpr.NodeType) {
				case ExpressionType.Equal: relOp = RelOp.Equal; break;
				case ExpressionType.NotEqual: relOp = RelOp.NotEqual; break;
				case ExpressionType.LessThan: relOp = RelOp.LessThan; break;
				case ExpressionType.LessThanOrEqual: relOp = RelOp.LessThanOrEqual; break;
				case ExpressionType.GreaterThan: relOp = RelOp.GreaterThan; break;
				case ExpressionType.GreaterThanOrEqual: relOp = RelOp.GreaterThanOrEqual; break;
				default:
					throw new NotSupportedException ("The operator '"
						+ binaryExpr.NodeType + "' is not supported!");
			}

			Expression left = this.Visit (binaryExpr.Left);
			Expression right = this.Visit (binaryExpr.Right);
			
			if (left is PropertyExpression && right is PropertyExpression) {
				var prop1 = GetMapiPropertyAttribute (((PropertyExpression) left).Name);
				var prop2 = GetMapiPropertyAttribute (((PropertyExpression) right).Name);
				currentRestriction = BuildComparePropsRestriction (prop1, relOp, prop2);
				currentRestrictionUsed = true;
			} else {
				PropertyExpression propExpr = left as PropertyExpression;
				ConstantExpression constExpr = right as ConstantExpression;
				if (propExpr == null) {
					propExpr = right as PropertyExpression;
					constExpr = left as ConstantExpression;
				}
				if (propExpr == null)
					throw new NotSupportedException (
						"One parameter must be a Property!");
				MapiPropertyAttribute prop = GetMapiPropertyAttribute (propExpr.Name);

				currentRestriction = BuildPropertyRestriction (prop, relOp, constExpr.Value);
				currentRestrictionUsed = true;
			}

		}

		private MapiPropertyAttribute GetMapiPropertyAttribute (string name)
		{
			Type type = typeof (MEntity);		
			PropertyInfo pinfo = type.GetProperty (name);
			object [] attribs = pinfo.GetCustomAttributes (
					typeof (MapiPropertyAttribute), true);
			var prop = attribs [0] as MapiPropertyAttribute;
			if (prop == null)
				throw new NotSupportedException (
					"Properties used in query must have " + 
					"a MapiProperty-Attribute.");
			return prop;
		}

		private ComparePropsRestriction BuildComparePropsRestriction (
			MapiPropertyAttribute prop1, RelOp relOp, MapiPropertyAttribute prop2)
		{
			ComparePropsRestriction rest = new ComparePropsRestriction ();
			rest.RelOp = relOp;
			rest.PropTag1 = prop1.PropertyOrKind;
			rest.PropTag2 = prop2.PropertyOrKind;
			return rest;
		}

		private PropertyRestriction BuildPropertyRestriction (
			MapiPropertyAttribute prop, RelOp relOp, object value)
		{
			PropertyRestriction rest = new PropertyRestriction ();
			rest.RelOp = relOp;
			rest.PropTag = prop.PropertyOrKind;
			rest.Prop = MakePropertyValue (prop.Type, value);
			return rest;
		}

		private PropertyValue MakePropertyValue (PropertyType propertyType, object value)
		{
			return PropertyValue.Make (propertyType, value);
		}


		private void ConstructAndRestriction (BinaryExpression binaryExpr)
		{
			Restriction[] children = new Restriction [2];

			this.Visit (binaryExpr.Left);
			children [0] = currentRestriction;

			this.Visit (binaryExpr.Right);
			children [1] = currentRestriction;
			
			currentRestriction = new AndRestriction (children);
			currentRestrictionUsed = true;
		}

		private void ConstructRegExRestriction (BinaryExpression binaryExpr)
		{
			throw new NotImplementedException ("Not implemented!");
		}

		private void ConstructOrRestriction (BinaryExpression binaryExpr)
		{
			Restriction[] children = new Restriction [2];

			this.Visit (binaryExpr.Left);
			children [0] = currentRestriction;
			
			this.Visit (binaryExpr.Right);
			children [1] = currentRestriction;
			
			currentRestriction = new OrRestriction (children);
			currentRestrictionUsed = true;
		}

	}

}
