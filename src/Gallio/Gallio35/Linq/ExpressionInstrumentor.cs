// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Gallio.Linq
{
    /// <summary>
    /// Instuments an <see cref="Expression{T}" /> to intercept intermediate results
    /// from each sub-expression.
    /// </summary>
    public abstract class ExpressionInstrumentor
    {
        private static readonly MethodInfo interceptVoidMethod =
            typeof(ExpressionInstrumentor).GetMethod("InterceptVoid", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo interceptNonVoidMethod =
            typeof(ExpressionInstrumentor).GetMethod("InterceptNonVoid", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Compiles an expression to introduce trace points.
        /// </summary>
        /// <typeparam name="T">The expression type</typeparam>
        /// <param name="expr">The expression tree</param>
        /// <returns>The compiled delegate representing expression</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expr"/> is null</exception>
        public T Compile<T>(Expression<T> expr)
        {
            return Rewrite(expr).Compile();
        }

        /// <summary>
        /// Rewrites an expression tree to introduce trace points.
        /// </summary>
        /// <typeparam name="T">The expression type</typeparam>
        /// <param name="expr">The expression tree</param>
        /// <returns>The compiled delegate representing expression</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expr"/> is null</exception>
        public Expression<T> Rewrite<T>(Expression<T> expr)
        {
            if (expr == null)
                throw new ArgumentNullException("expr");

            return Expression.Lambda<T>(
                new Rewriter(this).RewriteExpression(expr.Body),
                expr.Parameters);
        }

        /// <summary>
        /// Evaluates a sub-expression and collects trace information.
        /// </summary>
        /// <typeparam name="T">The return type of the sub-expression</typeparam>
        /// <param name="expr">The sub-expression to evaluate</param>
        /// <param name="continuation">The continuation that evaluates the sub-expression</param>
        /// <returns>The result of the evaluation</returns>
        [DebuggerHidden, DebuggerStepThrough]
        protected virtual T Intercept<T>(Expression expr, System.Func<T> continuation)
        {
            return continuation();
        }

        [DebuggerHidden, DebuggerStepThrough]
        private void InterceptVoid(Expression expr, System.Action continuation)
        {
            Intercept(expr, continuation.AsUnitFunc());
        }

        [DebuggerHidden, DebuggerStepThrough]
        private T InterceptNonVoid<T>(Expression expr, System.Func<T> continuation)
        {
            return Intercept(expr, continuation);
        }

        private sealed class Rewriter : ExpressionVisitor<Expression>
        {
            private readonly ExpressionInstrumentor instrumentor;

            public Rewriter(ExpressionInstrumentor instrumentor)
            {
                this.instrumentor = instrumentor;
            }

            public Expression RewriteExpression(Expression expr)
            {
                if (expr == null)
                    return null;

                Expression rewrittenExpr = Visit(expr);
                Type exprType = expr.Type;

                Debug.Assert(rewrittenExpr.NodeType == expr.NodeType, String.Format(
                    "The rewritten expression node type '{0}' should equal the original expression node type '{1}'.", rewrittenExpr.NodeType, expr.NodeType));
                Debug.Assert(rewrittenExpr.Type == exprType, String.Format(
                    "The rewritten expression type '{0}' should equal the original expression type '{1}'.", rewrittenExpr.Type, exprType));

                if (exprType == typeof(void))
                {
                    return Expression.Call(
                        Expression.Constant(instrumentor),
                        interceptVoidMethod,
                        Expression.Constant(expr),
                        Expression.Lambda(
                            typeof(System.Action),
                            rewrittenExpr));
                }
                else
                {
                    // Note: This is a tricky little hack.
                    //       Expression nodes for NewArrayInit or NewArrayBounds have a type which is
                    //       not a sized array.  You can see this because the type will print like
                    //       "Int32[*]" instead of "Int32[]".  It turns out that you get a
                    //       VerificationException when calling a generic function specialized to a
                    //       rank-1 rather than sized array type.  So here we detect the case and
                    //       ensure that we use a sized array.  Not sure if there will be any deeper
                    //       consequences... -- Jeff.
                    if (exprType.IsArray && exprType.GetArrayRank() == 1)
                        exprType = exprType.GetElementType().MakeArrayType();

                    return Expression.Call(
                        Expression.Constant(instrumentor),
                        interceptNonVoidMethod.MakeGenericMethod(exprType),
                        Expression.Constant(expr),
                        Expression.Lambda(
                            typeof(System.Func<>).MakeGenericType(exprType),
                            rewrittenExpr));
                }
            }

            protected override Expression VisitBinary(BinaryExpression expr)
            {
                return Expression.MakeBinary(
                    expr.NodeType,
                    RewriteExpression(expr.Left),
                    RewriteExpression(expr.Right),
                    expr.IsLiftedToNull,
                    expr.Method,
                    expr.Conversion);
            }

            protected override Expression VisitUnary(UnaryExpression expr)
            {
                if (expr.NodeType == ExpressionType.Quote)
                    return expr;
                if (expr.NodeType == ExpressionType.UnaryPlus)
                    return Expression.UnaryPlus(RewriteExpression(expr.Operand), expr.Method);

                return Expression.MakeUnary(
                    expr.NodeType,
                    RewriteExpression(expr.Operand),
                    expr.Type,
                    expr.Method);
            }

            protected override Expression VisitMethodCall(MethodCallExpression expr)
            {
                return Expression.Call(
                    RewriteExpression(expr.Object),
                    expr.Method,
                    RewriteExpressions(expr.Arguments));
            }

            protected override Expression VisitConditional(ConditionalExpression expr)
            {
                return Expression.Condition(
                    RewriteExpression(expr.Test),
                    RewriteExpression(expr.IfTrue),
                    RewriteExpression(expr.IfFalse));
            }

            protected override Expression VisitInvocation(InvocationExpression expr)
            {
                return Expression.Invoke(
                    RewriteExpression(expr.Expression),
                    RewriteExpressions(expr.Arguments));
            }

            protected override Expression VisitListInit(ListInitExpression expr)
            {
                return Expression.ListInit(
                    expr.NewExpression,
                    RewriteInitializers(expr.Initializers));
            }

            protected override Expression VisitMember(MemberExpression expr)
            {
                return Expression.MakeMemberAccess(
                    RewriteExpression(expr.Expression),
                    expr.Member);
            }

            protected override Expression VisitMemberInit(MemberInitExpression expr)
            {
                return Expression.MemberInit(expr.NewExpression,
                    RewriteBindings(expr.Bindings));
            }

            protected override Expression VisitNew(NewExpression expr)
            {
                // Note: We get an ArgumentException when Members is null and we use the
                // full specification of the New expression.
                if (expr.Members == null)
                    return Expression.New(
                        expr.Constructor,
                        RewriteExpressions(expr.Arguments));

                return Expression.New(
                    expr.Constructor,
                    RewriteExpressions(expr.Arguments),
                    expr.Members);
            }

            protected override Expression VisitNewArray(NewArrayExpression expr)
            {
                if (expr.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(
                        expr.Type.GetElementType(),
                        RewriteExpressions(expr.Expressions));
                }

                return Expression.NewArrayBounds(
                    expr.Type.GetElementType(),
                    RewriteExpressions(expr.Expressions));
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression expr)
            {
                return Expression.TypeIs(
                    RewriteExpression(expr.Expression),
                    expr.TypeOperand);
            }

            protected override Expression VisitAny(Expression expr)
            {
                return expr;
            }

            private IEnumerable<MemberBinding> RewriteBindings(IEnumerable<MemberBinding> bindings)
            {
                foreach (MemberBinding binding in bindings)
                {
                    switch (binding.BindingType)
                    {
                        case MemberBindingType.Assignment:
                            yield return Expression.Bind(binding.Member, RewriteExpression(((MemberAssignment)binding).Expression));
                            break;
                        case MemberBindingType.ListBinding:
                            yield return Expression.ListBind(binding.Member, RewriteInitializers(((MemberListBinding)binding).Initializers));
                            break;
                        case MemberBindingType.MemberBinding:
                            yield return Expression.MemberBind(binding.Member, RewriteBindings(((MemberMemberBinding)binding).Bindings));
                            break;
                    }
                }
            }

            private IEnumerable<ElementInit> RewriteInitializers(IEnumerable<ElementInit> inits)
            {
                foreach (ElementInit init in inits)
                    yield return Expression.ElementInit(init.AddMethod, RewriteExpressions(init.Arguments));
            }

            private IEnumerable<Expression> RewriteExpressions(IEnumerable<Expression> exprs)
            {
                return from e in exprs select RewriteExpression(e);
            }
        }
    }
}
