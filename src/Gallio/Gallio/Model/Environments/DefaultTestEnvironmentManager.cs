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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gallio.Common;

namespace Gallio.Model.Environments
{
    /// <summary>
    /// Default implementation of a test environment manager.
    /// </summary>
    public class DefaultTestEnvironmentManager : ITestEnvironmentManager
    {
        private readonly ITestEnvironment[] environments;

        /// <summary>
        /// Creates a test environment manager.
        /// </summary>
        /// <param name="environments">The environments, not null.</param>
        public DefaultTestEnvironmentManager(ITestEnvironment[] environments)
        {
            this.environments = environments;
        }

        /// <inheritdoc />
        public virtual IDisposable SetUpAppDomain()
        {
            return new State(environments, environment => environment.SetUpAppDomain());
        }

        /// <inheritdoc />
        public virtual IDisposable SetUpThread()
        {
            return new State(environments, environment => environment.SetUpThread());
        }

        private sealed class State : IDisposable
        {
            private readonly List<IDisposable> innerStates = new List<IDisposable>();

            public State(ITestEnvironment[] environments, Func<ITestEnvironment, IDisposable> setUp)
            {
                foreach (ITestEnvironment environment in environments)
                {
                    IDisposable innerState = setUp(environment);
                    if (innerState != null)
                        innerStates.Add(innerState);
                }
            }

            public void Dispose()
            {
                foreach (IDisposable innerState in innerStates)
                    innerState.Dispose();
            }
        }
    }
}
