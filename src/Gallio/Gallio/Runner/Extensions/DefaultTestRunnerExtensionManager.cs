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
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Default implementation of <see cref="ITestRunnerExtensionManager"/>
    /// </summary>
    public class DefaultTestRunnerExtensionManager : ITestRunnerExtensionManager
    {
        private readonly ComponentHandle<ITestRunnerExtensionFactory, TestRunnerExtensionFactoryTraits>[] factoryHandles;

        /// <summary>
        /// Creates a test runner extension manager.
        /// </summary>
        /// <param name="factoryHandles">The factory handles, not null</param>
        public DefaultTestRunnerExtensionManager(ComponentHandle<ITestRunnerExtensionFactory, TestRunnerExtensionFactoryTraits>[] factoryHandles)
        {
            this.factoryHandles = factoryHandles;
        }

        /// <inheritdoc />
        public void RegisterAutoActivatedExtensions(ITestRunner testRunner)
        {
            if (testRunner == null)
                throw new ArgumentNullException("testRunner");

            var context = new ActivationConditionContext();

            foreach (var factoryHandle in factoryHandles)
            {
                TestRunnerExtensionFactoryTraits traits = factoryHandle.GetTraits();
                if (traits.AutoActivationCondition != null)
                {
                    if (traits.AutoActivationCondition.Evaluate(context))
                    {
                        ITestRunnerExtensionFactory factory = factoryHandle.GetComponent();
                        testRunner.RegisterExtension(factory.CreateExtension());
                    }
                }
            }
        }
    }
}
