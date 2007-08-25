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
using System.Text;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Encapsulates concerns relating to handling console-based
    /// cancelation events.  
    /// </summary>
    public static class ConsoleCancelHandler
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

        private static bool isCanceled;
        private static event EventHandler cancelHandlers;

        private static Stopwatch repeatStart;
        private static int repeatCount;

        /// <summary>
        /// Installs the handler.
        /// </summary>
        public static void Install()
        {
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += HandleCancelKeyPress;
        }

        /// <summary>
        /// Raised when console cancelation occurs.
        /// If the console cancelation signal is already set when
        /// an event handler is added, the event handler will be
        /// automatically invoked.
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
        /// Gets or sets whether cancelation has occurred.
        /// The <see cref="Cancel" /> event handlers will be called
        /// when the value of <see cref="IsCanceled" /> transitions from
        /// false to true.  The value will remain true unless it is reset.
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
