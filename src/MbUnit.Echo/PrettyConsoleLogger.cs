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
