// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using System.Threading;
using Gallio.Loader;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// Runs remote tasks via the facade.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation creates a facade for the ReSharper interfaces used
    /// by Gallio then kicks off the work in a fresh AppDomain.  This ensures
    /// that the Gallio runtime environment is not polluted with test assemblies
    /// or ReSharper dependencies that may interfere with test execution.
    /// </para>
    /// <para>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </para>
    /// <para>
    /// Keep in mind that while this code is running none of the Gallio types outside of this
    /// assembly (and not in GAC, like Gallio.Loader) are accessible.  After bootstrapping
    /// a new AppDomain with the Gallio runtime, this class delegates to <see cref="RemoteFacadeTaskRunner"/>
    /// which will not have access to the ReSharper assemblies (hence this facade)!
    /// </para>
    /// </remarks>
    public class FacadeTaskRunner : RecursiveRemoteTaskRunner
    {
        private TaskResult executeResult;

        public FacadeTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            executeResult = TaskResult.Error;
        }

        sealed public override void ConfigureAppDomain(TaskAppDomainConfiguration configuration)
        {
            configuration.ApartmentState = ApartmentState.STA;
        }

        sealed public override TaskResult Start(TaskExecutionNode node)
        {
            return TaskResult.Success; 
        }

        sealed public override TaskResult Execute(TaskExecutionNode node)
        {
            throw new NotImplementedException("Should not be called.");
        }

        sealed public override TaskResult Finish(TaskExecutionNode node)
        {
            return executeResult;
        }

        sealed public override void ExecuteRecursive(TaskExecutionNode node)
        {
            AdapterFacadeTaskServer facadeTaskServer = new AdapterFacadeTaskServer(Server);
            AdapterFacadeLogger facadeLogger = new AdapterFacadeLogger();

            FacadeTaskExecutorConfiguration facadeTaskExecutorConfiguration = new FacadeTaskExecutorConfiguration()
            {
                ShadowCopy = TaskExecutor.Configuration.ShadowCopy,
                AssemblyFolder = TaskExecutor.Configuration.AssemblyFolder
            };

            FacadeTask facadeTask = facadeTaskServer.MapTasks(node);
            executeResult = FacadeUtils.ToTaskResult(Execute(facadeTaskServer, facadeLogger, facadeTask, facadeTaskExecutorConfiguration));
        }

        protected virtual FacadeTaskResult Execute(IFacadeTaskServer facadeTaskServer, IFacadeLogger facadeLogger, FacadeTask facadeTask, FacadeTaskExecutorConfiguration facadeTaskExecutorConfiguration)
        {
            IGallioRemoteEnvironment environment = EnvironmentManager.GetSharedEnvironment();

            Type taskRunnerType = typeof(RemoteFacadeTaskRunner);
            IRemoteFacadeTaskRunner taskRunner = (IRemoteFacadeTaskRunner)environment.AppDomain.CreateInstanceAndUnwrap(
                taskRunnerType.Assembly.FullName, taskRunnerType.FullName);

            return taskRunner.Execute(facadeTaskServer, facadeLogger, facadeTask, facadeTaskExecutorConfiguration);
        }
    }
}
