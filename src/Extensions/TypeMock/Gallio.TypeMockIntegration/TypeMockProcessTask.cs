// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Diagnostics;
using Gallio.Concurrency;
using TypeMock.Integration;

namespace Gallio.TypeMockIntegration
{
    /// <summary>
    /// A <see cref="ProcessTask" /> that uses the TypeMock integration
    /// <see cref="TypeMockProcess" /> feature to launch hosting process
    /// with TypeMock attached.
    /// </summary>
    public class TypeMockProcessTask : ProcessTask
    {
        /// <summary>
        /// Creates a process task.
        /// </summary>
        /// <param name="executablePath">The path of the executable executable</param>
        /// <param name="arguments">The arguments for the executable</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>
        /// or <paramref name="arguments"/> is null</exception>
        public TypeMockProcessTask(string executablePath, string arguments)
            : base(executablePath, arguments)
        {
        }
        
        /// <inheritdoc />
        protected override Process StartProcess(ProcessStartInfo startInfo)
        {
            return TypeMockProcess.Start(startInfo);
        }
    }
}
