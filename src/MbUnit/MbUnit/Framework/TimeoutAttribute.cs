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
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Sets the maximum amount of time in seconds that a test or fixture is permitted to run.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default timeout is unlimited for test fixtures and 10 minutes for test methods.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class TimeoutAttribute : TestDecoratorPatternAttribute
    {
        private readonly int timeoutSeconds;

        /// <summary>
        /// Sets the test timeout in seconds, or zero if none.
        /// </summary>
        /// <param name="timeoutSeconds">The timeout in seconds, or zero if none.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeoutSeconds"/> is less than 0.</exception>
        public TimeoutAttribute(int timeoutSeconds)
        {
            if (timeoutSeconds < 0)
                throw new ArgumentOutOfRangeException("timeoutSeconds", "The timeout must be zero or at least 1 second.");

            this.timeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// Gets the test timeout in seconds, or zero if none.
        /// </summary>
        public int TimeoutSeconds
        {
            get { return timeoutSeconds; }
        }

        /// <summary>
        /// Gets the timeout, or null if none.
        /// </summary>
        public TimeSpan? Timeout
        {
            get { return timeoutSeconds == 0 ? (TimeSpan?)null : TimeSpan.FromSeconds(timeoutSeconds); }
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.Timeout = Timeout;
        }
    }
}
