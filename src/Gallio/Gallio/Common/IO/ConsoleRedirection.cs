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
using Gallio.Common.Platform;

namespace Gallio.Common.IO
{
    /// <summary>
    /// Redirects the console streams.
    /// </summary>
    public class ConsoleRedirection : IDisposable
    {
        private TextReader oldConsoleIn;
        private TextWriter oldConsoleOut;
        private TextWriter oldConsoleError;

        /// <summary>
        /// Redirect console output and error stream and disables the console input stream.
        /// </summary>
        /// <param name="consoleOut">The new console output writer.</param>
        /// <param name="consoleError">The new console error writer.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="consoleOut"/>
        /// or <paramref name="consoleError"/> is null.</exception>
        public ConsoleRedirection(TextWriter consoleOut, TextWriter consoleError)
        {
            if (consoleOut == null)
                throw new ArgumentNullException("consoleOut");
            if (consoleError == null)
                throw new ArgumentNullException("consoleError");

            // Save the old console streams.
            oldConsoleIn = Console.In;
            oldConsoleOut = Console.Out;
            oldConsoleError = Console.Error;

            // Inject console streams.
            if (! DotNetRuntimeSupport.IsUsingMono)
                Console.SetIn(TextReader.Null);

            Console.SetOut(consoleOut);
            Console.SetError(consoleError);
        }

        /// <summary>
        /// Resets the console streams as they were initially.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Resets the console streams as they were initially.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()"/> was called directly.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
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