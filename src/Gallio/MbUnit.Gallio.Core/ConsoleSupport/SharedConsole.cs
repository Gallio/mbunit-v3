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
using System.Diagnostics;
using System.IO;
using System.Text;
using MbUnit.Framework;

namespace MbUnit.Core.ConsoleSupport
{
    /// <summary>
    /// <para>
    /// A shared console is a wrapper about <see cref="Console" /> that knows how to
    /// share the console among several background tasks so that they can interoperate
    /// smoothly.
    /// </para>
    /// <para>
    /// An example is a progress monitor that wishes to remain visible
    /// at the bottom of the console while other text scrolls past.  Meanwhile it
    /// wishes to listen to cancelation events.
    /// </para>
    /// </summary>
    public static class SharedConsole
    {
        /// <summary>
        /// The application will forcibly terminate if the cancel key is pressed
        /// <see cref="ForceQuitKeyPressRepetitions" /> times within no more than
        /// <see cref="ForceQuitKeyPressTimeoutMilliseconds"/> milliseconds.
        /// </summary>
        public const int ForceQuitKeyPressRepetitions = 3;

        /// <summary>
        /// The application will forcibly terminate if the cancel key is pressed
        /// <see cref="ForceQuitKeyPressRepetitions" /> times within no more than
        /// <see cref="ForceQuitKeyPressTimeoutMilliseconds"/> milliseconds.
        /// </summary>
        public const int ForceQuitKeyPressTimeoutMilliseconds = 1000;

        private static readonly object syncRoot = new object();
        private static bool isCancelationHandlerInstalled;

        private static bool isCanceled;
        private static event EventHandler cancelHandlers;

        private static Stopwatch repeatStart;
        private static int repeatCount;

        private static bool footerVisible = true;
        private static Block showFooter;
        private static Block hideFooter;

        private static int redirectedFlag;

        /// <summary>
        /// Gets a synchronization object that a task can lock to ensure
        /// that it is the only thread currently accessing the console.
        /// </summary>
        public static object SyncRoot
        {
            get { return syncRoot; }
        }

        /// <summary>
        /// <para>
        /// The event raised when console cancelation occurs.
        /// </para>
        /// <para>
        /// If the console cancelation signal is already set when
        /// an event handler is added, the event handler will be
        /// automatically invoked.
        /// </para>
        /// </summary>
        public static event EventHandler Cancel
        {
            add
            {
                lock (syncRoot)
                {
                    cancelHandlers += value;

                    if (!isCanceled)
                        return;
                }

                value(null, EventArgs.Empty);
            }
            remove
            {
                lock (syncRoot)
                {
                    cancelHandlers -= value;
                }
            }
        }

        /// <summary>
        /// Returns true if the console is being redirected and therefore the
        /// output should be as simple as possible.  In particular, it may not be
        /// possible to set the cursor position, console color or other properties.
        /// </summary>
        /// <remarks>
        /// Trying to set the console color while output is redirected is tolerated
        /// by the system and won't cause an error; but it is pointless.  On the other
        /// hand cursor positioning or attempting to listen to Ctrl-C will fail with an
        /// <see cref="IOException" />.
        /// </remarks>
        public static bool IsRedirected
        {
            get
            {
                if (redirectedFlag == 0)
                {
                    try
                    {
                        bool dummy = Console.CursorVisible;
                        redirectedFlag = -1;
                    }
                    catch (IOException)
                    {
                        redirectedFlag = 1;
                    }
                }

                return redirectedFlag > 0;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether cancelation has occurred.
        /// </para>
        /// <para>
        /// The <see cref="Cancel" /> event handlers will be called
        /// when the value of <see cref="IsCanceled" /> transitions from
        /// false to true.  The value will remain true unless it is reset.
        /// </para>
        /// </summary>
        public static bool IsCanceled
        {
            get
            {
                lock (syncRoot)
                    return isCanceled;
            }
            set
            {
                EventHandler currentCancelHandlers;

                lock (syncRoot)
                {
                    if (isCanceled == value)
                        return;

                    isCanceled = value;
                    if (!isCanceled)
                        return;

                    currentCancelHandlers = cancelHandlers;
                }

                if (currentCancelHandlers != null)
                    currentCancelHandlers(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Installs the cancelation handler.
        /// </summary>
        /// <returns>True on success, false if console cancelation is not supported
        /// possibly because the application does not have access to a system console.</returns>
        public static bool InstallCancelationHandler()
        {
            lock (syncRoot)
            {
                if (!isCancelationHandlerInstalled)
                {
                    try
                    {
                        Console.TreatControlCAsInput = false;
                        Console.CancelKeyPress += HandleCancelKeyPress;
                    }
                    catch (IOException)
                    {
                        return false;
                    }

                    isCancelationHandlerInstalled = true;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets whether the footer is visible.
        /// </summary>
        public static bool FooterVisible
        {
            get { return footerVisible; }
            set
            {
                lock (syncRoot)
                {
                    if (footerVisible == value)
                        return;

                    footerVisible = value;

                    if (footerVisible)
                    {
                        if (showFooter != null)
                            showFooter();
                    }
                    else
                    {
                        if (hideFooter != null)
                            hideFooter();
                    }
                }
            }
        }

        /// <summary>
        /// Sets a pair of delegates that together display a footer at the bottom
        /// of the console.  The footer can be hidden so that new text can be written
        /// from that point.  Removes the previous footer and displays the new
        /// one automatically if the footer is visible.
        /// </summary>
        /// <param name="showFooter">A delegate to display the footer</param>
        /// <param name="hideFooter">A delegate to hide the footer, leaving the custor at
        /// the beginning of the line where the footer used to begin</param>
        public static void SetFooter(Block showFooter, Block hideFooter)
        {
            lock (syncRoot)
            {
                if (footerVisible && SharedConsole.hideFooter != null)
                    SharedConsole.hideFooter();

                SharedConsole.showFooter = showFooter;
                SharedConsole.hideFooter = hideFooter;

                if (footerVisible && showFooter != null)
                    showFooter();
            }
        }

        private static void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (repeatCount != 0)
            {
                if (repeatStart.ElapsedMilliseconds > ForceQuitKeyPressTimeoutMilliseconds)
                    repeatCount = 0;
            }

            if (repeatCount == 0)
                repeatStart = Stopwatch.StartNew();

            repeatCount += 1;

            if (repeatCount < ForceQuitKeyPressRepetitions)
                e.Cancel = true;

            IsCanceled = true;
        }
    }
}