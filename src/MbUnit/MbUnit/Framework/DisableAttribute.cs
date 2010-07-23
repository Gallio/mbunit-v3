// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies that a test fixture or a test method is abstract and should not run.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The attribute hides the test methods in the current test fixture or a test method itself, 
    /// so they can be consumed properly in the desired context from any derived concrete test fixtures.
    /// </para>
    /// <para>
    /// This attribute is not inherited.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = false)]
    public class DisableAttribute : TestDecoratorPatternAttribute
    {
        private readonly string reason;

        /// <summary>
        /// Gets the reason that the test method/fixture is disabled.
        /// </summary>
        public string Reason
        {
            get
            {
                return reason;
            }
        }


        /// <summary>
        /// Indicates that this test method/fixture is disabled.
        /// </summary>
        public DisableAttribute()
            : this("Explicitly disabled test method/fixture.")
        {
        }

        /// <summary>
        ///  Indicates that this test method/fixture is disabled and provides a reason.
        /// </summary>
        /// <param name="reason">The reason for which the test method/fixture is disabled.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reason"/> is null.</exception>
        public DisableAttribute(string reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            this.reason = reason;
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.TestActions.BeforeTestChain.Before(state =>
            {
                throw new SilentTestException(TestOutcome.Skipped, reason);
            });
        }
    }
}
