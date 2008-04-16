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
using Gallio.Runtime.Remoting;
using Gallio.Model.Execution;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
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
    public abstract class BaseTestDomain : LongLivedMarshalByRefObject, ITestDomain
    {
        private bool disposed;
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

        /// <inheritdoc />
        public void Load(TestPackageConfig packageConfig, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (packageConfig == null)
                throw new ArgumentNullException("packageConfig");

            ThrowIfDisposed();

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading test package.", 1.05);

                Unload(progressMonitor.CreateSubProgressMonitor(0.05));

                TestPackageConfig canonicalPackageConfig = packageConfig.Copy();
                canonicalPackageConfig.Canonicalize(null);

                packageData = InternalLoad(canonicalPackageConfig, progressMonitor);
            }
        }

        /// <inheritdoc />
        public void Explore(TestExplorationOptions options, IProgressMonitor progressMonitor)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Building tests.", 1);

                if (packageData == null)
                    throw new InvalidOperationException("No test package has been loaded.");

                modelData = InternalExplore(options, progressMonitor);
            }
        }

        /// <inheritdoc />
        public void Unload(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();

            using (progressMonitor)
            {
                if (packageData != null)
                {
                    progressMonitor.BeginTask("Unloading test package.", 1);
                    InternalUnload(progressMonitor);

                    packageData = null;
                    modelData = null;
                }
            }
        }

        /// <inheritdoc />
        public void Run(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            if (listener == null)
                throw new ArgumentNullException("listener");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", 1);

                if (modelData == null)
                    throw new InvalidOperationException("The test model has not been built.");

                InternalRun(options, listener, progressMonitor);
            }
        }

        /// <summary>
        /// Internal implementation of <see cref="Dispose" />.
        /// </summary>
        protected abstract void InternalDispose();

        /// <summary>
        /// Internal implementation of <see cref="Load" />.
        /// </summary>
        /// <param name="packageConfig">The test package configuration</param>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        /// <returns>The test package data</returns>
        protected abstract TestPackageData InternalLoad(TestPackageConfig packageConfig, IProgressMonitor progressMonitor);

        /// <summary>
        /// Internal implementation of <see cref="Explore" />.
        /// </summary>
        /// <param name="options">The test enumeration options</param>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        /// <returns>The test model data</returns>
        protected abstract TestModelData InternalExplore(TestExplorationOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// Internal implementation of <see cref="Run" />.
        /// </summary>
        /// <param name="options">The test execution options</param>
        /// <param name="listener">The test listener for monitoring test execution</param>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        protected abstract void InternalRun(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor);

        /// <summary>
        /// Internal implementation of <see cref="Unload" />.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        protected abstract void InternalUnload(IProgressMonitor progressMonitor);

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