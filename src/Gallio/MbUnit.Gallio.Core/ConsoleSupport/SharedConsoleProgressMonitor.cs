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
using MbUnit.Core.Runner;

namespace MbUnit.Core.ConsoleSupport
{
    /// <summary>
    /// A console progress monitor displays a simple tally of the amount of work
    /// to be done on the main task as a bar chart.  The progress monitor responds
    /// to cancelation events at the console.
    /// </summary>
    public class SharedConsoleProgressMonitor : TextualProgressMonitor
    {
        private int newlinesWritten;
        private bool bannerPrinted;

        /// <summary>
        /// Creates a console progress monitor.
        /// </summary>
        public SharedConsoleProgressMonitor()
        {
            SharedConsole.Cancel += HandleCancel;
        }

        /// <inheritdoc />
        protected override void OnDone()
        {
            SharedConsole.Cancel -= HandleCancel;
            base.OnDone();
        }

        /// <inheritdoc />
        protected override void UpdateDisplay()
        {
            lock (SharedConsole.SyncRoot)
            {
                ShowTaskBeginningBanner();

                // Don't try to show real-time progress if output is redirected.
                // It can't work because we can't erase previously written text.
                if (SharedConsole.IsRedirected)
                    return;

                if (IsDone)
                    SharedConsole.SetFooter(null, null);
                else
                    SharedConsole.SetFooter(ShowFooter, HideFooter);
            }
        }

        private void ShowTaskBeginningBanner()
        {
            // Just print the task name once.
            if (!bannerPrinted)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(TrimMessageWithEllipses(TaskName, Console.BufferWidth - 1));
                Console.ResetColor();

                bannerPrinted = true;
            }
        }

        private void ShowFooter()
        {
            NewLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(TrimMessageWithEllipses(TaskName, Console.BufferWidth - 20).PadRight(Console.BufferWidth - 19));

            if (!IsDone && !double.IsNaN(TotalWorkUnits))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write('[');
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(new string('=', (int)Math.Round(CompletedWorkUnits * 10 / TotalWorkUnits)).PadRight(10));
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(@"] ");
                Console.Write(Math.Floor(CompletedWorkUnits * 100 / TotalWorkUnits));
                Console.Write('%');
                NewLine();

                if (CurrentSubTaskName.Length != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(@"  ");
                    Console.Write(TrimMessageWithEllipses(CurrentSubTaskName, Console.BufferWidth - 3));
                    NewLine();
                }

                if (Status.Length != 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(@"  ");
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
                Console.Write(@"    ");
                Console.Write(IsDone ? Resources.ConsoleProgressMonitor_CanceledBanner : Resources.ConsoleProgressMonitor_CancelingBanner);
                NewLine();
            }

            Console.ResetColor();
        }

        private void HideFooter()
        {
            EraseLine();
            for (; newlinesWritten > 0; newlinesWritten -= 1)
            {
                Console.CursorTop -= 1;
                EraseLine();
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
                return str.Substring(0, length - 3) + @"...";

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