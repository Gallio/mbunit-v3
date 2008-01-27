// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Explorer;
using Gallio.Framework.Patterns;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Indicates that a test is to be ignored by the framework and will not be run.
    /// The test will still appear in test reports along with the reason that it
    /// was ignored, if provided.
    /// </para>
    /// <para>
    /// This attribute can be used to disable tests that are broken or expensive
    /// without commenting them out or removing them from the source code.
    /// </para>
    /// </summary>
    public class IgnoreAttribute : TestDecoratorPatternAttribute
    {
        private readonly string reason;

        /// <summary>
        /// Indicates that this test is to be ignored without providing a reason.
        /// </summary>
        public IgnoreAttribute()
            : this("")
        {
        }

        /// <summary>
        /// Indicates that this test is to be ignored and provides a reason.
        /// </summary>
        /// <param name="reason">The reason for which the test is to be ignored</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reason"/>
        /// is null</exception>
        public IgnoreAttribute(string reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            this.reason = reason;
        }

        /// <summary>
        /// Gets the reason that the test has been ignored.
        /// </summary>
        public string Reason
        {
            get { return reason; }
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternTestBuilder builder, ICodeElementInfo codeElement)
        {
            builder.Test.Metadata.Add(MetadataKeys.IgnoreReason, reason);
        }
    }
}
