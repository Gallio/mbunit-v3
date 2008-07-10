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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Gallio.Linq;

namespace Gallio.Framework.Formatting
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="Expression" />.
    /// </para>
    /// <para>
    /// Formats expression trees using a more familiar C#-like syntax than
    /// the default.  Also recognizes captured variables and displays them
    /// naturally to conceal the implied field access to an anonymous class.
    /// </para>
    /// <para>
    /// Made-up syntax for nodes that cannot be directly represented in C#.
    /// <list type="bullet">
    /// <item>Power operator: **, as in a ** b</item>
    /// <item>Quote expression: `...`, as in `a + b`</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ExpressionFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (typeof(Expression).IsAssignableFrom(type))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            Expression value = (Expression)obj;
            ExpressionFormatter visitor = new ExpressionFormatter(formatter);
            visitor.Visit(value);
            return visitor.ToString();
        }

        // According to C# language spec, section 7.2.1
        private enum Precedence
        {
            Primary, // x.y  f(x)  a[x]  x++  x--  new  typeof  checked  unchecked
            Unary, // +  -  !  ~  ++x  --x  (T)x
            Multiplicative, // *  /  %
            Additive, // +  -
            Shift, // <<  >>
            Relational, // <  >  <=  >=  is  as
            Equality, // ==  !=
            LogicalAnd, // &
            LogicalXor, // ^
            LogicalOr, // |
            ConditionalAnd, // &&
            ConditionalOr, // ||
            Conditional, // ?:
            Assignment, // =  *=  /=  %=  +=  -=  <<=  >>=  &=  ^=  |= ??
            Lambda
        }

        private enum CheckingMode
        {
            Inherit,
            Checked,
            Unchecked,
            Reset
        }

        private sealed class ExpressionFormatter : ExpressionVisitor<Unit>
        {
            private readonly IFormatter formatter;
            private readonly StringBuilder result;
            private readonly Stack<Pair<Precedence, CheckingMode>> modeStack;

            public ExpressionFormatter(IFormatter formatter)
            {
                this.formatter = formatter;
                result = new StringBuilder();
                modeStack = new Stack<Pair<Precedence, CheckingMode>>();
                modeStack.Push(new Pair<Precedence, CheckingMode>(Precedence.Lambda, CheckingMode.Reset));
            }

            public override string ToString()
            {
                return result.ToString();
            }

            protected override Unit VisitBinary(BinaryExpression expr)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Add:
                        return VisitInfixOperator(expr, "+", Precedence.Additive, CheckingMode.Unchecked);
                    case ExpressionType.AddChecked:
                        return VisitInfixOperator(expr, "+", Precedence.Additive, CheckingMode.Checked);
                    case ExpressionType.And:
                        return VisitInfixOperator(expr, "&", Precedence.LogicalAnd, CheckingMode.Inherit);
                    case ExpressionType.AndAlso:
                        return VisitInfixOperator(expr, "&&", Precedence.ConditionalAnd, CheckingMode.Inherit);
                    case ExpressionType.Coalesce:
                        return VisitInfixOperator(expr, "??", Precedence.Assignment, CheckingMode.Inherit);
                    case ExpressionType.Divide:
                        return VisitInfixOperator(expr, "/", Precedence.Multiplicative, CheckingMode.Inherit);
                    case ExpressionType.Equal:
                        return VisitInfixOperator(expr, "==", Precedence.Equality, CheckingMode.Inherit);
                    case ExpressionType.ExclusiveOr:
                        return VisitInfixOperator(expr, "^", Precedence.LogicalXor, CheckingMode.Inherit);
                    case ExpressionType.GreaterThan:
                        return VisitInfixOperator(expr, ">", Precedence.Relational, CheckingMode.Inherit);
                    case ExpressionType.GreaterThanOrEqual:
                        return VisitInfixOperator(expr, ">=", Precedence.Relational, CheckingMode.Inherit);
                    case ExpressionType.LeftShift:
                        return VisitInfixOperator(expr, "<<", Precedence.Shift, CheckingMode.Inherit);
                    case ExpressionType.LessThan:
                        return VisitInfixOperator(expr, "<", Precedence.Relational, CheckingMode.Inherit);
                    case ExpressionType.LessThanOrEqual:
                        return VisitInfixOperator(expr, "<=", Precedence.Relational, CheckingMode.Inherit);
                    case ExpressionType.Modulo:
                        return VisitInfixOperator(expr, "%", Precedence.Multiplicative, CheckingMode.Inherit);
                    case ExpressionType.Multiply:
                        return VisitInfixOperator(expr, "*", Precedence.Multiplicative, CheckingMode.Unchecked);
                    case ExpressionType.MultiplyChecked:
                        return VisitInfixOperator(expr, "*", Precedence.Multiplicative, CheckingMode.Checked);
                    case ExpressionType.NotEqual:
                        return VisitInfixOperator(expr, "!=", Precedence.Equality, CheckingMode.Inherit);
                    case ExpressionType.Or:
                        return VisitInfixOperator(expr, "|", Precedence.LogicalOr, CheckingMode.Inherit);
                    case ExpressionType.OrElse:
                        return VisitInfixOperator(expr, "||", Precedence.ConditionalOr, CheckingMode.Inherit);
                    case ExpressionType.Power:
                        return VisitInfixOperator(expr, "**", Precedence.Multiplicative, CheckingMode.Inherit);
                    case ExpressionType.RightShift:
                        return VisitInfixOperator(expr, ">>", Precedence.Shift, CheckingMode.Inherit);
                    case ExpressionType.Subtract:
                        return VisitInfixOperator(expr, "-", Precedence.Additive, CheckingMode.Unchecked);
                    case ExpressionType.SubtractChecked:
                        return VisitInfixOperator(expr, "-", Precedence.Additive, CheckingMode.Checked);
                    case ExpressionType.ArrayIndex:
                        return VisitArrayIndex(expr);
                    default:
                        throw new NotSupportedException();
                }
            }

            private Unit VisitInfixOperator(BinaryExpression expr, string token, Precedence precedence, CheckingMode checkingMode)
            {
                BeginExpression(precedence, checkingMode);

                Visit(expr.Left);
                result.Append(' ').Append(token).Append(' ');
                Visit(expr.Right);

                EndExpression();
                return Unit.Value;
            }

            private Unit VisitArrayIndex(BinaryExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                Visit(expr.Left);
                result.Append('[');
                Visit(expr.Right);
                result.Append(']');

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitUnary(UnaryExpression expr)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.ArrayLength:
                        return VisitArrayLength(expr);
                    case ExpressionType.Convert:
                        return VisitPrefixOperator(expr, "(" + FormatTypeName(expr.Type) + ")", Precedence.Unary, CheckingMode.Unchecked);
                    case ExpressionType.ConvertChecked:
                        return VisitPrefixOperator(expr, "(" + FormatTypeName(expr.Type) + ")", Precedence.Unary, CheckingMode.Checked);
                    case ExpressionType.Negate:
                        return VisitPrefixOperator(expr, "-", Precedence.Unary, CheckingMode.Unchecked);
                    case ExpressionType.NegateChecked:
                        return VisitPrefixOperator(expr, "-", Precedence.Unary, CheckingMode.Checked);
                    case ExpressionType.Not:
                        return VisitPrefixOperator(expr, expr.Type == typeof(bool) ? "!" : "~", Precedence.Unary, CheckingMode.Inherit);
                    case ExpressionType.UnaryPlus:
                        return VisitPrefixOperator(expr, "+", Precedence.Unary, CheckingMode.Inherit);
                    case ExpressionType.Quote:
                        return VisitQuote(expr);
                    case ExpressionType.TypeAs:
                        return VisitTypeAs(expr);
                    default:
                        throw new NotSupportedException();
                }
            }

            private Unit VisitPrefixOperator(UnaryExpression expr, string token, Precedence precedence, CheckingMode checkingMode)
            {
                BeginExpression(precedence, checkingMode);

                result.Append(token).Append(' ');
                Visit(expr.Operand);

                EndExpression();
                return Unit.Value;
            }

            private Unit VisitArrayLength(UnaryExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                Visit(expr.Operand);
                result.Append(".Length");

                EndExpression();
                return Unit.Value;
            }

            private Unit VisitQuote(UnaryExpression expr)
            {
                BeginExpression(Precedence.Lambda, CheckingMode.Reset);

                result.Append('`');
                Visit(expr.Operand);
                result.Append('`');

                EndExpression();
                return Unit.Value;
            }

            private Unit VisitTypeAs(UnaryExpression expr)
            {
                BeginExpression(Precedence.Relational, CheckingMode.Inherit);

                Visit(expr.Operand);
                result.Append(" as ");
                result.Append(FormatTypeName(expr.Type));

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitMethodCall(MethodCallExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                AppendMemberAccess(expr.Object, expr.Method);
                result.Append('.');
                result.Append(expr.Method.Name);
                AppendArguments(expr.Arguments);
                EndExpression();

                return Unit.Value;
            }

            protected override Unit VisitConditional(ConditionalExpression expr)
            {
                BeginExpression(Precedence.Conditional, CheckingMode.Inherit);

                Visit(expr.Test);
                result.Append(" ? ");
                Visit(expr.IfTrue);
                result.Append(" : ");
                Visit(expr.IfFalse);

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitConstant(ConstantExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                bool isType = expr.Value is Type;
                if (isType)
                    result.Append("typeof(");

                result.Append(formatter.Format(expr.Value));

                if (isType)
                    result.Append(')');

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitInvocation(InvocationExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                Visit(expr.Expression);
                AppendArguments(expr.Arguments);
                result.Append(')');

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitLambda(LambdaExpression expr)
            {
                BeginExpression(Precedence.Lambda, CheckingMode.Reset);

                AppendParameters(expr.Parameters);
                result.Append(" => ");
                Visit(expr.Body);

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitListInit(ListInitExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                Visit(expr.NewExpression);
                AppendListInitializer(expr.Initializers);

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitMember(MemberExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                if (! IsCapturedVariable(expr))
                    AppendMemberAccess(expr.Expression, expr.Member);
                else
                    result.Append(expr.Member.Name);

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitMemberInit(MemberInitExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                Visit(expr.NewExpression);
                result.Append(' ');
                AppendObjectInitializer(expr.Bindings);

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitNew(NewExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                result.Append("new ");
                result.Append(expr.Constructor.DeclaringType.Name);
                AppendArguments(expr.Arguments);

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitNewArray(NewArrayExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                result.Append("new ");
                result.Append(FormatTypeName(expr.Type.GetElementType()));
                result.Append('[');
                if (expr.NodeType == ExpressionType.NewArrayBounds)
                    AppendExpressionList(expr.Expressions);
                result.Append(']');

                if (expr.NodeType == ExpressionType.NewArrayInit)
                {
                    result.Append(" {");
                    if (expr.Expressions.Count != 0)
                    {
                        result.Append(' ');
                        AppendExpressionList(expr.Expressions);
                    }
                    result.Append(" }");
                }

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitParameter(ParameterExpression expr)
            {
                BeginExpression(Precedence.Primary, CheckingMode.Inherit);

                result.Append(expr.Name);

                EndExpression();
                return Unit.Value;
            }

            protected override Unit VisitTypeBinary(TypeBinaryExpression expr)
            {
                BeginExpression(Precedence.Relational, CheckingMode.Inherit);

                Visit(expr.Expression);
                result.Append(" is ");
                result.Append(FormatTypeName(expr.TypeOperand));

                EndExpression();
                return Unit.Value;
            }

            private void BeginExpression(Precedence innerPrecedence, CheckingMode innerMode)
            {
                Precedence outerPrecedence = modeStack.Peek().First;
                CheckingMode outerMode = modeStack.Peek().Second;

                if (innerMode == CheckingMode.Inherit)
                    innerMode = outerMode;

                bool? modeSwitch = DetermineCheckingModeSwitch(outerMode, innerMode);
                if (modeSwitch == true)
                    result.Append("checked(");
                else if (modeSwitch == false)
                    result.Append("unchecked(");
                else if (AreParenthesesNeeded(outerPrecedence, innerPrecedence))
                    result.Append('(');

                modeStack.Push(new Pair<Precedence,CheckingMode>(innerPrecedence, innerMode));
            }

            private void EndExpression()
            {
                Precedence innerPrecedence = modeStack.Peek().First;
                CheckingMode innerMode = modeStack.Pop().Second;
                Precedence outerPrecedence = modeStack.Peek().First;
                CheckingMode outerMode = modeStack.Peek().Second;

                bool? modeSwitch = DetermineCheckingModeSwitch(outerMode, innerMode);
                if (modeSwitch.HasValue || AreParenthesesNeeded(outerPrecedence, innerPrecedence))
                    result.Append(')');
            }

            private void AppendMemberAccess(Expression objectExpr, MemberInfo member)
            {
                if (objectExpr != null)
                    Visit(objectExpr);
                else
                    result.Append(member.ReflectedType.Name);
            }

            private void AppendArguments(ReadOnlyCollection<Expression> args)
            {
                result.Append('(');
                AppendExpressionList(args);
                result.Append(')');
            }

            private void AppendParameters(ReadOnlyCollection<ParameterExpression> @params)
            {
                if (@params.Count != 1)
                    result.Append('(');

                AppendExpressionList(@params);

                if (@params.Count != 1)
                    result.Append(')');
            }

            private void AppendExpressionList<T>(IEnumerable<T> exprs)
                where T : Expression
            {
                bool first = true;
                foreach (T expr in exprs)
                {
                    if (first)
                        first = false;
                    else
                        result.Append(", ");

                    Visit(expr);
                }
            }

            private void AppendObjectInitializer(ReadOnlyCollection<MemberBinding> bindings)
            {
                result.Append(" {");
                if (bindings.Count != 0)
                {
                    result.Append(' ');

                    bool first = true;
                    foreach (MemberBinding binding in bindings)
                    {
                        if (first)
                            first = false;
                        else
                            result.Append(", ");

                        result.Append(binding.Member.Name);
                        result.Append(" = ");

                        switch (binding.BindingType)
                        {
                            case MemberBindingType.Assignment:
                                Visit(((MemberAssignment)binding).Expression);
                                break;

                            case MemberBindingType.ListBinding:
                                AppendListInitializer(((MemberListBinding)binding).Initializers);
                                break;

                            case MemberBindingType.MemberBinding:
                                AppendObjectInitializer(((MemberMemberBinding)binding).Bindings);
                                break;
                        }
                    }
                }
                result.Append(" }");
            }

            private void AppendListInitializer(ReadOnlyCollection<ElementInit> inits)
            {
                result.Append(" {");
                if (inits.Count != 0)
                {
                    result.Append(' ');

                    bool first = true;
                    foreach (ElementInit init in inits)
                    {
                        if (first)
                            first = false;
                        else
                            result.Append(", ");

                        if (init.Arguments.Count != 1)
                            throw new NotImplementedException("Don't know how to handle case where multiple arguments are passed to a list initializer method.");

                        Visit(init.Arguments[0]);
                    }
                }
                result.Append(" }");
            }

            private string FormatTypeName(Type type)
            {
                return formatter.Format(type);
            }

            private static bool? DetermineCheckingModeSwitch(CheckingMode outerMode, CheckingMode innerMode)
            {
                if (innerMode == CheckingMode.Checked && (outerMode == CheckingMode.Unchecked || outerMode == CheckingMode.Reset))
                    return true;
                if (innerMode == CheckingMode.Unchecked && outerMode == CheckingMode.Checked)
                    return false;
                return null;
            }

            private static bool AreParenthesesNeeded(Precedence outerPrecedence, Precedence innerPrecedence)
            {
                return innerPrecedence > outerPrecedence;
            }

            private static bool IsCapturedVariable(MemberExpression expr)
            {
                return expr.Expression is ConstantExpression
                    && expr.Member.DeclaringType.Name.StartsWith("<");
            }
        }
    }
}