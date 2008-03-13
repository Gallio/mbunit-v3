// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Sets the maximum amount of time that a test or fixture is permitted to run.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method,
        AllowMultiple = false, Inherited = true)]
    public class TimeoutAttribute : TestDecoratorPatternAttribute
    {
        private readonly int timeoutSeconds;

        /// <summary>
        /// Sets the test timeout in seconds.
        /// </summary>
        /// <param name="timeoutSeconds">The timeout in seconds</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeoutSeconds"/> is less than 1</exception>
        public TimeoutAttribute(int timeoutSeconds)
        {
            if (timeoutSeconds < 1)
                throw new ArgumentOutOfRangeException("timeoutSeconds", "The timeout must be at least 1 second.");

            this.timeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// Gets the timeout in seconds.
        /// </summary>
        public int TimeoutSeconds
        {
            get { return timeoutSeconds; }
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternTestBuilder builder, ICodeElementInfo codeElement)
        {
            builder.Test.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        }
    }
}
