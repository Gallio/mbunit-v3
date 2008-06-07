// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System.IO;
using Gallio.Runtime.Logging;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// Encapsulates the code for invoking MSTest.
    /// </summary>
    internal interface IMSTestCommand
    {
        /// <summary>
        /// Runs MSTest with the specified arguments.
        /// </summary>
        /// <param name="workingDirectory">The current working directory to use</param>
        /// <param name="args">The command-line arguments</param>
        /// <param name="outputWriter">The text writer to which the output stream should be directed</param>
        /// <param name="errorWriter">The text writer to which the error stream should be directed</param>
        /// <returns>The MSTest exit code, or -1 if MSTest does not appear to be installed</returns>
        int Run(string workingDirectory, MSTestCommandArguments args,
            TextWriter outputWriter, TextWriter errorWriter);
    }
}
