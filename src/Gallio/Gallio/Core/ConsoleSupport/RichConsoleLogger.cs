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
using Castle.Core.Logging;

namespace Gallio.Core.ConsoleSupport
{
    /// <summary>
    /// A logger that sends all output to the console and displays messages in color
    /// according to their status.
    /// </summary>
    public class RichConsoleLogger : LevelFilteredLogger
    {
        private readonly IRichConsole console;

        /// <summary>
        /// Creates a logger.
        /// </summary>
        /// <param name="console">The console</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="console"/> is null</exception>
        public RichConsoleLogger(IRichConsole console)
        {
            if (console == null)
                throw new ArgumentNullException(@"console");

            this.console = console;
        }

        /// <inheritdoc />
        public override ILogger CreateChildLogger(string name)
        {
            return new RichConsoleLogger(console);
        }

        /// <inheritdoc />
        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            lock (console.SyncRoot)
            {
                bool oldFooterVisible = console.FooterVisible;
                try
                {
                    console.FooterVisible = false;

                    if (!console.IsRedirected)
                    {
                        switch (level)
                        {
                            case LoggerLevel.Fatal:
                            case LoggerLevel.Error:
                                console.ForegroundColor = ConsoleColor.Red;
                                break;

                            case LoggerLevel.Warn:
                                console.ForegroundColor = ConsoleColor.Yellow;
                                break;

                            case LoggerLevel.Info:
                                console.ForegroundColor = ConsoleColor.Gray;
                                break;

                            case LoggerLevel.Debug:
                                console.ForegroundColor = ConsoleColor.DarkGray;
                                break;
                        }
                    }

                    console.WriteLine(message);

                    if (exception != null)
                        console.WriteLine(Indent(exception.ToString()));

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