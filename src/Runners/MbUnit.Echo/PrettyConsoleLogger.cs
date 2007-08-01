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
using Castle.Core.Logging;

namespace MbUnit.Echo
{
    public class PrettyConsoleLogger : ConsoleLogger
    {
        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            switch (level)
            {
                case LoggerLevel.Fatal:
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

            Console.WriteLine(message);

            if (exception != null)
                Console.WriteLine(Indent(exception.ToString()));

            Console.ResetColor();
        }

        private string Indent(string message)
        {
            if (message.Length == 0)
                return "";

            return "\t" + message.Replace("\n", "\n\t");
        }
    }
}
