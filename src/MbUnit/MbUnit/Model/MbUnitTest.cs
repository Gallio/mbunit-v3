// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Data;
using Gallio.Reflection;
using MbUnit.Model;
using Gallio.Model;
using Gallio.Model.Actions;

namespace MbUnit.Model
{
    /// <summary>
    /// A basic MbUnit test.
    /// </summary>
    public class MbUnitTest : BaseTest, IDataSourceScope
    {
        private DataSourceTable dataSourceTable;
        private event MbUnitTestPopulator populator;

        private readonly ActionChain<MbUnitTestState> setUpChain;
        private readonly ActionChain<MbUnitTestState> executeChain;
        private readonly ActionChain<MbUnitTestState> tearDownChain;
        private readonly ActionChain<MbUnitTestState> beforeChildChain;
        private readonly ActionChain<MbUnitTestState> afterChildChain;

        private TimeSpan? timeout;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public MbUnitTest(string name, ICodeElementInfo codeElement) : base(name, codeElement)
        {
            setUpChain = new ActionChain<MbUnitTestState>();
            executeChain = new ActionChain<MbUnitTestState>();
            tearDownChain = new ActionChain<MbUnitTestState>();
            beforeChildChain = new ActionChain<MbUnitTestState>();
            afterChildChain = new ActionChain<MbUnitTestState>();
        }

        /// <summary>
        /// Gets or sets the maximum amount of time the whole test including
        /// it setup, teardown and body should be permitted to run.  If the test
        /// runs any longer than this, it will be aborted by the framework.
        /// The timeout may be null to indicate the absence of a timeout.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/>
        /// represents a negative time span</exception>
        public TimeSpan? Timeout
        {
            get { return timeout; }
            set
            {
                if (value.HasValue && value.Value.Ticks < 0)
                    throw new ArgumentOutOfRangeException(@"value");
                timeout = value;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if there is no additional work required to populate
        /// the children of this test.
        /// </summary>
        /// <seealso cref="Populate"/>
        public bool IsPopulated
        {
            get { return populator == null; }
        }

        /// <summary>
        /// Gets the chain of actions to perform while the test
        /// is being set up prior to execution.  The chain may be
        /// extended to inject new behaviors.
        /// </summary>
        /// <remarks>
        /// The actions in the chain are executed as part of the
        /// setup lifecycle phase of the test.
        /// </remarks>
        public ActionChain<MbUnitTestState> SetUpChain
        {
            get { return setUpChain; }
        }

        /// <summary>
        /// Gets the chain of actions to perform while the main body
        /// of the test is executing.  The chain may be
        /// extended to inject new behaviors.
        /// </summary>
        /// <remarks>
        /// The actions in the chain are executed as part of the
        /// execute lifecycle phase of the test.
        /// </remarks>
        public ActionChain<MbUnitTestState> ExecuteChain
        {
            get { return executeChain; }
        }

        /// <summary>
        /// Gets the chain of actions to perform while the test is being
        /// torn down after execution.  The chain may be
        /// extended to inject new behaviors.
        /// </summary>
        /// <remarks>
        /// The actions in the chain are executed as part of the
        /// tear down lifecycle phase of the test.
        /// </remarks>
        public ActionChain<MbUnitTestState> TearDownChain
        {
            get { return tearDownChain; }
        }

        /// <summary>
        /// Gets the chain of actions to perform before each nested test
        /// is executed.  The chain may be extended to inject new behaviors.
        /// </summary>
        /// <remarks>
        /// The actions in the chain are executed as part of the
        /// set up lifecycle phase of the nested test before the test's own <see cref="SetUpChain" /> runs.
        /// </remarks>
        public ActionChain<MbUnitTestState> BeforeChildChain
        {
            get { return beforeChildChain; }
        }

        /// <summary>
        /// Gets the chain of actions to perform after each nested test
        /// is executed.  The chain may be extended to inject new behaviors.
        /// </summary>
        /// <remarks>
        /// The actions in the chain are executed as part of the
        /// tear down lifecycle phase of the nested test after the test's own <see cref="TearDownChain" /> runs.
        /// </remarks>
        public ActionChain<MbUnitTestState> AfterChildChain
        {
            get { return afterChildChain; }
        }

        /// <summary>
        /// Populates the children of this test if not already populated.
        /// Otherwise does nothing.
        /// </summary>
        /// <param name="recurse">If true, the populator should recursively populate
        /// all of its newly populated test elements in addition to itself</param>
        public void Populate(bool recurse)
        {
            MbUnitTestPopulator oldPopulator = populator;

            if (oldPopulator != null)
            {
                populator = null;
                oldPopulator(recurse);
            }
        }

        /// <summary>
        /// <para>
        /// Adds a populator to the list of actions to perform to populate
        /// the children of this test lazily.
        /// </para>
        /// <para>
        /// When the test is initially created, it may not
        /// have all of its children in place.  This can occur because we only
        /// require a subset of the tests to be populated for certain operations
        /// such as enumerating the tests within a type.  The delegate added here
        /// will eventually be called by <see cref="Populate" /> if the children
        /// of this test are needed.
        /// </para>
        /// </summary>
        /// <param name="populator">The populator to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="populator"/> is null</exception>
        public void AddPopulator(MbUnitTestPopulator populator)
        {
            if (populator == null)
                throw new ArgumentNullException("populator");

            this.populator += populator;
        }

        /// <inheritdoc />
        public DataSource DefineDataSource(string name)
        {
            if (dataSourceTable == null)
                dataSourceTable = new DataSourceTable();

            return dataSourceTable.DefineDataSource(name);
        }

        /// <inheritdoc />
        public DataSource ResolveDataSource(string name)
        {
            if (dataSourceTable != null)
            {
                DataSource source = dataSourceTable.ResolveDataSource(name);
                if (source != null)
                    return source;
            }

            IDataSourceScope parentScope = Parent as IDataSourceScope;
            return parentScope != null ? parentScope.ResolveDataSource(name) : null;
        }
    }
}