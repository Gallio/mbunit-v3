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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Configures runtime parameters while executing tests within a test assembly.
    /// </summary>
    public static class TestAssemblyExecutionParameters
    {
        private static int degreeOfParallelism;
        private static TimeSpan? defaultTestCaseTimeout;

        static TestAssemblyExecutionParameters()
        {
            Reset();
        }

        /// <summary>
        /// Specifies the maximum number of concurrent threads to use when tests are run in parallel.
        /// </summary>
        /// <value>The degree of parallelism.  Defaults to <see cref="Environment.ProcessorCount" /> or 2, whichever is greater.</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 1.</exception>
        public static int DegreeOfParallelism
        {
            get { return degreeOfParallelism; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "Degree of parallelism must be at least 1.");
                degreeOfParallelism = value;
            }
        }

        /// <summary>
        /// Specifies the default test case timeout or null if none.
        /// </summary>
        /// <value>The default timeout for test cases or null if none.  Defaults to 10 minutes.</value>
        public static TimeSpan? DefaultTestCaseTimeout
        {
            get { return defaultTestCaseTimeout; }
            set
            {
                if (value.HasValue && value.Value.Ticks < 0)
                    throw new ArgumentOutOfRangeException("value", "Default test case timeout must be non-negative or null.");
                defaultTestCaseTimeout = value;
            }
        }

        /// <summary>
        /// Resets the globals to default values.
        /// </summary>
        public static void Reset()
        {
            DegreeOfParallelism = Math.Max(2, Environment.ProcessorCount);
            DefaultTestCaseTimeout = TimeSpan.FromMinutes(10);
        }
    }
}
