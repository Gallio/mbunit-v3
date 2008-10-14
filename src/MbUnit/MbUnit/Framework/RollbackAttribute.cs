// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Transactions;
using Gallio.Collections;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Tags a test method whose database operation must be executed within a transaction and rolled
    /// back when it has finished executing.
    /// </para>
    /// <para>
    /// By default, the attribute does not rollback changes performed during SetUp and TearDown.
    /// Set the <see cref="IncludeSetUpAndTearDown"/> to <c>true</c> to change this.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The attribute uses a <see cref="TransactionScope" /> to set an ambient transaction that will
    /// be rolled back.  Assuming the subject under test enlists itself with this transaction then
    /// its transactions operations will be rolled back.
    /// </para>
    /// <para>
    /// Unfortunately there are a few caveats from this approach, the subject under test might use
    /// its own transaction manager and ignore the ambient transaction set up here.  Likewise,
    /// it is possible that the presence of an enclosing transaction scope can interfere with the
    /// normal operation of the subject under test.
    /// </para>
    /// <para>
    /// If this attribute does not work for a particular application, we recommend that you
    /// create your own custom rollback decorator attribute with the appropriate application-specific
    /// semantics.  Just create a new subclass of <see cref="TestDecoratorAttribute" /> and override the
    /// <see cref="TestDecoratorAttribute.Execute" /> method to wrap test execution as required.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// The following example shows the <see cref="RollbackAttribute"/> in use:
    /// </para>
    /// <code>
    /// [TestFixture]
    /// public class RollbackTest {
    /// 
    ///     [Test, Rollback]
    ///     public void TestWithRollback()
    ///     {
    ///         // Any transaction performed within this test will be rolled back
    ///         // automatically when the test completes.
    ///     }
    ///     
    ///     [Test, Rollback(IncludeSetupAndTearDown=true)]
    ///     public void TestWithRollback()
    ///     {
    ///         // Any transaction performed within this test or within its setup and 
    ///         // teardown will be rolled back automatically when the test completes.
    ///     }
    /// } 
    /// </code>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class RollbackAttribute : TestDecoratorPatternAttribute
    {
        private readonly Key<TransactionScope> TransactionScopeKey = new Key<TransactionScope>("RollbackAttribute.TransactionScope");

        /// <summary>
        /// <para>
        /// Tags a test method whose database operation must be executed within a transaction and rolled
        /// back when it has finished executing.
        /// </para>
        /// </summary>
        public RollbackAttribute()
        {
        }

        /// <summary>
        /// <para>
        /// When set to true, includes setup and teardown in the rollback.  Otherwise
        /// only transactions performed during the test itself are affected.
        /// </para>
        /// <para>
        /// The fixture setup and teardown is not included regardless unless this
        /// attribute is applied to the fixture class itself instead of the test method.
        /// </para>
        /// </summary>
        public bool IncludeSetUpAndTearDown { get; set; }

        /// <inheritdoc />
        protected override void DecorateTest(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            if (IncludeSetUpAndTearDown)
            {
                scope.Test.TestInstanceActions.BeforeTestInstanceChain.Before(delegate(PatternTestInstanceState state)
                {
                    state.Data.SetValue(TransactionScopeKey, CreateAndEnterTransactionScope(state));
                });

                scope.Test.TestInstanceActions.AfterTestInstanceChain.After(delegate(PatternTestInstanceState state)
                {
                    TransactionScope transactionScope;
                    if (state.Data.TryGetValue(TransactionScopeKey, out transactionScope))
                        transactionScope.Dispose();
                });
            }
            else
            {
                scope.Test.TestInstanceActions.ExecuteTestInstanceChain.Around(delegate(PatternTestInstanceState state, Action<PatternTestInstanceState> inner)
                {
                    using (CreateAndEnterTransactionScope(state))
                    {
                        inner(state);
                    }
                });
            }
        }

        private static TransactionScope CreateAndEnterTransactionScope(PatternTestInstanceState state)
        {
            TimeSpan timeout = TransactionManager.MaximumTimeout;
            if (state.Test.Timeout.HasValue && state.Test.Timeout.Value < timeout)
                timeout = state.Test.Timeout.Value;
            return new TransactionScope(TransactionScopeOption.RequiresNew, timeout);
        }
    }
}
