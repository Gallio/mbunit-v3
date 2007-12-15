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
using MbUnit.Model;

namespace MbUnit.Model
{
    /// <summary>
    /// Captures the run-time state of a test.
    /// </summary>
    /// <todo author="jeff">
    /// Roll this stuff into the Context possibly or create new interfaces for it.
    /// </todo>
    public class MbUnitTestState
    {
        private MbUnitTest test;
        private object fixtureInstance;

        /// <summary>
        /// Creates the state for a test.
        /// </summary>
        /// <param name="test">The test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        public MbUnitTestState(MbUnitTest test)
        {
            if (test == null)
                throw new ArgumentNullException(@"test");

            this.test = test;
        }

        /// <summary>
        /// Gets the test.
        /// </summary>
        public MbUnitTest Test
        {
            get { return test; }
        }

        /// <summary>
        /// Gets or sets the test fixture instance or null if none.
        /// </summary>
        public object FixtureInstance
        {
            get { return fixtureInstance; }
            set { fixtureInstance = value; }
        }
    }
}