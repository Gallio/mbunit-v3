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
using Gallio.Common.Diagnostics;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.ConsoleSupport
{
    /// <summary>
    /// A logger that sends all output to the console and displays messages in color
    /// according to their status.
    /// </summary>
    public class RichConsoleLogger : BaseLogger
    {
        private readonly IRichConsole console;

        /// <summary>
        /// Creates a logger.
        /// </summary>
        /// <param name="console">The console.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="console"/> is null.</exception>
        public RichConsoleLogger(IRichConsole console)
        {
            if (console == null)
                throw new ArgumentNullException(@"console");

            this.console = console;
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            lock (console.SyncRoot)
            {
                bool oldFooterVisible = console.FooterVisible;
                try
                {
                    console.FooterVisible = false;

                    if (!console.IsRedirected)
                    {
                        switch (severity)
                        {
                            case LogSeverity.Error:
                                console.ForegroundColor = ConsoleColor.Red;
                                break;

                            case LogSeverity.Warning:
                                console.ForegroundColor = ConsoleColor.Yellow;
                                break;

                            case LogSeverity.Important:
                                console.ForegroundColor = ConsoleColor.White;
                                break;

                            case LogSeverity.Info:
                                console.ForegroundColor = ConsoleColor.Gray;
                                break;

                            case LogSeverity.Debug:
                                console.ForegroundColor = ConsoleColor.DarkGray;
                                break;
                        }
                    }

                    console.WriteLine(message);

                    if (exceptionData != null)
                        console.WriteLine(Indent(exceptionData.ToString()));

                    if (!console.IsRedirected)
                    {
                        console.ResetColor();
                    }
                }
                finally
                {
                    console.FooterVisible = oldFooterVisible;
                }
            }
        }

        private static string Indent(string message)
        {
            if (message.Length == 0)
                return @"";

            return "\t" + message.Replace("\n", "\n\t");
        }
    }
}