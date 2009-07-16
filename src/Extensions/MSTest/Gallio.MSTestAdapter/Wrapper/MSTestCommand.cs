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
using System.IO;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// Runs an MSTest command.
    /// </summary>
    internal abstract class MSTestCommand
    {
        /// <summary>
        /// Runs MSTest with the specified arguments.
        /// </summary>
        /// <param name="executablePath">The path of MSTest.exe.</param>
        /// <param name="workingDirectory">The current working directory to use.</param>
        /// <param name="args">The command-line arguments.</param>
        /// <param name="writer">The text writer to which the output and error streams should be directed.</param>
        /// <returns>The exit code.</returns>
        public abstract int Run(string executablePath, string workingDirectory, MSTestCommandArguments args, TextWriter writer);
    }
}
