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
using System.Threading;
using Gallio.Runtime;
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
    public class PatternTest : BaseTest, IPatternTestComponent
    {
        private readonly PatternTestDataContext dataContext;
        private readonly PatternTestActions testActions;
        private TimeSpan? timeout;
        private ApartmentState apartmentState = ApartmentState.Unknown;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the test</param>
        /// <param name="codeElement">The point of definition of the test, or null if unknown</param>
        /// <param name="dataContext">The data context</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="dataContext"/> is null</exception>
        public PatternTest(string name, ICodeElementInfo codeElement, PatternTestDataContext dataContext)
            : base(name, codeElement)
        {
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            this.dataContext = dataContext;
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
        /// <para>
        /// Gets or sets the apartment state to be used to run the test.
        /// </para>
        /// <para>
        /// If the apartment state is <see cref="System.Threading.ApartmentState.Unknown" />
        /// the test will inherit the apartment state of its parent.  Otherwise
        /// it will run in a thread with the specified apartment state.
        /// </para>
        /// <para>
        /// The test runner guarantees that the root test runs with the <see cref="System.Threading.ApartmentState.STA" />
        /// apartment state.  Consequently the apartment state only needs to be overridden to run 
        /// a test in some mode that may differ from that which it would ordinarily inherit.
        /// </para>
        /// </summary>
        /// <value>
        /// The default value of this property is <see cref="System.Threading.ApartmentState.Unknown" />.
        /// </value>
        public ApartmentState ApartmentState
        {
            get { return apartmentState; }
            set { apartmentState = value; }
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
        public override Func<ITestController> TestControllerFactory
        {
            get { return GetTestController; }
        }

        /// <inheritdoc />
        public PatternTestDataContext DataContext
        {
            get { return dataContext; }
        }

        /// <inheritdoc />
        public void SetName(string value)
        {
            Name = value;
        }

        private static ITestController GetTestController()
        {
            return ResolveTestController();
        }

        private static PatternTestController ResolveTestController()
        {
            return RuntimeAccessor.Instance.Resolve<PatternTestController>();
        }
    }
}