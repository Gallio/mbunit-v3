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
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Runner.Harness;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// A simple test driver implementation that runs tests locally within the
    /// current domain without any additional consultation of the test frameworks.
    /// </summary>
    public class LocalTestDriver : BaseTestDriver
    {
        private ITestHarness harness;

        /// <inheritdoc />
        protected override void InitializeImpl(RuntimeSetup runtimeSetup, ILogger logger)
        {
            base.InitializeImpl(runtimeSetup, logger);

            ITestHarnessFactory factory = RuntimeAccessor.Instance.Resolve<ITestHarnessFactory>();
            harness = factory.CreateHarness();
        }

        /// <inheritdoc />
        protected override void LoadImpl(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor)
        {
            harness.Load(testPackageConfig, progressMonitor);
        }

        /// <inheritdoc />
        protected override TestModelData ExploreImpl(TestExplorationOptions options, IProgressMonitor progressMonitor)
        {
            harness.Explore(options, progressMonitor);
            return new TestModelData(harness.TestModel);
        }

        /// <inheritdoc />
        protected override void RunImpl(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor)
        {
            harness.Run(options, listener, progressMonitor);
        }

        /// <inheritdoc />
        protected override void UnloadImpl(IProgressMonitor progressMonitor)
        {
            harness.Unload(progressMonitor);
        }
    }
}
