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
using MbUnit.Core.Model.Events;
using MbUnit.Core.RuntimeSupport;
using MbUnit.Framework.Kernel.RuntimeSupport;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// Creates instances of <see cref="DependencyTestPlan" />.
    /// </summary>
    public class DependencyTestPlanFactory : ITestPlanFactory
    {
        private readonly ICoreContextManager contextManager;

        /// <summary>
        /// Initializes a test plan factory.
        /// </summary>
        /// <param name="contextManager">The context manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextManager"/> is null</exception>
        private DependencyTestPlanFactory(ICoreContextManager contextManager)
        {
            if (contextManager == null)
                throw new ArgumentNullException(@"contextManager");

            this.contextManager = contextManager;
        }

        /// <summary>
        /// Initializes a test plan factory.
        /// </summary>
        /// <remarks>
        /// This is a temporary workaround for the fact that Windsor does not consider
        /// subtypes of services to be acceptable substitutes for a service.  Hence we
        /// can't just register ICoreContextManager because we also want to resolve it
        /// as IContextManager.
        /// </remarks>
        /// <param name="contextManager">The context manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextManager"/> is null</exception>
        public DependencyTestPlanFactory(IContextManager contextManager)
            : this((ICoreContextManager) contextManager)
        {
        }

        /// <inheritdoc />
        public ITestPlan CreateTestPlan(ITestListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(@"listener");

            return new DependencyTestPlan(contextManager, listener);
        }
    }
}
