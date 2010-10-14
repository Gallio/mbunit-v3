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
using System.Diagnostics;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Provides information about the context in which the Shell is running.
    /// </summary>
    public static class ShellEnvironment
    {
        /// <summary>
        /// Gets whether the Shell is running in Visual Studio.
        /// </summary>
        /// <remarks>
        /// This property can be used to determine whether Shell components are running
        /// inside of Visual Studio or if they have been invoked from some other context.
        /// For example, the Gallio Test Integration Provider can be called by MSTest
        /// or Visual Studio.  When called by MSTest, the Shell package and add-in are not
        /// initialized so neither are the shell extensions and other standard features.
        /// </remarks>
        public static bool IsRunningInVisualStudio
        {
            get
            {
                return Process.GetCurrentProcess().ProcessName
                    .Equals("devenv", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
