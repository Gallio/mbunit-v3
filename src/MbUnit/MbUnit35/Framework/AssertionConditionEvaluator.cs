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
using Gallio.Collections;
using Gallio.Framework.Formatting;
using Gallio.Linq;
using Gallio.Model.Diagnostics;

namespace MbUnit.Framework
{
    /// <summary>
    /// Evaluates a conditional expression.  If the condition evaluates differently
    /// than expected, returns a detailed <see cref="AssertionFailure"/> that
    /// describes the formatted values of relevant sub-expressions within the condtion.
    /// </summary>
    [TestFrameworkInternal]
    public static class AssertionConditionEvaluator
    {
        /// <summary>
        /// Evaluates a conditional expression.
        /// </summary>
        /// <param name="condition">The conditional expression</param>
        /// <param name="expectedResult">The expected result</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <returns>The assertion failure if the conditional expression evaluated
        /// to a different result than was expected or threw an exception, otherwise null</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="condition"/> is null</exception>
        public static AssertionFailure Eval(Expression<System.Func<bool>> condition, bool expectedResult,
            string messageFormat, params object[] messageArgs)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");

            return new Interpreter(condition, expectedResult, messageFormat, messageArgs).Eval();
        }

        private sealed class Interpreter : ExpressionInstrumentor
        {
            private readonly Expression<System.Func<bool>> condition;
            private readonly bool expectedResult;
            private readonly string messageFormat;
            private readonly object[] messageArgs;

            private Trace currentTrace;

            public Interpreter(Expression<System.Func<bool>> condition, bool expectedResult,
                string messageFormat, object[] messageArgs)
            {
                this.condition = condition;
                this.expectedResult = expectedResult;
                this.messageFormat = messageFormat;
                this.messageArgs = messageArgs;
            }

            public AssertionFailure Eval()
            {
                currentTrace = new Trace(null);

                try
                {
                    bool result = Compile(condition)();
                    if (result == expectedResult)
                        return null;

                    return DescribeAssertionFailure(currentTrace);
                }
                catch (AbruptTerminationException ex)
                {
                    return DescribeAssertionFailure(ex.Trace);
                }
            }

            [DebuggerHidden, DebuggerStepThrough]
            protected override T Intercept<T>(Expression expr, System.Func<T> continuation)
            {
                Trace parentTrace = currentTrace;
                Trace trace = new Trace(expr);
                try
                {
                    currentTrace.AddChild(trace);
                    currentTrace = trace;

                    T result = base.Intercept(expr, continuation);
                    trace.Result = result;
                    return result;
                }
                catch (AbruptTerminationException ex)
                {
                    trace.Exception = ex.Trace.Exception;
                    throw;
                }
                catch (Exception ex)
                {
                    trace.Exception = ex;
                    throw new AbruptTerminationException(trace);
                }
                finally
                {
                    currentTrace = parentTrace;
                }
            }

            private AssertionFailure DescribeAssertionFailure(Trace trace)
            {
                // Skip trivial nodes in the trace tree that are not of much interest.
                while (IsTrivialExpression(trace.Expression) && trace.Children.Count == 1 && trace.Exception == null)
                    trace = trace.Children[0];

                string expectedResultString = expectedResult ? "true" : "false";

                AssertionFailureBuilder failureBuilder;
                if (trace.Exception != null)
                {
                    failureBuilder = new AssertionFailureBuilder(
                        String.Format("Expected the condition to evaluate to {0} but it threw an exception.", expectedResultString))
                        .AddException(trace.Exception);
                }
                else
                {
                    failureBuilder = new AssertionFailureBuilder(
                        String.Format("Expected the condition to evaluate to {0}.", expectedResultString));
                }

                failureBuilder.SetMessage(messageFormat, messageArgs);
                failureBuilder.SetLabeledValue("Condition", condition.Body);

                var labeledTraces = new List<Trace>();
                AddLabeledTraces(labeledTraces, trace, 0);
                foreach (Trace labeledTrace in labeledTraces)
                    failureBuilder.SetLabeledValue(Formatter.Instance.Format(labeledTrace.Expression), labeledTrace.Result);

                return failureBuilder.ToAssertionFailure();
            }

            private static void AddLabeledTraces(List<Trace> labeledTraces, Trace trace, int depth)
            {
                if (trace.Exception == null)
                {
                    Expression expr = trace.Expression;
                    if (!(expr is ConstantExpression))
                    {
                        // We print the expressions at depth 1 but not the one at depth 0 because
                        // at depth 0.  We know that the value was not equal to what we expected
                        // so printing the value at depth 0 tells us nothing of interest.
                        // To help determine the cause of this effect, we print the value of the
                        // sub-expressions at depth 1.
                        //
                        // The exception to this rule is when the expression is a captured
                        // variable or parameter.  We always print these values because they are
                        // most instructive about the context in which the expression was being
                        // evaluated.  For example, they may describe the value of a loop iteration
                        // variable and other terms.
                        if (depth == 1 || expr.IsCapturedVariableOrParameter())
                        {
                            if (!labeledTraces.Contains(trace))
                                labeledTraces.Add(trace);
                        }
                    }
                }

                foreach (Trace child in trace.Children)
                    AddLabeledTraces(labeledTraces, child, depth + 1);
            }

            private static bool IsTrivialExpression(Expression expr)
            {
                return expr == null || expr.NodeType == ExpressionType.Not;
            }
        }

        private sealed class Trace
        {
            private List<Trace> children;

            public Trace(Expression expression)
            {
                Expression = expression;
            }

            public Expression Expression { get; private set; }

            public object Result { get; set; }

            public Exception Exception { get; set; }

            public IList<Trace> Children
            {
                get { return children ?? (IList<Trace>) EmptyArray<Trace>.Instance; }
            }

            public void AddChild(Trace child)
            {
                if (children == null)
                    children = new List<Trace>();
                children.Add(child);
            }
        }

        [Serializable]
        private sealed class AbruptTerminationException : Exception
        {
            public AbruptTerminationException(Trace trace)
            {
                Trace = trace;
            }

            public Trace Trace { get; private set; }
        }
    }
}
