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
using MbUnit.Core.Reporting;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Core.Runtime;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Base implementation of <see cref="ITestRunner" />.
    /// </summary>
    public class BaseRunner : ITestRunner
    {
        private readonly EventDispatcher eventDispatcher;
        private readonly ITestDomainFactory domainFactory;
        private ICoreRuntime runtime;

        private TemplateEnumerationOptions templateEnumerationOptions;
        private TestEnumerationOptions testEnumerationOptions;
        private TestExecutionOptions testExecutionOptions;

        private ITestDomain domain;

        /// <summary>
        /// Creates a runner with a specified test domain factory.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <param name="domainFactory">The test domain factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/>
        /// or <paramref name="domainFactory"/> is null</exception>
        public BaseRunner(ICoreRuntime runtime, ITestDomainFactory domainFactory)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (domainFactory == null)
                throw new ArgumentNullException("domainFactory");

            this.runtime = runtime;
            this.domainFactory = domainFactory;

            eventDispatcher = new EventDispatcher();
            templateEnumerationOptions = new TemplateEnumerationOptions();
            testEnumerationOptions = new TestEnumerationOptions();
            testExecutionOptions = new TestExecutionOptions();
        }

        /// <summary>
        /// Gets the runtime used by the test runner to obtain services.
        /// </summary>
        public ICoreRuntime Runtime
        {
            get { return runtime; }
            protected set { runtime = value; }
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
                    domain.SetEventListener(eventDispatcher);
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
        public EventDispatcher EventDispatcher
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
                    throw new ArgumentNullException("value");

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
                    throw new ArgumentNullException("value");

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
                    throw new ArgumentNullException("value");

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
                throw new ArgumentNullException("progressMonitor");
            if (package == null)
                throw new ArgumentNullException("package");

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
                throw new ArgumentNullException("progressMonitor");

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
                throw new ArgumentNullException("progressMonitor");

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
                throw new ArgumentNullException("progressMonitor");

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
