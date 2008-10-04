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
using System.Collections.Generic;
using System.Reflection;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// A facade and remote proxy for the ReSharper task server interface.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </remarks>
    public class AdapterFacadeTaskServer : MarshalByRefObject, IFacadeTaskServer
    {
        private readonly IRemoteTaskServer server;
        private readonly List<RemoteTask> remoteTasks;

        public AdapterFacadeTaskServer(IRemoteTaskServer server)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            this.server = server;
            remoteTasks = new List<RemoteTask>();
        }

        /// <summary>
        /// Recursively maps a tree of task execution nodes to facade tasks.
        /// The server will retain references to the nodes and the tasks for the
        /// duration of its lifetime so that it can perform an inverse mapping as
        /// required.
        /// </summary>
        /// <param name="node">The execution node to map</param>
        /// <returns>The facade task tree</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is null</exception>
        public FacadeTask MapTasks(TaskExecutionNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            FacadeTaskWrapper remoteTask = (FacadeTaskWrapper)node.RemoteTask;
            FacadeTask facadeTask = remoteTask.FacadeTask;

            facadeTask.RemoteTaskHandle = remoteTasks.Count;
            remoteTasks.Add(remoteTask);

            foreach (TaskExecutionNode child in node.Children)
                facadeTask.AddChild(MapTasks(child));

            return facadeTask;
        }

        /// <summary>
        /// Gets the remote task that corresponds to a particular facade task.
        /// </summary>
        /// <param name="task">The facade task</param>
        /// <returns>The corresponding remote task</returns>
        public RemoteTask GetRemoteTask(FacadeTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return remoteTasks[task.RemoteTaskHandle];
        }

        /// <inheritdoc />
        public string SessionId
        {
            get
            {
                // TODO: Should ask for a better way of doing this.
                object taskRunnerProxy = server.WithoutProxy;
#if RESHARPER_31
                PropertyInfo property = taskRunnerProxy.GetType().GetProperty("SessionId");
                return (string)property.GetValue(taskRunnerProxy, null);
#else
                return ((TaskRunnerProxy) taskRunnerProxy).SessionId;
#endif
            }
        }

        /// <inheritdoc />
        public void TaskError(FacadeTask task, string message)
        {
            server.TaskError(GetRemoteTask(task), message);
        }

        /// <inheritdoc />
        public void TaskException(FacadeTask task, FacadeTaskException[] exceptions)
        {
            TaskException[] nativeExceptions = Array.ConvertAll<FacadeTaskException, TaskException>(exceptions, FacadeUtils.ToTaskException);
            server.TaskException(GetRemoteTask(task), nativeExceptions);
        }

        /// <inheritdoc />
        public void TaskExplain(FacadeTask task, string explanation)
        {
            server.TaskExplain(GetRemoteTask(task), explanation);
        }

        /// <inheritdoc />
        public void TaskFinished(FacadeTask task, string message, FacadeTaskResult result)
        {
            server.TaskFinished(GetRemoteTask(task), message, FacadeUtils.ToTaskResult(result));
        }

        /// <inheritdoc />
        public void TaskOutput(FacadeTask task, string text, FacadeTaskOutputType outputType)
        {
            server.TaskOutput(GetRemoteTask(task), text, FacadeUtils.ToTaskOutputType(outputType));
        }

        /// <inheritdoc />
        public void TaskProgress(FacadeTask task, string message)
        {
            server.TaskProgress(GetRemoteTask(task), message);
        }

        /// <inheritdoc />
        public void TaskStarting(FacadeTask task)
        {
            server.TaskStarting(GetRemoteTask(task));
        }
    }
}