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
