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
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Indicates that a test should only be run explicitly.
    /// The test will still appear in the test tree but it will not run and it will not appear
    /// in the test results unless it is explicitly selected for execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A test is considered to be explicitly selected when the filter used to run the tests
    /// matches the test or its descendants but none of its ancestors.  For example, if the filter
    /// matches a test case but not its containing test fixture then the test case will be deemed
    /// to be explicitly selected.  Otherwise the test case will be implicitly selected by virtue
    /// of the fact that the filter matched one of its ancestors.
    /// </para>
    /// <para>
    /// This attribute can be used to exclude from normal execution any tests that are
    /// particularly expensive or require manual supervision by an operator.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class ExplicitAttribute : TestDecoratorPatternAttribute
    {
        private readonly string reason;

        /// <summary>
        /// Indicates that this test should only run explicitly without providing a reason.
        /// </summary>
        public ExplicitAttribute()
            : this("")
        {
        }

        /// <summary>
        /// Indicates that this test should only run explicitly and provides a reason.
        /// </summary>
        /// <param name="reason">The reason for which the test should be run explicitly</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reason"/>
        /// is null</exception>
        public ExplicitAttribute(string reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            this.reason = reason;
        }

        /// <summary>
        /// Gets the reason that the test should only run explicitly.
        /// </summary>
        public string Reason
        {
            get { return reason; }
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.AddMetadata(MetadataKeys.ExplicitReason, reason);

            scope.TestBuilder.TestActions.BeforeTestChain.Before(delegate(PatternTestState state)
            {
                if (!state.IsExplicit)
                    throw new SilentTestException(TestOutcome.Explicit);
            });
        }
    }
}
