// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using System.Text;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// An event dispatched by the <see cref="ProcessTask"/> when its
    /// <see cref="ProcessStartInfo" /> is being configured to enable customization.
    /// </summary>
    public class ConfigureProcessStartInfoEventArgs : EventArgs
    {
        private readonly ProcessStartInfo processStartInfo;

        /// <summary>
        /// Creates event arguments.
        /// </summary>
        /// <param name="processStartInfo">The process start info being configured.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="processStartInfo"/> is null.</exception>
        public ConfigureProcessStartInfoEventArgs(ProcessStartInfo processStartInfo)
        {
            if (processStartInfo == null)
                throw new ArgumentNullException("processStartInfo");

            this.processStartInfo = processStartInfo;
        }

        /// <summary>
        /// Gets the process start info being configured.
        /// </summary>
        public ProcessStartInfo ProcessStartInfo
        {
            get { return processStartInfo; }
        }
    }
}
