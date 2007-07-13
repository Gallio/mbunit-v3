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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MbUnit.Core.Serialization;

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
            Disconnect();
        }

        /// <inheritdoc />
        protected override void InternalLoadProject(TestProjectInfo project)
        {
            Connect();

            try
            {
                proxy.LoadProject(project);
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to load the project in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalBuildTestTemplates()
        {
            try
            {
                proxy.BuildTestTemplates();
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to build test templates in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalBuildTests()
        {
            try
            {
                proxy.BuildTests();
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to build tests in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalRunTests()
        {
            try
            {
                proxy.RunTests();
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to run tests in the remote test domain.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalUnloadProject()
        {
            try
            {
                if (proxy != null)
                    proxy.UnloadProject();
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to unload the project in the remote test domain.", ex);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Connects to the remote test domain and returns a proxy for the remote instance.
        /// </summary>
        /// <returns>A proxy for the remote test domain instance</returns>
        protected abstract ITestDomain InternalConnect();

        /// <summary>
        /// Disconnects from the remote test domain.
        /// </summary>
        protected abstract void InternalDisconnect();

        private void Connect()
        {
            try
            {
                proxy = InternalConnect();
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Failed to connect to the remote test domain.", ex);
            }
        }

        private void Disconnect()
        {
            try
            {
                InternalDisconnect();
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
