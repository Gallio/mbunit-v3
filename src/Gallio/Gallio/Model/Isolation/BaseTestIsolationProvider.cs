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
using Gallio.Runtime.Logging;

namespace Gallio.Model.Isolation
{
    /// <summary>
    /// An abstract base implementation of a test isolation provider.
    /// </summary>
    public abstract class BaseTestIsolationProvider : ITestIsolationProvider
    {
        /// <inheritdoc />
        public ITestIsolationContext CreateContext(TestIsolationOptions testIsolationOptions, ILogger logger)
        {
            if (testIsolationOptions == null)
                throw new ArgumentNullException("testIsolationOptions");
            if (logger == null)
                throw new ArgumentNullException("logger");

            return CreateContextImpl(testIsolationOptions, logger);
        }

        /// <summary>
        /// Creates a test isolation context.
        /// </summary>
        /// <param name="testIsolationOptions">The test isolation options, not null.</param>
        /// <param name="logger">The logger, not null.</param>
        /// <returns>The test isolation context.</returns>
        protected abstract ITestIsolationContext CreateContextImpl(TestIsolationOptions testIsolationOptions, ILogger logger);
    }
}