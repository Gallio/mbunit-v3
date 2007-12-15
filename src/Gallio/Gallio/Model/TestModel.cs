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

namespace Gallio.Model
{
    /// <summary>
    /// The test model provides access to the contents of the test tree
    /// generated from a test package by the test enumeration process.
    /// </summary>
    public sealed class TestModel
    {
        private readonly RootTest rootTest;
        private UserDataCollection userData;

        /// <summary>
        /// Creates a test model with a new empty root test.
        /// </summary>
        public TestModel()
            : this(new RootTest())
        {
        }

        /// <summary>
        /// Creates a test model.
        /// </summary>
        /// <param name="rootTest">The root test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rootTest"/> is null</exception>
        public TestModel(RootTest rootTest)
        {
            if (rootTest == null)
                throw new ArgumentNullException(@"rootTest");

            this.rootTest = rootTest;
        }

        /// <summary>
        /// Gets the root test in the model.
        /// </summary>
        public RootTest RootTest
        {
            get { return rootTest; }
        }

        /// <summary>
        /// Gets a dictionary that contains user data that may be used to
        /// store state needed by test frameworks during the construction of
        /// the test model.
        /// </summary>
        public UserDataCollection UserData
        {
            get
            {
                if (userData == null)
                    userData = new UserDataCollection();
                return userData;
            }
        }
    }
}