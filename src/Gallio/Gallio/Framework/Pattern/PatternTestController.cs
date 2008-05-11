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
using Gallio.Framework.Data.Conversions;
using Gallio.Framework.Data.Formatters;
using Gallio.Model.Execution;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Controls the execution of <see cref="PatternTest" /> instances.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternTestController : BaseTestController
    {
        private readonly IConverter converter;
        private readonly IFormatter formatter;

        /// <summary>
        /// Creates a pattern test controller.
        /// </summary>
        /// <param name="formatter">The formatter for data binding</param>
        /// <param name="converter">The converter for data binding</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/>
        /// or <paramref name="converter"/> is null</exception>
        public PatternTestController(IFormatter formatter, IConverter converter)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");
            if (converter == null)
                throw new ArgumentNullException("converter");

            this.formatter = formatter;
            this.converter = converter;
        }

        /// <inheritdoc />
        protected override TestOutcome RunTestsImpl(ITestCommand rootTestCommand, ITestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", rootTestCommand.TestCount);

                // Note: We do not check options.SkipTestExecution here because we want to build up
                // the tree of data-driven test steps.  So we actually check it later on in the
                // PatternTestExecutor.  This is different from framework adapters
                // at this time (because they do not generally support dynamically generated data-driven tests).
                Sandbox sandbox = new Sandbox();
                EventHandler canceledHandler = delegate { sandbox.Abort(TestOutcome.Canceled, "The user canceled the test run."); };
                try
                {
                    progressMonitor.Canceled += canceledHandler;

                    PatternTestExecutor executor = new PatternTestExecutor(options, progressMonitor, formatter, converter);
                    return executor.RunTest(rootTestCommand, parentTestStep, sandbox, null);
                }
                finally
                {
                    progressMonitor.Canceled -= canceledHandler;
                    sandbox.Dispose();
                }
            }
        }
    }
}