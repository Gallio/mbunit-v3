// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Gallio.Common.Linq
{
    /// <summary>
    /// Performs different actions depending on the type of <see cref="Expression"/> visited.
    /// </summary>
    /// <typeparam name="T">The visitor result type.</typeparam>
    public abstract class ExpressionVisitor<T>
    {
        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        public T Visit(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return VisitBinary((BinaryExpression) expr);

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return VisitUnary((UnaryExpression) expr);

                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression) expr);

                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression) expr);

                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression) expr);

                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression) expr);

                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression) expr);

                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression) expr);

                case ExpressionType.MemberAccess:
                    return VisitMember((MemberExpression) expr);

                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression) expr);

                case ExpressionType.New:
                    return VisitNew((NewExpression) expr);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression) expr);

                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression) expr);

                case ExpressionType.TypeIs:
                    return VisitTypeBinary((TypeBinaryExpression)expr);

                default:
                    return VisitAny(expr);
            }
        }

        /// <summary>
        /// Visits a binary expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitBinary(BinaryExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a unary expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitUnary(UnaryExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a call expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitMethodCall(MethodCallExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a conditional expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitConditional(ConditionalExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a constant expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitConstant(ConstantExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits an invocation expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitInvocation(InvocationExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a lambda expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitLambda(LambdaExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits an list init expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitListInit(ListInitExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a member access expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitMember(MemberExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a member init expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitMemberInit(MemberInitExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a new expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitNew(NewExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a new array expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitNewArray(NewArrayExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a parameter expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitParameter(ParameterExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// Visits a type binary expression.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitTypeBinary(TypeBinaryExpression expr)
        {
            return VisitAny(expr);
        }

        /// <summary>
        /// <para>
        /// Visits an expression of any type that does not have other special behavior.
        /// </para>
        /// <para>
        /// The default implementation throws <see cref="NotSupportedException"/>.
        /// </para>
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <returns>The result.</returns>
        protected virtual T VisitAny(Expression expr)
        {
            throw new NotSupportedException(String.Format("Expression node type {0} is not supported by this visitor.",
                expr.NodeType));
        }
    }
}
