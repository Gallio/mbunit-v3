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
using Gallio.Model.Execution;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Model;
using Gallio.Utilities;
using Gallio.Model.Serialization;

namespace Gallio.Runner.Domains
{
    /// <summary>
    /// Base implementation of a test domain.
    /// </summary>
    /// <remarks>
    /// The base implementation inherits from <see cref="MarshalByRefObject" />
    /// so that test domain's services can be accessed remotely if needed.
    /// </remarks>
    public abstract class BaseTestDomain : LongLivingMarshalByRefObject, ITestDomain
    {
        private bool disposed;
        private ITestListener listener;
        private TestPackageData packageData;
        private TestModelData modelData;

        /// <summary>
        /// Creates a test domain.
        /// </summary>
        protected BaseTestDomain()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!disposed)
            {
                InternalDispose();

                listener = null;
                packageData = null;
                modelData = null;
                disposed = true;
            }
        }

        /// <inheritdoc />
        public TestPackageData TestPackageData
        {
            get
            {
                ThrowIfDisposed();
                return packageData;
            }
        }

        /// <inheritdoc />
        public TestModelData TestModelData
        {
            get
            {
                ThrowIfDisposed();
                return modelData;
            }
        }

        /// <summary>
        /// Gets the event listener, or null if none was set.
        /// </summary>
        protected ITestListener Listener
        {
            get { return listener; }
        }

        /// <inheritdoc />
        public void SetTestListener(ITestListener listener)
        {
            this.listener = listener;
        }

        /// <inheritdoc />
        public void LoadTestPackage(TestPackageConfig packageConfig, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (packageConfig == null)
                throw new ArgumentNullException("packageConfig");

            ThrowIfDisposed();

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading test package.", 1.05);

                UnloadPackage(progressMonitor.CreateSubProgressMonitor(0.05));

                packageData = InternalLoadTestPackage(packageConfig, progressMonitor);
            }
        }

        /// <inheritdoc />
        public void BuildTestModel(TestEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (options == null)
                throw new ArgumentNullException("options");

            ThrowIfDisposed();

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Building tests.", 1);

                if (packageData == null)
                    throw new InvalidOperationException("No test package has been loaded.");

                modelData = InternalBuildTestModel(options, progressMonitor);
            }
        }

        /// <inheritdoc />
        public void UnloadPackage(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();

            using (progressMonitor)
            {
                if (packageData != null)
                {
                    progressMonitor.BeginTask("Unloading test package.", 1);
                    InternalUnloadTestPackage(progressMonitor);

                    packageData = null;
                    modelData = null;
                }
            }
        }

        /// <inheritdoc />
        public void RunTests(IProgressMonitor progressMonitor, TestExecutionOptions options)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (options == null)
                throw new ArgumentNullException("options");

            ThrowIfDisposed();

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", 1);

                if (modelData == null)
                    throw new InvalidOperationException("The test model has not been built.");

                InternalRunTests(options, progressMonitor);
            }
        }

        /// <summary>
        /// Internal implementation of <see cref="Dispose" />.
        /// </summary>
        protected abstract void InternalDispose();

        /// <summary>
        /// Internal implementation of <see cref="LoadTestPackage" />.
        /// </summary>
        /// <param name="packageConfig">The test package configuration</param>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        /// <returns>The test package data</returns>
        protected abstract TestPackageData InternalLoadTestPackage(TestPackageConfig packageConfig, IProgressMonitor progressMonitor);

        /// <summary>
        /// Internal implementation of <see cref="BuildTestModel" />.
        /// </summary>
        /// <param name="options">The test enumeration options</param>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        /// <returns>The test model data</returns>
        protected abstract TestModelData InternalBuildTestModel(TestEnumerationOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// Internal implementation of <see cref="RunTests" />.
        /// </summary>
        /// <param name="options">The test execution options</param>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        protected abstract void InternalRunTests(TestExecutionOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// Internal implementation of <see cref="UnloadPackage" />.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        protected abstract void InternalUnloadTestPackage(IProgressMonitor progressMonitor);

        /// <summary>
        /// Throws <see cref="ObjectDisposedException"/> if the domain has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException("Isolated test domain");
        }
    }
}