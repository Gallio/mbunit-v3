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
using Gallio.Common;
using Gallio.Common.Policies;
using Gallio.Model.Contexts;

namespace Gallio.Model.Environments
{
    /// <summary>
    /// Test environmnent hosting custom actions to be run on AppDomain/Thread set-up and teardown.
    /// </summary>
    public class CustomTestEnvironment : BaseTestEnvironment
    {
        /// <summary>
        /// A chain of actions to be run on AppDomain set-up.
        /// </summary>
        public readonly ActionChain SetUpAppDomainChain = new ActionChain();

        /// <summary>
        /// A chain of actions to be run on AppDomain teardown.
        /// </summary>
        public readonly ActionChain TeardownAppDomainChain = new ActionChain();

        /// <summary>
        /// A chain of actions to be run on Thread set-up.
        /// </summary>
        public readonly ActionChain SetUpThreadChain = new ActionChain();

        /// <summary>
        /// A chain of actions to be run on Thread teardown.
        /// </summary>
        public readonly ActionChain TeardownThreadChain = new ActionChain();

        /// <inheritdoc />
        public override IDisposable SetUpAppDomain()
        {
            SetUpAppDomainChain.Action();
            return new State(TeardownAppDomainChain);
        }

        /// <inheritdoc />
        public override IDisposable SetUpThread()
        {
            SetUpThreadChain.Action();
            return new State(TeardownThreadChain);
        }

        private class State : IDisposable
        {
            private readonly ActionChain teardown;

            public State(ActionChain teardown)
            {
                this.teardown = teardown;
            }

            public void Dispose()
            {
                teardown.Action();
            }
        }
    }
}