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

using System;
using Castle.Core.Logging;
using Gallio.Concurrency;
using Gallio.Hosting;

namespace Gallio.TypeMockIntegration
{
    /// <summary>
    /// A TypeMock host is a variation on a <see cref="IsolatedProcessHost" />
    /// launches the process with the TypeMock.Net tool attached.
    /// </summary>
    public class TypeMockHost : IsolatedProcessHost
    {
        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <param name="logger">The logger for host message output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/> 
        /// or <paramref name="logger"/> is null</exception>
        public TypeMockHost(HostSetup hostSetup, ILogger logger)
            : base(hostSetup, logger)
        {
        }

        /// <inheritdoc />
        protected override ProcessTask CreateProcessTask(string executablePath, string arguments, string workingDirectory)
        {
            return new TypeMockProcessTask(executablePath, arguments, workingDirectory);
        }
    }
}
