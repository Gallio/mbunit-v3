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
using Castle.Core.Logging;

namespace MbUnit.Core.ConsoleSupport
{
    /// <summary>
    /// A logger that sends all output to the console and displays messages in color
    /// according to their status.
    /// </summary>
    public class SharedConsoleLogger : LevelFilteredLogger
    {
        /// <inheritdoc />
        public override ILogger CreateChildLogger(string name)
        {
            return new SharedConsoleLogger();
        }

        /// <inheritdoc />
        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            lock (SharedConsole.SyncRoot)
            {
                bool oldFooterVisible = SharedConsole.FooterVisible;
                try
                {
                    SharedConsole.FooterVisible = false;

                    if (!SharedConsole.IsRedirected)
                    {
                        switch (level)
                        {
                            case LoggerLevel.Fatal:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;

                            case LoggerLevel.Error:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;

                            case LoggerLevel.Warn:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;

                            case LoggerLevel.Info:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                break;

                            case LoggerLevel.Debug:
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                break;
                        }
                    }

                    Console.WriteLine(message);

                    if (exception != null)
                        Console.WriteLine(Indent(exception.ToString()));

                    if (!SharedConsole.IsRedirected)
                    {
                        Console.ResetColor();
                    }
                }
                finally
                {
                    SharedConsole.FooterVisible = oldFooterVisible;
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