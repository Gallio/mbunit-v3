// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using MbUnit.Core.Harness;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Runtime;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A local implementation of a test domain that performs all processing
    /// with the current app-domain including loading assemblies.
    /// </summary>
    public class LocalTestDomain : BaseTestDomain
    {
        private IRuntime runtime;
        private ITestHarnessFactory harnessFactory;
        private ITestHarness harness;

        /// <summary>
        /// Creates a local test domain using the specified resolver manager.
        /// </summary>
        /// <param name="runtime">The runtime environment for tests (will be set in
        /// <see cref="RuntimeHolder" /> during test execution)</param>
        /// <param name="harnessFactory">The test harness factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> or
        /// <paramref name="harnessFactory"/> is null</exception>
        public LocalTestDomain(IRuntime runtime, ITestHarnessFactory harnessFactory)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (harnessFactory == null)
                throw new ArgumentNullException("harnessFactory");

            this.runtime = runtime;
            this.harnessFactory = harnessFactory;
        }

        /// <inheritdoc />
        protected override void InternalDispose()
        {
            runtime = null;
            harnessFactory = null;
        }

        /// <inheritdoc />
        protected override void InternalLoadPackage(IProgressMonitor progressMonitor, TestPackage package)
        {
            progressMonitor.SetStatus("Creating test harness.");

            RuntimeHolder.Instance = runtime;
            harness = harnessFactory.CreateHarness();

            progressMonitor.Worked(0.1);

            if (Listener != null)
                harness.EventDispatcher.Listeners.Add(Listener);

            harness.LoadPackage(new SubProgressMonitor(progressMonitor, 0.9), package);
        }

        /// <inheritdoc />
        protected override void InternalBuildTemplates(IProgressMonitor progressMonitor, TemplateEnumerationOptions options)
        {
            TemplateModel = null;
            harness.BuildTemplates(new SubProgressMonitor(progressMonitor, 1), options);
            TemplateModel = new TemplateModel(new TemplateInfo(harness.TemplateTreeBuilder.Root));
        }

        /// <inheritdoc />
        protected override void InternalBuildTests(IProgressMonitor progressMonitor, TestEnumerationOptions options)
        {
            TestModel = null;
            harness.BuildTests(new SubProgressMonitor(progressMonitor, 1), options);
            TestModel = new TestModel(new TestInfo(harness.TestTreeBuilder.Root));
        }

        /// <inheritdoc />
        protected override void InternalRunTests(IProgressMonitor progressMonitor, TestExecutionOptions options)
        {
            harness.RunTests(new SubProgressMonitor(progressMonitor, 1), options);
        }

        /// <inheritdoc />
        protected override void InternalUnloadPackage(IProgressMonitor progressMonitor)
        {
            try
            {
                progressMonitor.SetStatus("Disposing test harness.");

                if (harness != null)
                    harness.Dispose();

                progressMonitor.Worked(1);
            }
            finally
            {
                harness = null;
                RuntimeHolder.Instance = null;
            }
        }
    }
}
