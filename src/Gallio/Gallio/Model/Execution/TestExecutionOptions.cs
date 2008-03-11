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
using Gallio.Model.Filters;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Provides options that control how test execution occurs.
    /// </summary>
    [Serializable]
    public class TestExecutionOptions
    {
        private Filter<ITest> filter = new AnyFilter<ITest>();
        private bool skipDynamicTestInstances;
        private bool skipTestInstanceExecution;

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>Defaults to an instance of <see cref="AnyFilter{T}" />.</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public Filter<ITest> Filter
        {
            get { return filter; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                filter = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether to skip running dynamic test instances.
        /// </para>
        /// <para>
        /// This flag can be useful in combination with <see cref="SkipTestInstanceExecution" />
        /// to enumerate non-dynamic test instances only.
        /// </para>
        /// </summary>
        /// <value>Defaults to <c>false</c></value>
        public bool SkipDynamicTestInstances
        {
            get { return skipDynamicTestInstances; }
            set { skipDynamicTestInstances = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether to skip the execution of test instances.
        /// </para>
        /// <para>
        /// The test runner will go through most of the motions of running tests but will skip
        /// the actual execution phase.  This option can be used to enumerate test instances without
        /// running them and to pre-validate the test environment without doing most of the work
        /// of test execution.
        /// </para>
        /// </summary>
        /// <value>Defaults to <c>false</c></value>
        public bool SkipTestInstanceExecution
        {
            get { return skipTestInstanceExecution; }
            set { skipTestInstanceExecution = value; }
        }
    }
}