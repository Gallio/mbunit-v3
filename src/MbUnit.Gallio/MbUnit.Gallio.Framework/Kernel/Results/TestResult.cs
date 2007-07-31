// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Results
{
    /// <summary>
    /// A test result describes the final result of having executed a test.
    /// </summary>
    [Serializable]
    public class TestResult
    {
        private TestOutcome outcome;
        private TestState state;
        private TimeSpan duration;

        /// <summary>
        /// Gets or sets the test outcome.
        /// </summary>
        public TestOutcome Outcome
        {
            get { return outcome; }
            set { outcome = value; }
        }

        /// <summary>
        /// Gets or sets the test state.
        /// </summary>
        public TestState State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// Gets or sets the test duration.
        /// </summary>
        public TimeSpan Duration
        {
            get { return duration; }
            set { duration = value; }
        }
    }
}
