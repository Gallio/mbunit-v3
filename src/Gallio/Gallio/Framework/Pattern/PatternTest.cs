// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Data;
using Gallio.Hosting;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Gallio.Model;
using Gallio;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test case that has been defined by the <see cref="PatternTestFramework" />.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternTest : BaseTest, IDataSourceScope
    {
        private readonly PatternTestActions testActions;
        private DataSourceTable dataSourceTable;
        private TimeSpan? timeout;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public PatternTest(string name, ICodeElementInfo codeElement) : base(name, codeElement)
        {
            testActions = new PatternTestActions();
        }

        /// <summary>
        /// Gets or sets the maximum amount of time the whole test including
        /// its setup, teardown and body should be permitted to run.  If the test
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
        /// Gets the set of actions that describe the behavior of the test.
        /// </summary>
        public PatternTestActions TestActions
        {
            get { return testActions; }
        }

        /// <summary>
        /// Gets the set of actions that describe the behavior of the test's instances.
        /// </summary>
        public PatternTestInstanceActions TestInstanceActions
        {
            get { return testActions.TestInstanceActions; }
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

        /// <inheritdoc />
        public override Func<ITestController> TestControllerFactory
        {
            get { return GetTestController; }
        }

        private static ITestController GetTestController()
        {
            return ResolveTestController();
        }

        private static PatternTestController ResolveTestController()
        {
            return Runtime.Instance.Resolve<PatternTestController>();
        }
    }
}