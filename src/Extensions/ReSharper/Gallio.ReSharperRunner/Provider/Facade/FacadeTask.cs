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
using System.Xml;
using Gallio.Collections;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// Describes a task that can be executed within the Gallio runtime environment.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </remarks>
    [Serializable]
    public abstract class FacadeTask
    {
        private IList<FacadeTask> children;

        /// <summary>
        /// Gets or sets an internal handle to the corresponding remote task.
        /// </summary>
        internal int RemoteTaskHandle { get; set; }

        /// <summary>
        /// Creates a facade task.
        /// </summary>
        protected FacadeTask()
        {
        }

        /// <summary>
        /// Deserializes a facade task from Xml.
        /// </summary>
        /// <param name="element">The xml element</param>
        protected FacadeTask(XmlElement element)
        {
        }

        /// <summary>
        /// Gets the children of the task.
        /// </summary>
        public IList<FacadeTask> Children
        {
            get { return new ReadOnlyCollection<FacadeTask>(children ?? EmptyArray<FacadeTask>.Instance); }
        }

        /// <summary>
        /// Executes the task and its children recursively.
        /// </summary>
        /// <param name="server">The task server</param>
        /// <param name="config">The task executor configuration</param>
        /// <returns>The task result</returns>
        public virtual FacadeTaskResult Execute(IFacadeTaskServer server, FacadeTaskExecutorConfiguration config)
        {
            throw new NotSupportedException("This task is not executable.");
        }

        /// <summary>
        /// Serializes a facade task to Xml.
        /// </summary>
        /// <param name="element">The xml element</param>
        public virtual void SaveXml(XmlElement element)
        {
        }

        /// <summary>
        /// Adds a child task.
        /// </summary>
        /// <param name="child">The child task</param>
        public void AddChild(FacadeTask child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

            if (children == null)
                children = new List<FacadeTask>();
            children.Add(child);
        }
    }
}