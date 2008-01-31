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
using System.Diagnostics;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Model.Execution;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Model;
using Gallio.Runner.Domains;

namespace Gallio.Runner.Domains
{
    /// <summary>
    /// An implementation of <see cref="ITestDomain" /> designed to access
    /// services provided by a remote instance.  The implementation establishes
    /// a remote connection when a project is loaded and releases it when
    /// a project is unloaded or when the domain is disposed.
    /// </summary>
    public abstract class RemoteTestDomain : BaseTestDomain
    {
        private ITestDomain proxy;

        /// <summary>
        /// Creates a remote test domain initially in a disconnected
        /// state (without a proxy).
        /// </summary>
        protected RemoteTestDomain()
        {
        }

        /// <summary>
        /// Gets a proxy for the remote test domain instance.
        /// </summary>
        public ITestDomain Proxy
        {
            get { return proxy; }
        }

        /// <inheritdoc />
        protected override void InternalDispose()
        {
            Disconnect(NullProgressMonitor.CreateInstance());
        }

        /// <inheritdoc />
        protected override TestPackageData InternalLoadTestPackage(TestPackageConfig packageConfig, IProgressMonitor progressMonitor)
        {
            Connect(packageConfig, progressMonitor.CreateSubProgressMonitor(0.1));

            try
            {
                proxy.LoadTestPackage(packageConfig, new RemoteProgressMonitor(progressMonitor.CreateSubProgressMonitor(0.9)));
                return proxy.TestPackageData; 
            }
            catch (Exception ex)
            {
                throw new RunnerException("Failed to load the test package in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override TestModelData InternalBuildTestModel(TestEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            try
            {
                proxy.BuildTestModel(options, new RemoteProgressMonitor(progressMonitor.CreateSubProgressMonitor(1)));
                return proxy.TestModelData;
            }
            catch (Exception ex)
            {
                throw new RunnerException("Failed to build the test model in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalRunTests(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor)
        {
            try
            {
                proxy.RunTests(options,
                    new RemoteTestListener(listener),
                    new RemoteProgressMonitor(progressMonitor.CreateSubProgressMonitor(1)));
            }
            catch (Exception ex)
            {
                throw new RunnerException("Failed to run tests in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalUnloadTestPackage(IProgressMonitor progressMonitor)
        {
            try
            {
                if (proxy != null)
                    proxy.UnloadPackage(new RemoteProgressMonitor(progressMonitor.CreateSubProgressMonitor(0.9)));
            }
            catch (Exception ex)
            {
                throw new RunnerException("Failed to unload the project in the remote test domain.", ex);
            }
            finally
            {
                Disconnect(progressMonitor.CreateSubProgressMonitor(0.1));
            }
        }

        /// <summary>
        /// Connects to the remote test domain and returns a proxy for the remote instance.
        /// </summary>
        /// <param name="packageConfig">The test package configuration</param>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        /// <returns>A proxy for the remote test domain instance</returns>
        protected abstract ITestDomain InternalConnect(TestPackageConfig packageConfig, IProgressMonitor progressMonitor);

        /// <summary>
        /// Disconnects from the remote test domain.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        protected abstract void InternalDisconnect(IProgressMonitor progressMonitor);

        private void Connect(TestPackageConfig packageConfig, IProgressMonitor progressMonitor)
        {
            try
            {
                using (progressMonitor)
                {
                    progressMonitor.BeginTask("Connecting to the remote test domain.", 1);
                    proxy = InternalConnect(packageConfig, progressMonitor);
                }
            }
            catch (Exception ex)
            {
                throw new RunnerException("Failed to connect to the remote test domain.", ex);
            }
        }

        private void Disconnect(IProgressMonitor progressMonitor)
        {
            try
            {
                using (progressMonitor)
                {
                    progressMonitor.BeginTask("Disconnecting from the remote test domain.", 1);
                    InternalDisconnect(progressMonitor);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to safely disconnect from remote test domain: " + ex);
            }
            finally
            {
                proxy = null;
            }
        }
    }
}