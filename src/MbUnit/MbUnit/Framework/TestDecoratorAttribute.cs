using System;
using System.Threading;
using Gallio;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// A test decorator applies actions to be performed around the initialization, setup,
    /// execution, teardown and disposal lifecycle of a test.
    /// </para>
    /// <para>
    /// This abstract class provides a convenient way to implement new test decorators
    /// of your own.  If you need more control over how the test is decorated, you may
    /// prefer subclassing <see cref="TestDecoratorPatternAttribute" /> directly instead.
    /// </para>
    /// <para>
    /// When multiple test decorators are applied to a test, they are installed
    /// in order according to the <see cref="DecoratorPatternAttribute.Order" /> property.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method,
        AllowMultiple = true, Inherited = true)]
    public abstract class TestDecoratorAttribute : TestDecoratorPatternAttribute
    {
        private static int nextUniqueId;
        private readonly string defaultActionKey = @"DefaultAction:" + Interlocked.Increment(ref nextUniqueId);

        /// <summary>
        /// Initializes the test.
        /// </summary>
        /// <remarks>
        /// Override this method to add your own behavior around the default actions.
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null</param>
        /// <seealso cref="IPatternTestInstanceHandler.InitializeTestInstance"/>
        protected virtual void Initialize(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <summary>
        /// Sets up the test.
        /// </summary>
        /// <remarks>
        /// Override this method to add your own behavior around the default actions.
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null</param>
        /// <seealso cref="IPatternTestInstanceHandler.SetUpTestInstance"/>
        protected virtual void SetUp(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <remarks>
        /// Override this method to add your own behavior around the default actions.
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null</param>
        /// <seealso cref="IPatternTestInstanceHandler.ExecuteTestInstance"/>
        protected virtual void Execute(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <summary>
        /// Tears down the test.
        /// </summary>
        /// <remarks>
        /// Override this method to add your own behavior around the default actions.
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null</param>
        /// <seealso cref="IPatternTestInstanceHandler.TearDownTestInstance"/>
        protected virtual void TearDown(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <summary>
        /// Disposes the test.
        /// </summary>
        /// <remarks>
        /// Override this method to add your own behavior around the default actions.
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null</param>
        /// <seealso cref="IPatternTestInstanceHandler.DisposeTestInstance"/>
        protected virtual void Dispose(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternTestBuilder builder, ICodeElementInfo codeElement)
        {
            // Note: We do this as a test instance decorator to ensure that all of
            //       the setup actions registered by the test or its containing fixture
            //       can be wrapped because this is the last possible opportunity for new
            //       actions to be added.
            builder.Test.TestActions.DecorateTestInstanceChain.After(delegate(PatternTestState testState, PatternTestInstanceActions decoratedTestInstanceActions)
            {
                WrapDefaultAction(decoratedTestInstanceActions.InitializeTestInstanceChain, Initialize);
                WrapDefaultAction(decoratedTestInstanceActions.SetUpTestInstanceChain, SetUp);
                WrapDefaultAction(decoratedTestInstanceActions.ExecuteTestInstanceChain, Execute);
                WrapDefaultAction(decoratedTestInstanceActions.TearDownTestInstanceChain, TearDown);
                WrapDefaultAction(decoratedTestInstanceActions.DisposeTestInstanceChain, Dispose);
            });
        }

        private void WrapDefaultAction(ActionChain<PatternTestInstanceState> chain, Action<PatternTestInstanceState> decoratorAction)
        {
            chain.Around(delegate(PatternTestInstanceState testInstanceState, Action<PatternTestInstanceState> decoratedAction)
            {
                try
                {
                    testInstanceState.Data.SetValue(defaultActionKey, decoratedAction);
                    decoratorAction(testInstanceState);
                }
                finally
                {
                    testInstanceState.Data.SetValue(defaultActionKey, null);
                }
            });
        }

        private void InvokeDefaultAction(PatternTestInstanceState testInstanceState)
        {
            Action<PatternTestInstanceState> action = testInstanceState.Data.GetValue<Action<PatternTestInstanceState>>(defaultActionKey);
            action(testInstanceState);
        }
    }
}