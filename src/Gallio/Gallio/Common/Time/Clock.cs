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
using System.Diagnostics;
using System.Threading;

namespace Gallio.Common.Time
{
    /// <summary>
    /// Utility class for absolute and relative time management.
    /// </summary>
    public sealed class Clock : IClock
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <inheritdoc />
        public void Start()
        {
            stopwatch.Start();
        }

        /// <inheritdoc />
        public void Stop()
        {
            stopwatch.Stop();
        }

        /// <inheritdoc />
        public TimeSpan Elapsed
        {
            get
            {
                return TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
            }
        }

        /// <inheritdoc />
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <inheritdoc />
        public DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow;
            }
        }

        /// <inheritdoc />
        public void ThreadSleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }

        /// <inheritdoc />
        public void ThreadSleep(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
        }
    }
}
