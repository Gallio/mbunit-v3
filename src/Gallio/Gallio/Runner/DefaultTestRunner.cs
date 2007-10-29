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
using Gallio.Core.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Runner.Domains;

namespace Gallio.Runner
{
    /// <summary>
    /// Default implementation of <see cref="ITestRunner" />.
    /// </summary>
    public class DefaultTestRunner : ITestRunner
    {
        private readonly ITestDomainFactory domainFactory;

        private bool isDisposed;

        private TestEventDispatcher eventDispatcher;
        private TemplateEnumerationOptions templateEnumerationOptions;
        private TestEnumerationOptions testEnumerationOptions;
        private TestExecutionOptions testExecutionOptions;

        private ITestDomain domain;

        /// <summary>
        /// Creates a runner with a specified test domain factory.
        /// </summary>
        /// <param name="domainFactory">The test domain factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="domainFactory"/> is null</exception>
        public DefaultTestRunner(ITestDomainFactory domainFactory)
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
                ThrowIfDisposed();

                if (domain == null)
                {
                    domain = domainFactory.CreateDomain();
                    domain.SetTestListener(eventDispatcher);
                }

                return domain;
            }
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            if (!isDisposed)
            {
                if (domain != null)
                {
                    domain.Dispose();
                    domain = null;
                }

                LoadPackageComplete = null;
                BuildTemplatesComplete = null;
                BuildTestsComplete = null;
                RunStarting = null;
                RunComplete = null;
                eventDispatcher = null;
                templateEnumerationOptions = null;
                testEnumerationOptions = null;
                testExecutionOptions = null;

                isDisposed = true;
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
            get
            {
                ThrowIfDisposed();
                return eventDispatcher;
            }
        }

        /// <inheritdoc />
        public TemplateEnumerationOptions TemplateEnumerationOptions
        {
            get
            {
                ThrowIfDisposed();
                return templateEnumerationOptions;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                ThrowIfDisposed();

                templateEnumerationOptions = value;
            }
        }

        /// <inheritdoc />
        public TestEnumerationOptions TestEnumerationOptions
        {
            get
            {
                ThrowIfDisposed();
                return testEnumerationOptions;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                ThrowIfDisposed();

                testEnumerationOptions = value;
            }
        }

        /// <inheritdoc />
        public TestExecutionOptions TestExecutionOptions
        {
            get
            {
                ThrowIfDisposed();
                return testExecutionOptions;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                ThrowIfDisposed();

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
            ThrowIfDisposed();

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
            ThrowIfDisposed();

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
            ThrowIfDisposed();

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
            ThrowIfDisposed();

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

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
