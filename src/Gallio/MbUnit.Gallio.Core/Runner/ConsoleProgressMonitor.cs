using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MbUnit.Framework.Kernel.Events;

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
    public class ConsoleProgressMonitor : TrackingProgressMonitor
    {
        private int newlinesWritten;

        /// <summary>
        /// Creates a console progress monitor.
        /// </summary>
        public ConsoleProgressMonitor()
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        protected override void OnBeginTask(string taskName, double totalWorkUnits)
        {
            base.OnBeginTask(taskName, totalWorkUnits);

            UpdateDisplay();
        }

        protected override void OnWorked(double workUnits)
        {
            base.OnWorked(workUnits);

            UpdateDisplay();
        }

        protected override void OnDone()
        {
            base.OnDone();

            Console.CancelKeyPress -= Console_CancelKeyPress;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            lock (this)
            {
                //Thread.Sleep(100);

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

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;

            NotifyCanceled();
            UpdateDisplay();
        }
    }
}
