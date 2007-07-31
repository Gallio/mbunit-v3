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

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// Common event arguments superclass for events pertaining to a particular test.
    /// </summary>
    [Serializable]
    public class TestEventArgs : EventArgs
    {
        private string testId;

        /// <summary>
        /// Creates a test event.
        /// </summary>
        /// <param name="testId">The id of the test this event is about</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> is null</exception>
        public TestEventArgs(string testId)
        {
            if (testId == null)
                throw new ArgumentNullException("testId");

            this.testId = testId;
        }

        /// <summary>
        /// Gets the id of the test this event is about.
        /// </summary>
        public string TestId
        {
            get { return testId; }
        }
    }
}
