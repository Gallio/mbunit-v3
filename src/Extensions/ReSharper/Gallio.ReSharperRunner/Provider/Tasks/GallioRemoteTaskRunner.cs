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
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
using System.Reflection;
using Gallio.Loader;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Tasks
{
    /// <summary>
    /// A remote task runner for Gallio.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation starts off a fresh AppDomain for Gallio because the
    /// current AppDomain includes references to test assemblies some of which
    /// may conflict with Gallio assemblies.  It is careful not to access any
    /// core Gallio types except within the new AppDomain.
    /// </para>
    /// </remarks>
    /// <seealso cref="GallioRemoteTask"/> for important remarks related to tasks.
    public class GallioRemoteTaskRunner : RecursiveRemoteTaskRunner
    {
        private TaskResult executeResult;

        public GallioRemoteTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            executeResult = TaskResult.Error;
        }

        public override void ConfigureAppDomain(TaskAppDomainConfiguration configuration)
        {
            configuration.ApartmentState = ApartmentState.STA;
        }

        public override TaskResult Start(TaskExecutionNode node)
        {
            return TaskResult.Success; 
        }

        public override TaskResult Execute(TaskExecutionNode node)
        {
            throw new NotImplementedException("Should not be called.");
        }

        public override TaskResult Finish(TaskExecutionNode node)
        {
            return executeResult;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            IGallioRemoteEnvironment environment = EnvironmentManager.GetSharedEnvironment();

            Type taskRunnerType = typeof(RemoteProxyTaskRunner);
            IProxyTaskRunner taskRunner = (IProxyTaskRunner)environment.AppDomain.CreateInstanceAndUnwrap(
                taskRunnerType.Assembly.FullName, taskRunnerType.FullName);

            AdapterProxyTaskServer proxyTaskServer = new AdapterProxyTaskServer(Server);
            executeResult = proxyTaskServer.Execute(taskRunner, node);
        }
    }
}
