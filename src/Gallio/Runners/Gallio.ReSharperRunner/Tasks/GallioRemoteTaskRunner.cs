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
using Gallio.Hosting;
using Gallio.ReSharperRunner.Hosting;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Tasks
{
    /// <summary>
    /// A remote task runner for Gallio.
    /// </summary>
    /// <remarks>
    /// This implementation is careful to initialize the <see cref="Runtime" />
    /// before doing anything else because it's possible that the Gallio
    /// assemblies cannot yet be resolved.
    /// </remarks>
    /// <seealso cref="GallioRemoteTask"/> for important remarks related to tasks.
    public class GallioRemoteTaskRunner : RecursiveRemoteTaskRunner
    {
        public GallioRemoteTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        public override void ConfigureAppDomain(TaskAppDomainConfiguration configuration)
        {
        }

        public override TaskResult Start(TaskExecutionNode node)
        {
            RuntimeProxy.Initialize();
            return TaskResult.Success; 
        }

        public override TaskResult Execute(TaskExecutionNode node)
        {
            throw new NotImplementedException("Should not be called.");
        }

        public override TaskResult Finish(TaskExecutionNode node)
        {
            return RuntimeProxy.IsInitialized ? TaskResult.Success : TaskResult.Error;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            GallioRemoteTask rootTask = (GallioRemoteTask)node.RemoteTask;
            rootTask.Execute(Server, node);
        }
    }
}
