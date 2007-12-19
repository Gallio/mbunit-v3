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
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Tasks
{
    /// <summary>
    /// A remote task that is intended to be executed within the Gallio
    /// runtime environment.
    /// </summary>
    /// <remarks>
    /// <para>
    /// At the time when this task is instantiated and deserialized, the Gallio
    /// runtime environment has yet to be initialized.  One consequence of this
    /// fact is that it might not be possible to resolve types in the Gallio assemblies.
    /// </para>
    /// <para>
    /// While we could try to ensure that these assemblies are pre-loaded using
    /// a <see cref="IsolatedAssemblyTask" />, that strategy may be somewhat
    /// brittle because it depends on knowing ahead of time all of the possible
    /// dependencies.  So in the end it turns out to be simpler to just assume
    /// that no Gallio types other than the ones defined in this assembly can
    /// be resolved.
    /// </para>
    /// <para>
    /// One consequence of this policy is that the tasks cannot contain any
    /// members of Gallio types.  So we delegate processing to a <see cref="GallioRemoteAction" />
    /// which is instantiated on demand by the <see cref="GallioRemoteTaskRunner" />.
    /// </para>
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
        public TaskResult ExecuteRecursive(IRemoteTaskServer server, TaskExecutionNode node)
        {
            return CreateAction().ExecuteRecursive(server, node);
        }

        /// <summary>
        /// Gets the action to run.
        /// </summary>
        /// <returns>The action</returns>
        public virtual GallioRemoteAction CreateAction()
        {
            throw new NotSupportedException("This task does not have an executable remote action.");
        }
    }
}
