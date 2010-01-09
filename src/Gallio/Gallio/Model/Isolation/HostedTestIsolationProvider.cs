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
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;

namespace Gallio.Model.Isolation
{
    /// <summary>
    /// A test isolation provider based on <see cref="IHostFactory" />.
    /// </summary>
    public class HostedTestIsolationProvider : BaseTestIsolationProvider
    {
        private readonly IHostFactory hostFactory;

        /// <summary>
        /// Creates a hosted test isolation provider.
        /// </summary>
        /// <param name="hostFactory">The host factory.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>
        /// is null.</exception>
        public HostedTestIsolationProvider(IHostFactory hostFactory)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");

            this.hostFactory = hostFactory;
        }

        /// <inheritdoc />
        protected override ITestIsolationContext CreateContextImpl(TestIsolationOptions testIsolationOptions, ILogger logger)
        {
            return new HostedTestIsolationContext(hostFactory, testIsolationOptions, logger);
        }
    }
}