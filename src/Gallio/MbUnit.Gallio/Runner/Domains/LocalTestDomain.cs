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
using MbUnit.Runner.Harness;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Hosting;
using MbUnit.Model;
using MbUnit.Model.Execution;
using MbUnit.Model.Serialization;
using MbUnit.Runner.Domains;

namespace MbUnit.Runner.Domains
{
    /// <summary>
    /// A local implementation of a test domain that performs all processing
    /// with the current app-domain including loading assemblies.
    /// </summary>
    /// <remarks>
    /// The <see cref="Runtime" /> must be initialized prior to the use of this domain.
    /// </remarks>
    public class LocalTestDomain : BaseTestDomain
    {
        private ITestHarnessFactory harnessFactory;
        private ITestHarness harness;

        /// <summary>
        /// Creates a local test domain using the specified resolver manager.
        /// </summary>
        /// <param name="harnessFactory">The test harness factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="harnessFactory"/> is null</exception>
        public LocalTestDomain(ITestHarnessFactory harnessFactory)
        {
            if (harnessFactory == null)
                throw new ArgumentNullException(@"harnessFactory");

            this.harnessFactory = harnessFactory;
        }

        /// <inheritdoc />
        protected override void InternalDispose()
        {
            harnessFactory = null;
        }

        /// <inheritdoc />
        protected override void InternalLoadPackage(TestPackage package, IProgressMonitor progressMonitor)
        {
            progressMonitor.SetStatus("Creating test harness.");

            harness = harnessFactory.CreateHarness();

            progressMonitor.Worked(0.1);

            if (Listener != null)
                harness.EventDispatcher.Listeners.Add(Listener);

            harness.LoadPackage(package, new SubProgressMonitor(progressMonitor, 0.9));
        }

        /// <inheritdoc />
        protected override void InternalBuildTemplates(TemplateEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            TemplateModel = null;
            harness.BuildTemplates(options, new SubProgressMonitor(progressMonitor, 1));
            TemplateModel = new TemplateModel(new TemplateData(harness.TemplateTreeBuilder.Root));
        }

        /// <inheritdoc />
        protected override void InternalBuildTests(TestEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            TestModel = null;
            harness.BuildTests(options, new SubProgressMonitor(progressMonitor, 1));
            TestModel = new TestModel(new TestData(harness.TestTreeBuilder.Root));
        }

        /// <inheritdoc />
        protected override void InternalRunTests(TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            harness.RunTests(options, new SubProgressMonitor(progressMonitor, 1));
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
            }
        }
    }
}