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
using Gallio.Model.Isolation;
using Gallio.NCoverIntegration.Tools;
using Gallio.Runtime;
using Gallio.Runtime.Logging;

namespace Gallio.NCoverIntegration
{
    /// <summary>
    /// An NCover test isolation provider runs tests in separate processes with NCover
    /// attached then merges the result.
    /// </summary>
    public class NCoverTestIsolationProvider : BaseTestIsolationProvider
    {
        private readonly IRuntime runtime;
        private readonly NCoverVersion version;

        /// <summary>
        /// Initializes the isolation provider.
        /// </summary>
        /// <param name="runtime">The runtime.</param>
        /// <param name="version">The NCover version.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null.</exception>
        public NCoverTestIsolationProvider(IRuntime runtime, NCoverVersion version)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.runtime = runtime;
            this.version = version;
        }

        /// <inheritdoc />
        protected override ITestIsolationContext CreateContextImpl(TestIsolationOptions testIsolationOptions, ILogger logger)
        {
            NCoverTool tool = NCoverTool.GetInstance(version);
            return new NCoverTestIsolationContext(testIsolationOptions, logger, runtime, tool);
        }
    }
}
