// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Contexts;
using Gallio.Hosting;
using Gallio.Logging;

namespace Gallio.Runner.Harness
{
    /// <summary>
    /// Logs unhandled exceptions instead of killing the AppDomain.
    /// </summary>
    public class UnhandledExceptionTestEnvironment : ITestEnvironment
    {
        /// <inheritdoc />
        public IDisposable SetUp()
        {
            return new State();
        }

        private sealed class State : IDisposable
        {
            public State()
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }

            public void Dispose()
            {
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            }

            void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                try
                {
                    Exception ex = e.ExceptionObject as Exception;
                    if (ex != null)
                        Context.CurrentContext.LogWriter[LogStreamNames.Warnings].WriteException(ex, "An unhandled exception occurred.");
                }
                catch (Exception ex)
                {
                    Panic.UnhandledException("An exception occurred while attempting to log an unhandled exception in the AppDomain.", ex);
                }
            }
        }
    }
}