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

namespace Gallio.ReSharperRunner.Provider.Tasks
{
    internal class AdapterProxyTaskServer : MarshalByRefObject, IProxyTaskServer
    {
        private readonly IRemoteTaskServer server;
        private readonly Dictionary<ProxyTask, RemoteTask> taskMap;

        public AdapterProxyTaskServer(IRemoteTaskServer server)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            this.server = server;
            taskMap = new Dictionary<ProxyTask, RemoteTask>();
        }

        /// <summary>
        /// Executes the task execution node using the specified runner.
        /// </summary>
        /// <param name="runner">The proxy task runner</param>
        /// <param name="node">The task execution node to execute</param>
        /// <returns>The result</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runner"/>
        /// or <paramref name="node"/> is null</exception>
        public TaskResult Execute(IProxyTaskRunner runner, TaskExecutionNode node)
        {
            if (runner == null)
                throw new ArgumentNullException("runner");

            try
            {
                ProxyTask proxyTask = MapTaskTree(node);
                ProxyTaskResult result = runner.Execute(this, proxyTask);
                return ToTaskResult(result);
            }
            finally
            {
                taskMap.Clear();
            }
        }

        /// <summary>
        /// Recursively maps a tree of task execution nodes to proxy tasks.
        /// The server will retain references to the nodes and the tasks for the
        /// duration of its lifetime so that it can perform an inverse mapping as
        /// required.
        /// </summary>
        /// <param name="node">The execution node to map</param>
        /// <returns>The proxy task tree</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is null</exception>
        private ProxyTask MapTaskTree(TaskExecutionNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            GallioRemoteTask remoteTask = (GallioRemoteTask) node.RemoteTask;
            ProxyTask proxyTask = remoteTask.CreateProxyTask();

            taskMap[proxyTask] = remoteTask;

            foreach (TaskExecutionNode child in node.Children)
                proxyTask.AddChild(MapTaskTree(child));

            return proxyTask;
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
        public void TaskError(ProxyTask task, string message)
        {
            server.TaskError(ToRemoteTask(task), message);
        }

        /// <inheritdoc />
        public void TaskException(ProxyTask task, ProxyTaskException[] exceptions)
        {
            TaskException[] nativeExceptions = Array.ConvertAll<ProxyTaskException, TaskException>(exceptions, ToTaskException);
            server.TaskException(ToRemoteTask(task), nativeExceptions);
        }

        /// <inheritdoc />
        public void TaskExplain(ProxyTask task, string explanation)
        {
            server.TaskExplain(ToRemoteTask(task), explanation);
        }

        /// <inheritdoc />
        public void TaskFinished(ProxyTask task, string message, ProxyTaskResult result)
        {
            server.TaskFinished(ToRemoteTask(task), message, ToTaskResult(result));
        }

        /// <inheritdoc />
        public void TaskOutput(ProxyTask task, string text, ProxyTaskOutputType outputType)
        {
            server.TaskOutput(ToRemoteTask(task), text, ToTaskOutputType(outputType));
        }

        /// <inheritdoc />
        public void TaskProgress(ProxyTask task, string message)
        {
            server.TaskProgress(ToRemoteTask(task), message);
        }

        /// <inheritdoc />
        public void TaskStarting(ProxyTask task)
        {
            server.TaskStarting(ToRemoteTask(task));
        }

        private RemoteTask ToRemoteTask(ProxyTask task)
        {
            return taskMap[task];
        }

        private static TaskException ToTaskException(ProxyTaskException exception)
        {
            return TaskExceptionFactory.CreateTaskException(exception.Type, exception.Message, exception.StackTrace);
        }

        private static TaskResult ToTaskResult(ProxyTaskResult result)
        {
            switch (result)
            {
                case ProxyTaskResult.Success:
                    return TaskResult.Success;

                case ProxyTaskResult.Skipped:
                    return TaskResult.Skipped;

                case ProxyTaskResult.Error:
                    return TaskResult.Error;

                case ProxyTaskResult.Exception:
                    return TaskResult.Exception;

                default:
                    throw new ArgumentOutOfRangeException("result");
            }
        }

        private static TaskOutputType ToTaskOutputType(ProxyTaskOutputType outputType)
        {
            switch (outputType)
            {
                case ProxyTaskOutputType.StandardOutput:
                    return TaskOutputType.STDOUT;

                case ProxyTaskOutputType.StandardError:
                    return TaskOutputType.STDERR;

                case ProxyTaskOutputType.DebugTrace:
                    return TaskOutputType.DEBUGTRACE;

                default:
                    throw new ArgumentOutOfRangeException("outputType");
            }
        }
    }
}
