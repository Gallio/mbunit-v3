// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Runner.Domains;
using Gallio.Utilities;

namespace Gallio.Runner
{
    /// <summary>
    /// An implementation of <see cref="ITestRunner" /> that uses a <see cref="ITestDomainFactory" />
    /// to create a test domain and manage test execution.
    /// </summary>
    public class DomainTestRunner : ITestRunner
    {
        private readonly ITestDomainFactory domainFactory;

        private bool isDisposed;

        private TestEventDispatcher eventDispatcher;
        private TestEnumerationOptions testEnumerationOptions;
        private TestExecutionOptions testExecutionOptions;

        private ITestDomain domain;

        /// <summary>
        /// Creates a runner with a specified test domain factory.
        /// </summary>
        /// <param name="domainFactory">The test domain factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="domainFactory"/> is null</exception>
        public DomainTestRunner(ITestDomainFactory domainFactory)
        {
            if (domainFactory == null)
                throw new ArgumentNullException(@"domainFactory");

            this.domainFactory = domainFactory;

            eventDispatcher = new TestEventDispatcher();
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
                    domain = domainFactory.CreateDomain();

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

                TestPackageChanged = null;
                BuildTestModelComplete = null;
                RunTestsStarting = null;
                RunTestsComplete = null;
                eventDispatcher = null;
                testEnumerationOptions = null;
                testExecutionOptions = null;

                isDisposed = true;
            }
        }

        /// <inheritdoc />
        public event EventHandler TestPackageChanged;

        /// <inheritdoc />
        public event EventHandler BuildTestModelComplete;

        /// <inheritdoc />
        public event EventHandler RunTestsStarting;

        /// <inheritdoc />
        public event EventHandler RunTestsComplete;

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
        public TestPackageData TestPackageData
        {
            get { return Domain.TestPackageData; }
        }

        /// <inheritdoc />
        public TestModelData TestModelData
        {
            get { return Domain.TestModelData; }
        }

        /// <inheritdoc />
        public virtual void LoadTestPackage(TestPackageConfig packageConfig, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (packageConfig == null)
                throw new ArgumentNullException("packageConfig");
            ThrowIfDisposed();

            try
            {
                Domain.LoadTestPackage(packageConfig, progressMonitor);
            }
            finally
            {
                EventHandlerUtils.SafeInvoke(TestPackageChanged, this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public virtual void BuildTestModel(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            ThrowIfDisposed();

            try
            {
                Domain.BuildTestModel(testEnumerationOptions, progressMonitor);
            }
            finally
            {
                EventHandlerUtils.SafeInvoke(BuildTestModelComplete, this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public virtual void RunTests(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            ThrowIfDisposed();

            try
            {
                EventHandlerUtils.SafeInvoke(RunTestsStarting, this, EventArgs.Empty);

                using (ProxyTestListener proxyTestListener = new ProxyTestListener(eventDispatcher))
                    Domain.RunTests(testExecutionOptions, proxyTestListener, progressMonitor);
            }
            finally
            {
                EventHandlerUtils.SafeInvoke(RunTestsComplete, this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public virtual void UnloadTestPackage(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            ThrowIfDisposed();

            try
            {
                Domain.UnloadPackage(progressMonitor);
            }
            finally
            {
                EventHandlerUtils.SafeInvoke(TestPackageChanged, this, EventArgs.Empty);
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
