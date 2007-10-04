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
using MbUnit.Core.Properties;
using MbUnit.Core.Runner;

namespace MbUnit.Core.ConsoleSupport
{
    /// <summary>
    /// A console progress monitor displays a simple tally of the amount of work
    /// to be done on the main task as a bar chart.  The progress monitor responds
    /// to cancelation events at the console.
    /// </summary>
    public class RichConsoleProgressMonitor : TextualProgressMonitor
    {
        private readonly IRichConsole console;
        private int newlinesWritten;
        private bool bannerPrinted;
        private int width;
        private bool footerInitialized;

        /// <summary>
        /// Creates a console progress monitor.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="console"/> is null</exception>
        public RichConsoleProgressMonitor(IRichConsole console)
        {
            if (console == null)
                throw new ArgumentNullException(@"console");

            this.console = console;

            console.Cancel += HandleCancel;
        }

        /// <inheritdoc />
        protected override void OnDone()
        {
            console.Cancel -= HandleCancel;

            if (IsCanceled)
                console.IsCanceled = false;

            base.OnDone();
        }

        /// <inheritdoc />
        protected override void UpdateDisplay()
        {
            lock (console.SyncRoot)
            {
                ShowTaskBeginningBanner();

                // Don't try to show real-time progress if output is redirected.
                // It can't work because we can't erase previously written text.
                if (console.IsRedirected)
                    return;

                if (IsDone)
                {
                    footerInitialized = false;
                    console.SetFooter(null, null);
                }
                else if (!footerInitialized)
                {
                    footerInitialized = true;
                    console.SetFooter(ShowFooter, HideFooter);
                }
                else if (console.FooterVisible)
                {
                    RedrawFooter(true);
                }
            }
        }

        private void ShowTaskBeginningBanner()
        {
            // Just print the task name once.
            if (!bannerPrinted)
            {
                width = console.Width;

                console.ForegroundColor = ConsoleColor.White;
                console.WriteLine(TrimMessageWithEllipses(TaskName, width - 1));
                console.ResetColor();

                bannerPrinted = true;
            }
        }

        private void ShowFooter()
        {
            RedrawFooter(false);
        }

        private void HideFooter()
        {
            if (newlinesWritten > 0)
            {
                console.CursorTop -= newlinesWritten;

                for (int i = 0; i < newlinesWritten; i++)
                    EraseLine();

                console.CursorTop -= newlinesWritten;
                newlinesWritten = 0;
            }
        }

        private void RedrawFooter(bool inplace)
        {
            // Scroll enough new lines into view for the text we might want to write.
            // This helps to reduce flickering of the progress monitor.
            // We should still do much better than this if we improve the console API
            // to handle scrolling of independent regions.
            int oldNewlinesWritten = newlinesWritten;
            if (inplace)
            {
                console.CursorTop -= newlinesWritten - 1;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                    console.WriteLine();
                console.CursorTop -= 4;
            }
            newlinesWritten = 1;

            // Write the progress monitor.
            console.ForegroundColor = ConsoleColor.White;
            console.Write(TrimMessageWithEllipses(Sanitize(TaskName), width - 20).PadRight(width - 19));

            if (!IsDone && !double.IsNaN(TotalWorkUnits))
            {
                console.ForegroundColor = ConsoleColor.Yellow;
                console.Write('[');
                console.ForegroundColor = ConsoleColor.DarkYellow;
                console.Write(new string('=', (int)Math.Round(CompletedWorkUnits * 10 / TotalWorkUnits)).PadRight(10));
                console.ForegroundColor = ConsoleColor.Yellow;
                console.Write(@"] ");
                console.Write(Math.Floor(CompletedWorkUnits * 100 / TotalWorkUnits).ToString());
                console.Write('%');
                NewLine(inplace);

                string sanitizedSubTaskName = Sanitize(CurrentSubTaskName);
                if (sanitizedSubTaskName.Length != 0)
                {
                    console.ForegroundColor = ConsoleColor.Gray;
                    console.Write(@"  ");
                    console.Write(TrimMessageWithEllipses(sanitizedSubTaskName, width - 3));
                    NewLine(inplace);
                }

                string sanitizedStatus = Sanitize(Status);
                if (sanitizedStatus.Length != 0)
                {
                    console.ForegroundColor = ConsoleColor.DarkGreen;
                    console.Write(@"  ");
                    console.Write(TrimMessageWithEllipses(sanitizedStatus, width - 3));
                    NewLine(inplace);
                }
            }
            else
                NewLine(inplace);

            if (IsCanceled)
            {
                console.ForegroundColor = ConsoleColor.Red;
                console.Write(@"    ");
                console.Write(IsDone ? Resources.ConsoleProgressMonitor_CanceledBanner : Resources.ConsoleProgressMonitor_CancelingBanner);
                NewLine(inplace);
            }

            console.ResetColor();

            // Clear out the remaining dirty lines in place.
            if (inplace && oldNewlinesWritten > newlinesWritten)
            {
                int dirtyLines = oldNewlinesWritten - newlinesWritten;
                for (int i = 0; i < dirtyLines; i++)
                    EraseLine();

                console.CursorTop -= dirtyLines;
            }
        }

        private void NewLine(bool inplace)
        {
            newlinesWritten += 1;

            if (inplace)
                console.Write(new string(' ', width - console.CursorLeft));
            else
                console.WriteLine();
        }

        private void EraseLine()
        {
            console.CursorLeft = 0;
            console.Write(new string(' ', width));
        }

        private void HandleCancel(object sender, EventArgs e)
        {
            NotifyCanceled();
            UpdateDisplay();
        }

        private static string TrimMessageWithEllipses(string str, int length)
        {
            if (str.Length > length)
                return str.Substring(0, length - 3) + @"...";

            return str;
        }

        /// <summary>
        /// It can happen that we'll receive all kinds of weird input because
        /// task names and status messages can be derived from user-data.
        /// Make sure it doesn't corrupt the display integrity.
        /// </summary>
        private static string Sanitize(string str)
        {
            return str.Replace('\n', ' ').Replace("\r", @"");
        }
    }
}