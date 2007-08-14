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

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A console progress monitor displays a simple tally of the amount of work
    /// to be done on the main task as a bar chart.
    /// </summary>
    /// <remarks author="jeff">
    /// This is an initial hack to provide an example for development of MbUnit Echo.
    /// Feel free to rip it apart then remove this remark.
    /// </remarks>
    public class ConsoleProgressMonitor : TextualProgressMonitor
    {
        private int newlinesWritten;

        /// <summary>
        /// Creates a console progress monitor.
        /// </summary>
        public ConsoleProgressMonitor()
        {
            ConsoleCancelHandler.Cancel += HandleCancel;
        }

        /// <inheritdoc />
        protected override void OnDone()
        {
            ConsoleCancelHandler.Cancel -= HandleCancel;
            base.OnDone();
        }

        /// <inheritdoc />
        protected override void UpdateDisplay()
        {
            lock (this)
            {
                EraseLine();
                for (; newlinesWritten > 0; newlinesWritten -= 1)
                {
                    Console.CursorTop -= 1;
                    EraseLine();
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(TrimMessageWithEllipses(TaskName, Console.BufferWidth - 20).PadRight(Console.BufferWidth - 19));

                if (!IsDone && !double.IsNaN(TotalWorkUnits))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write('[');
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(new string('=', (int)Math.Round(CompletedWorkUnits * 10 / TotalWorkUnits)).PadRight(10));
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(']');
                    Console.Write(" {0}%", Math.Floor(CompletedWorkUnits * 100 / TotalWorkUnits));
                    NewLine();

                    if (CurrentSubTaskName.Length != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("  ");
                        Console.Write(TrimMessageWithEllipses(CurrentSubTaskName, Console.BufferWidth - 3));
                        NewLine();
                    }

                    if (Status.Length != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("  ");
                        Console.Write(TrimMessageWithEllipses(Status, Console.BufferWidth - 3));
                        NewLine();
                    }
                }
                else
                {
                    NewLine();
                }

                if (IsCanceled)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(IsDone ? "    --- CANCELED ---" : "    >>> CANCELING <<<");
                    NewLine();
                }

                Console.ResetColor();
            }
        }

        private void NewLine()
        {
            newlinesWritten += 1;
            Console.WriteLine();
        }

        private static string TrimMessageWithEllipses(string str, int length)
        {
            if (str.Length > length)
                return str.Substring(0, length - 3) + "...";

            return str;
        }

        private static void EraseLine()
        {
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.BufferWidth));
            Console.CursorLeft = 0;
            Console.CursorTop -= 1;
        }

        private void HandleCancel(object sender, EventArgs e)
        {
            NotifyCanceled();
            UpdateDisplay();
        }
    }
}
