// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Gallio.Common.Time
{
    /// <summary>
    /// Utility class for absolute and relative time management.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Resets the time and starts the clock.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the clock.
        /// </summary>
        void Stop();

        /// <summary>
        /// Gets the time elapsed.
        /// </summary>
        TimeSpan Elapsed
        {
            get;
        }

        /// <summary>
        /// Returns the local date and time.
        /// </summary>
        DateTime Now
        {
            get;
        }

        /// <summary>
        /// Returns the current date and time, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        DateTime UtcNow
        {
            get;
        }

        /// <summary>
        /// Blocks the current thread for a specified time.
        /// </summary>
        /// <param name="millisecondsTimeout">The amount of time for which the thread is blocked.</param>
        /// <seealso cref="Thread.Sleep(int)"/>
        void ThreadSleep(int millisecondsTimeout);

        /// <summary>
        /// Blocks the current thread for a specified time.
        /// </summary>
        /// <param name="timeout">The amount of time for which the thread is blocked.</param>
        /// <seealso cref="Thread.Sleep(TimeSpan)"/>
        void ThreadSleep(TimeSpan timeout);
    }
}
