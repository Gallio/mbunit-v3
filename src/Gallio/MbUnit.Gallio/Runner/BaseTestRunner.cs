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
using MbUnit.Model.Execution;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Model;
using MbUnit.Model.Serialization;
using MbUnit.Runner.Domains;

namespace MbUnit.Runner
{
    /// <summary>
    /// Base implementation of <see cref="ITestRunner" />.
    /// </summary>
    public class BaseTestRunner : ITestRunner
    {
        private readonly TestEventDispatcher eventDispatcher;
        private readonly ITestDomainFactory domainFactory;

        private TemplateEnumerationOptions templateEnumerationOptions;
        private TestEnumerationOptions testEnumerationOptions;
        private TestExecutionOptions testExecutionOptions;

        private ITestDomain domain;

        /// <summary>
        /// Creates a runner with a specified test domain factory.
        /// </summary>
        /// <param name="domainFactory">The test domain factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="domainFactory"/> is null</exception>
        public BaseTestRunner(ITestDomainFactory domainFactory)
        {
            if (domainFactory == null)
                throw new ArgumentNullException(@"domainFactory");

            this.domainFactory = domainFactory;

            eventDispatcher = new TestEventDispatcher();
            templateEnumerationOptions = new TemplateEnumerationOptions();
            testEnumerationOptions = new TestEnumerationOptions();
            testExecutionOptions = new TestExecutionOptions();
        }

        /// <summary>
        /// Gets the test domain used by the runner.
        /// </summary>
        public ITestDomain Domain
        {
            get
            {
                if (domain == null)
                {
                    domain = domainFactory.CreateDomain();
                    domain.SetTestListener(eventDispatcher);
                }

                return domain;
            }
            protected set { domain = value; }
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            if (domain != null)
            {
                domain.Dispose();
                domain = null;
            }
        }

        /// <inheritdoc />
        public event EventHandler LoadPackageComplete;

        /// <inheritdoc />
        public event EventHandler BuildTemplatesComplete;

        /// <inheritdoc />
        public event EventHandler BuildTestsComplete;

        /// <inheritdoc />
        public event EventHandler RunStarting;

        /// <inheritdoc />
        public event EventHandler RunComplete;

        /// <inheritdoc />
        public TestEventDispatcher EventDispatcher
        {
            get { return eventDispatcher; }
        }

        /// <inheritdoc />
        public TemplateEnumerationOptions TemplateEnumerationOptions
        {
            get { return templateEnumerationOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                templateEnumerationOptions = value;
            }
        }

        /// <inheritdoc />
        public TestEnumerationOptions TestEnumerationOptions
        {
            get { return testEnumerationOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                testEnumerationOptions = value;
            }
        }

        /// <inheritdoc />
        public TestExecutionOptions TestExecutionOptions
        {
            get { return testExecutionOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                testExecutionOptions = value;
            }
        }

        /// <inheritdoc />
        public TestPackage Package
        {
            get { return Domain.Package; }
        }

        /// <inheritdoc />
        public TemplateModel TemplateModel
        {
            get { return Domain.TemplateModel; }
        }

        /// <inheritdoc />
        public TestModel TestModel
        {
            get { return Domain.TestModel; }
        }

        /// <inheritdoc />
        public virtual void LoadPackage(TestPackage package, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (package == null)
                throw new ArgumentNullException(@"package");

            try
            {
                Domain.LoadPackage(package, progressMonitor);
            }
            finally
            {
                if (LoadPackageComplete != null)
                    LoadPackageComplete(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public virtual void BuildTemplates(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            try
            {
                Domain.BuildTemplates(templateEnumerationOptions, progressMonitor);
            }
            finally
            {
                if (BuildTemplatesComplete != null)
                    BuildTemplatesComplete(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public virtual void BuildTests(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            try
            {
                Domain.BuildTests(testEnumerationOptions, progressMonitor);
            }
            finally
            {
                if (BuildTestsComplete != null)
                    BuildTestsComplete(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public virtual void Run(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            try
            {
                if (RunStarting != null)
                    RunStarting(this, EventArgs.Empty);

                Domain.RunTests(progressMonitor, testExecutionOptions);
            }
            finally
            {
                if (RunComplete != null)
                    RunComplete(this, EventArgs.Empty);
            }
        }
    }
}
