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
using System.Collections.Generic;
using Gallio.Runtime.Extensibility;

namespace Gallio.Common
{
    /// <summary>
    /// A condition is a simple light-weight mechanism for evaluating conditional
    /// expressions in a given context.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Conditions are most useful for defining declarative conditional activation
    /// expressions in <see cref="Traits" />.  For example, we might wish to automatically
    /// activate a plugin or enable an action when a given property appears in the
    /// conditional context.
    /// </para>
    /// <para>
    /// The currently-supported conditional expression syntax is as follows:
    /// <list type="bullet">
    /// <item>"${namespace:identifier}": Specifies a condition based on the existence
    /// of a property with a particular identifier within the specified namespace.
    /// eg. "${env:VariableName}" might evaluate to true if the "VariableName" environment
    /// variable is defined.</item>
    /// <item>"ExprA or ExprB": Given two conditions ExprA and ExprB produces a combined
    /// condition that evaluates to true when either on is true.</item>
    /// <item>An empty condition is invalid.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Note: This abstraction is a work in progress.  If you need additional
    /// conditional expression types, operators or combinators please post
    /// a feature request to the Gallio issue tracker.  Later we may open
    /// this up similar to the test Filter mechanism.
    /// </para>
    /// </remarks>
    public sealed class Condition
    {
        private readonly Constraint constraint;
        private readonly string expression;

        private Condition(Constraint constraint, string expression)
        {
            this.constraint = constraint;
            this.expression = expression;
        }

        /// <summary>
        /// Returns true if the condition is satisfied in the given context.
        /// </summary>
        /// <param name="context">The condition evaluation context.</param>
        /// <returns>True if the condition is satisfied.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is null.</exception>
        public bool Evaluate(ConditionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return constraint.Evaluate(context);
        }

        /// <summary>
        /// Parses a condition from an expression.
        /// </summary>
        /// <param name="expression">The expression to parse.</param>
        /// <returns>The expression.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expression"/> is null.</exception>
        /// <exception cref="FormatException">Thrown if <paramref name="expression"/> cannot be parsed.</exception>
        public static Condition Parse(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var words = new Queue<string>(expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            Constraint constraint = ParseConstraintDisjunction(words);
            return new Condition(constraint, expression);
        }

        private static Constraint ParseConstraintDisjunction(Queue<string> words)
        {
            Constraint left = ParseConstraintTerm(words);

            while (words.Count != 0)
            {
                string word = words.Dequeue();
                if (word == "or")
                {
                    Constraint right = ParseConstraintTerm(words);
                    left = new OrConstraint(left, right);
                }
                else
                {
                    throw new FormatException(string.Format("Did not expect '{0}' in this context.", word));
                }
            }

            return left;
        }

        private static Constraint ParseConstraintTerm(Queue<string> words)
        {
            if (words.Count == 0)
                throw new FormatException("Incomplete conditional expression.");

            string word = words.Dequeue();
            if (word.StartsWith("${") && word.EndsWith("}"))
            {
                int colonPos = word.IndexOf(':', 2, word.Length - 3);
                if (colonPos < 3 || colonPos > word.Length - 3)
                    throw new FormatException(string.Format(
                        "Expected property expression of form '${{namespace:identifier}}' got '{0}' instead.", word));

                string @namespace = word.Substring(2, colonPos - 2);
                string identifier = word.Substring(colonPos + 1, word.Length - colonPos - 2);

                return new PropertyConstraint(@namespace, identifier);
            }

            throw new FormatException(string.Format("Did not expect '{0}' in this context.", word));
        }

        /// <summary>
        /// Returns the conditional expression as a string.
        /// </summary>
        /// <returns>The expression string.</returns>
        public override string ToString()
        {
            return expression;
        }

        private abstract class Constraint
        {
            public abstract bool Evaluate(ConditionContext context);
        }

        private sealed class PropertyConstraint : Constraint
        {
            private readonly string @namespace;
            private readonly string identifier;

            public PropertyConstraint(string @namespace, string identifier)
            {
                this.@namespace = @namespace;
                this.identifier = identifier;
            }

            public override bool Evaluate(ConditionContext context)
            {
                return context.HasProperty(@namespace, identifier);
            }
        }

        private sealed class OrConstraint : Constraint
        {
            private readonly Constraint left;
            private readonly Constraint right;

            public OrConstraint(Constraint left, Constraint right)
            {
                this.left = left;
                this.right = right;
            }

            public override bool Evaluate(ConditionContext context)
            {
                return left.Evaluate(context) || right.Evaluate(context);
            }
        }
    }
}
