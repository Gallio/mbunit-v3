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
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Tasks
{
    /// <summary>
    /// A remote task that is intended to be executed within the Gallio
    /// runtime environment.
    /// </summary>
    /// </remarks>
    [Serializable]
    public abstract class GallioRemoteTask : RemoteTask
    {
        protected GallioRemoteTask()
            : base(GallioTestProvider.ProviderId)
        {
        }

        protected GallioRemoteTask(XmlElement element) : base(element)
        {
        }
        
        /// <summary>
        /// Executes the task recursively.
        /// </summary>
        /// <param name="server">The remote task server</param>
        /// <param name="node">The task execution node</param>
        /// <returns>The execution result</returns>
        public virtual TaskResult ExecuteRecursive(IRemoteTaskServer server, TaskExecutionNode node)
        {
            throw new NotSupportedException("This task does not have an executable remote action.");
        }
    }
}
