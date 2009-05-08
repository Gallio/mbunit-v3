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
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Indicates that a test has pending prerequisites so it will not be run.
    /// The test will still appear in test reports along with an explanation of the
    /// reason it it pending, if provided.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute can be used to disable tests that cannot run because the
    /// subject under test is missing certain prerequisite functionality.  It may
    /// also serve as a placeholder for test that have yet to be implemented.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class PendingAttribute : TestDecoratorPatternAttribute
    {
        private readonly string reason;

        /// <summary>
        /// Indicates that this test has pending prerequisites without providing a reason.
        /// </summary>
        public PendingAttribute()
            : this("")
        {
        }

        /// <summary>
        /// Indicates that this test has pending prerequisites and provides a reason.
        /// </summary>
        /// <param name="reason">The reason for which the test is pending</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reason"/>
        /// is null</exception>
        public PendingAttribute(string reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            this.reason = reason;
        }

        /// <summary>
        /// Gets the reason that the test is pending.
        /// </summary>
        public string Reason
        {
            get { return reason; }
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            string message = "The test is pending implementation.";
            if (reason.Length != 0)
                message += "\nReason: " + reason;
            
            scope.TestBuilder.AddMetadata(MetadataKeys.PendingReason, reason);

            scope.TestBuilder.TestActions.InitializeTestChain.Before(delegate(PatternTestState state)
            {
                if (! state.IsExplicit)
                    throw new SilentTestException(TestOutcome.Pending, message);
            });

            scope.TestModelBuilder.AddAnnotation(new Annotation(AnnotationType.Warning, codeElement, message));
        }
    }
}