// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using MbUnit.Hosting;
using MbUnit.Runner.Domains;
using MbUnit.Runner.Harness;

namespace MbUnit.Runner
{
    /// <summary>
    /// An isolated test runner runs tests in an isolated AppDomain using
    /// an <see cref="IsolatedTestDomain" />.  The <see cref="Runtime" /> must
    /// be initialized prior to using an isolated runner.
    /// </summary>
    public sealed class IsolatedTestRunner : BaseTestRunner
    {
        /// <summary>
        /// Creates an isolated runner using the current runtime stored in <see cref="Runtime.Instance" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="Runtime" /> is not initialized</exception>
        public IsolatedTestRunner()
            : base(new IsolatedTestDomainFactory(Runtime.Instance))
        {
        }
    }
}