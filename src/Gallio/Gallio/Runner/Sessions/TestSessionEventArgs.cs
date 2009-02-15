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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// Event arguments describing an event affecting a test session.
    /// </summary>
    public class TestSessionEventArgs : EventArgs
    {
        /// <summary>
        /// Creates event arguments for a test session event.
        /// </summary>
        /// <param name="session">The test session</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> is null</exception>
        public TestSessionEventArgs(ITestSession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            Session = session;
        }

        /// <summary>
        /// Gets the test session.
        /// </summary>
        public ITestSession Session { get; private set; }
    }
}
