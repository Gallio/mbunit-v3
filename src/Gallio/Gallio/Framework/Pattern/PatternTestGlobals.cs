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
using System.Text;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Provides global runtime configuration values for the pattern test framework during execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Using static variables is a bad idea.  We will probably refactor this and move the configuration
    /// parameters elsewhere.
    /// </para>
    /// </remarks>
    public static class PatternTestGlobals
    {
        private static int degreeOfParallelism;

        static PatternTestGlobals()
        {
            Reset();
        }

        /// <summary>
        /// Specifies the number of concurrent threads to spin up when tests are run in parallel.
        /// </summary>
        /// <value>The degree of parallelism.  Defaults to <see cref="Environment.ProcessorCount" /> or 2, whichever is greater.</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 1</exception>
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
        /// Resets the globals to default values.
        /// </summary>
        public static void Reset()
        {
            DegreeOfParallelism = Math.Max(2, Environment.ProcessorCount);
        }
    }
}
