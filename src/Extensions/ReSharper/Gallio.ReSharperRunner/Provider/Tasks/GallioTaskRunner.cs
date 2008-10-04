// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Threading;
using Gallio.Loader;
using Gallio.ReSharperRunner.Provider.Facade;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Tasks
{
    /// <summary>
    /// A remote task runner for Gallio.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation creates a facade for the ReSharper interfaces used
    /// by Gallio then kicks off the work in a fresh AppDomain.  This ensures
    /// that the Gallio runtime environment is not polluted with test assemblies
    /// or ReSharper dependencies that may interfere with test execution.
    /// </para>
    /// </remarks>
    public class GallioTaskRunner : BaseFacadeTaskRunner
    {
        public GallioTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        protected override FacadeTaskResult Execute(IFacadeTaskServer server, FacadeTask task)
        {
            IGallioRemoteEnvironment environment = EnvironmentManager.GetSharedEnvironment();

            Type taskRunnerType = typeof(RemoteTaskRunner);
            IRemoteTaskRunner taskRunner = (IRemoteTaskRunner)environment.AppDomain.CreateInstanceAndUnwrap(
                taskRunnerType.Assembly.FullName, taskRunnerType.FullName);

            return taskRunner.Execute(server, task);
        }

        private interface IRemoteTaskRunner
        {
            FacadeTaskResult Execute(IFacadeTaskServer server, FacadeTask facadeTask);
        }

        private class RemoteTaskRunner : MarshalByRefObject, IRemoteTaskRunner
        {
            public FacadeTaskResult Execute(IFacadeTaskServer server, FacadeTask facadeTask)
            {
                return facadeTask.Execute(server);
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }
        }
    }
}
