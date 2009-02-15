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
using System.IO;
using Gallio.Model.Logging;
using Gallio.Runtime;

namespace Gallio.Runner.Harness
{
    /// <summary>
    /// Sets up console I/O streams.
    /// </summary>
    public class ConsoleTestEnvironment : ITestEnvironment
    {
        /// <inheritdoc />
        public IDisposable SetUp()
        {
            return new State();
        }

        private sealed class State : IDisposable
        {
            private TextReader oldConsoleIn;
            private TextWriter oldConsoleOut;
            private TextWriter oldConsoleError;

            public State()
            {
                // Save the old console streams.
                oldConsoleIn = Console.In;
                oldConsoleOut = Console.Out;
                oldConsoleError = Console.Error;

                // Inject console streams.
                if (! RuntimeDetection.IsUsingMono)
                    Console.SetIn(TextReader.Null);

                Console.SetOut(new ContextualLogTextWriter(TestLogStreamNames.ConsoleOutput));
                Console.SetError(new ContextualLogTextWriter(TestLogStreamNames.ConsoleError));
            }

            public void Dispose()
            {
                // Restore the old console streams.
                if (oldConsoleIn != null)
                {
                    Console.SetIn(oldConsoleIn);
                    oldConsoleIn = null;
                }

                if (oldConsoleOut != null)
                {
                    Console.SetOut(oldConsoleOut);
                    oldConsoleOut = null;
                }

                if (oldConsoleError != null)
                {
                    Console.SetError(oldConsoleError);
                    oldConsoleError = null;
                }
            }
        }
    }
}
