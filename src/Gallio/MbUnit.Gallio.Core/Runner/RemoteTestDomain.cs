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
using System.Diagnostics;
using MbUnit.Core.Harness;
using MbUnit.Core.Model;
using MbUnit.Core.Model.Events;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Runner
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
        /// Gets a proxy for the remote test domain instance.
        /// </summary>
        public ITestDomain Proxy
        {
            get { return proxy; }
        }

        /// <inheritdoc />
        protected override void InternalDispose()
        {
            Disconnect(new NullProgressMonitor());
        }

        /// <inheritdoc />
        protected override void InternalLoadPackage(TestPackage package, IProgressMonitor progressMonitor)
        {
            Connect(new SubProgressMonitor(progressMonitor, 0.1));

            try
            {
                proxy.LoadPackage(package, new RemoteProgressMonitor(new SubProgressMonitor(progressMonitor, 0.9)));
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to load the project in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalBuildTemplates(TemplateEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            try
            {
                TemplateModel = null;
                proxy.BuildTemplates(options, new RemoteProgressMonitor(new SubProgressMonitor(progressMonitor, 1)));
                TemplateModel = proxy.TemplateModel;
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to build templates in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalBuildTests(TestEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            try
            {
                TestModel = null;
                proxy.BuildTests(options, new RemoteProgressMonitor(new SubProgressMonitor(progressMonitor, 1)));
                TestModel = proxy.TestModel;
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to build tests in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalRunTests(TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            try
            {
                proxy.RunTests(new RemoteProgressMonitor(new SubProgressMonitor(progressMonitor, 1)), options);
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to run tests in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalUnloadPackage(IProgressMonitor progressMonitor)
        {
            try
            {
                if (proxy != null)
                    proxy.UnloadPackage(new SubProgressMonitor(progressMonitor, 0.9));
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to unload the project in the remote test domain.", ex);
            }
            finally
            {
                Disconnect(new SubProgressMonitor(progressMonitor, 0.1));
            }
        }

        /// <summary>
        /// Connects to the remote test domain and returns a proxy for the remote instance.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        /// <returns>A proxy for the remote test domain instance</returns>
        protected abstract ITestDomain InternalConnect(IProgressMonitor progressMonitor);

        /// <summary>
        /// Disconnects from the remote test domain.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor with 1 work unit to do</param>
        protected abstract void InternalDisconnect(IProgressMonitor progressMonitor);

        private void Connect(IProgressMonitor progressMonitor)
        {
            try
            {
                using (progressMonitor)
                {
                    progressMonitor.BeginTask("Connecting to the remote test domain.", 1);
                    proxy = InternalConnect(progressMonitor);

                    if (Listener != null)
                        proxy.SetTestListener(new RemoteTestListener(Listener));
                }
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to connect to the remote test domain.", ex);
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
