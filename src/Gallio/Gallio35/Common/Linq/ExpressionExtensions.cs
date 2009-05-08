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
using System.Linq.Expressions;

namespace Gallio.Common.Linq
{
    /// <summary>
    /// Extension methods for <see cref="Expression{TDelegate}" />.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Binds the arguments of a function expression.
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="expr">The expression</param>
        /// <param name="arg">The argument value</param>
        /// <returns>The bound function</returns>
        public static Expression<System.Func<TResult>> Bind<T, TResult>(this Expression<System.Func<T, TResult>> expr, T arg)
        {
            return Expression.Lambda<System.Func<TResult>>(
                Expression.Invoke(expr, Expression.Constant(arg)));
        }

        /// <summary>
        /// Binds the arguments of a function expression.
        /// </summary>
        /// <typeparam name="T1">The first parameter type</typeparam>
        /// <typeparam name="T2">The second parameter type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="expr">The expression</param>
        /// <param name="arg1">The first argument value</param>
        /// <param name="arg2">The second argument value</param>
        /// <returns>The bound function</returns>
        public static Expression<System.Func<TResult>> Bind<T1, T2, TResult>(this Expression<System.Func<T1, TResult>> expr, T1 arg1, T2 arg2)
        {
            return Expression.Lambda<System.Func<TResult>>(
                Expression.Invoke(expr, Expression.Constant(arg1), Expression.Constant(arg2)));
        }

        /// <summary>
        /// Returns true if the expression represents a captured variable within a closure.
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns>True if the expression represents a captured variable</returns>
        public static bool IsCapturedVariable(this Expression expr)
        {
            MemberExpression memberExpr = expr as MemberExpression;
            return memberExpr != null && IsCapturedVariable(memberExpr);
        }

        /// <summary>
        /// Returns true if the expression represents a captured variable within a closure.
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns>True if the expression represents a captured variable</returns>
        public static bool IsCapturedVariable(this MemberExpression expr)
        {
            return expr.Expression is ConstantExpression
                && expr.Member.DeclaringType.Name.StartsWith("<");
        }

        /// <summary>
        /// Returns true if the expression represents a captured variable or a parameter.
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns>True if the expression represents a captured variable or a parameter</returns>
        public static bool IsCapturedVariableOrParameter(this Expression expr)
        {
            return expr is ParameterExpression || IsCapturedVariable(expr);
        }
    }
}
