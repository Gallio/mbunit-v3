// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using System.IO;
using Gallio;
using Gallio.Utilities;

namespace Gallio.Runtime.ConsoleSupport
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="IRichConsole" /> that targets the
    /// native <see cref="Console" />.
    /// </para>
    /// <para>
    /// This implementation offers protection against redirection of the <see cref="Console.Out" />
    /// and <see cref="Console.Error" /> streams.  This object will continue to refer to the
    /// standard output and error streams even if they are redirected after its initialization.
    /// </para>
    /// </summary>
    public sealed class NativeConsole : IRichConsole
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

        private static readonly NativeConsole instance = new NativeConsole();

        private readonly TextWriter standardOutput;
        private readonly TextWriter standardError;

        private readonly object syncRoot = new object();

        private bool isCancelationEnabled;

        private bool isCanceled;
        private event EventHandler cancelHandlers;

        private Stopwatch repeatStart;
        private int repeatCount;

        private bool footerVisible = true;
        private Action showFooter;
        private Action hideFooter;

        private int redirectedFlag;

        private NativeConsole()
        {
            // FIXME: Assuming that the NativeConsole gets created before any output redirection occurs.
            //        If redirection has occurred, then we would need to use Console.OpenStandardOutput
            //        and Console.OpenStandardError to recover the original streams.  However, this is
            //        not an issue at this time.
            standardOutput = Console.Out;
            standardError = Console.Error;
        }

        /// <summary>
        /// Gets the singleton instance of the native console.
        /// </summary>
        public static NativeConsole Instance
        {
            get { return instance; }
        }

        /// <inheritdoc />
        public bool IsCancelationEnabled
        {
            get { return isCancelationEnabled; }
            set
            {
                lock (syncRoot)
                {
                    if (IsRedirected || isCancelationEnabled == value)
                        return;

                    if (value)
                    {
                        Console.TreatControlCAsInput = false;
                        Console.CancelKeyPress += HandleCancelKeyPress;
                        isCancelationEnabled = true;
                    }
                    else
                    {
                        Console.CancelKeyPress -= HandleCancelKeyPress;
                        isCancelationEnabled = false;
                    }
                }
            }
        }
        
        /// <inheritdoc />
        public object SyncRoot
        {
            get { return syncRoot; }
        }

        /// <inheritdoc />
        public event EventHandler Cancel
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

        /// <inheritdoc />
        public bool IsCanceled
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

                EventHandlerUtils.SafeInvoke(currentCancelHandlers, null, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public bool IsRedirected
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

        /// <inheritdoc />
        public TextWriter Error
        {
            get { return standardError; }
        }

        /// <inheritdoc />
        public TextWriter Out
        {
            get { return standardOutput; }
        }

        /// <inheritdoc />
        public ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        /// <inheritdoc />
        public int CursorLeft
        {
            get { return Console.CursorLeft; }
            set { Console.CursorLeft = value; }
        }

        /// <inheritdoc />
        public int CursorTop
        {
            get { return Console.CursorTop; }
            set { Console.CursorTop = value; }
        }

        /// <inheritdoc />
        public string Title
        {
            get { return Console.Title; }
            set { Console.Title = value; }
        }

        /// <inheritdoc />
        public int Width
        {
            get
            {
                return IsRedirected ? 80 : Console.BufferWidth;
            }
        }


        /// <inheritdoc />
        public bool FooterVisible
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

        /// <inheritdoc />
        public void ResetColor()
        {
            Console.ResetColor();
        }

        /// <inheritdoc />
        public void SetFooter(Action showFooter, Action hideFooter)
        {
            lock (syncRoot)
            {
                if (footerVisible && this.hideFooter != null)
                    this.hideFooter();

                this.showFooter = showFooter;
                this.hideFooter = hideFooter;

                if (footerVisible && showFooter != null)
                    showFooter();
            }
        }

        /// <inheritdoc />
        public void Write(char c)
        {
            standardOutput.Write(c);
        }

        /// <inheritdoc />
        public void Write(string str)
        {
            standardOutput.Write(str);
        }

        /// <inheritdoc />
        public void WriteLine()
        {
            standardOutput.WriteLine();
        }

        /// <inheritdoc />
        public void WriteLine(string str)
        {
            standardOutput.WriteLine(str);
        }

        private void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
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