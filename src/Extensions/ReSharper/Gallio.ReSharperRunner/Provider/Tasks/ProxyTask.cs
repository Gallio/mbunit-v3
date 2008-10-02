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
using System.Collections.ObjectModel;
using Gallio.Collections;

namespace Gallio.ReSharperRunner.Provider.Tasks
{
    /// <summary>
    /// Describes a task that can be executed within the Gallio runtime environment.
    /// </summary>
    /// <remarks>
    /// This proxy decouples Gallio's private AppDomain from the ReSharper interfaces.
    /// </remarks>
    [Serializable]
    internal abstract class ProxyTask
    {
        private static int nextId;
        private readonly int id;
        private IList<ProxyTask> children;

        /// <summary>
        /// Creates a proxy task.
        /// </summary>
        protected ProxyTask()
        {
            id = ++nextId;
        }

        /// <summary>
        /// Gets the children of the task.
        /// </summary>
        public IList<ProxyTask> Children
        {
            get { return new ReadOnlyCollection<ProxyTask>(children ?? EmptyArray<ProxyTask>.Instance); }
        }

        /// <summary>
        /// Executes the task and its children recursively.
        /// </summary>
        /// <param name="server">The task server</param>
        /// <returns>The task result</returns>
        public virtual ProxyTaskResult Execute(IProxyTaskServer server)
        {
            throw new NotSupportedException("This task is not executable.");
        }

        /// <summary>
        /// Adds a child task.
        /// </summary>
        /// <param name="child">The child task</param>
        public void AddChild(ProxyTask child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

            if (children == null)
                children = new List<ProxyTask>();
            children.Add(child);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            ProxyTask other = obj as ProxyTask;
            return other != null && id == other.id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return id;
        }
    }
}
