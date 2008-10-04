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
using System.Xml;
using Gallio.ReSharperRunner.Provider.Facade;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Tasks
{
    /// <summary>
    /// This is the root task for running Gallio tests.
    /// It must always appear first in a task sequence followed by
    /// any number <see cref="GallioTestItemTask" /> instances that describe the work to
    /// be done.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Equality comparison is used by ReSharper to coalesce sequences of tasks into a tree.
    /// Sequential tasks form a chain of nested nodes.  When identical tasks are found they are
    /// combined and subsequent tasks in the sequence become children of the common ancestor.
    /// </para>
    /// <para>
    /// To ensure that we have full control over the node structure, we introduce a root task
    /// whose purpose is to gather all of the constituent tasks under a common parent.
    /// </para>
    /// </remarks>
    [Serializable]
    public class GallioTestRunTask : FacadeTask, IEquatable<GallioTestRunTask>
    {
        /// <summary>
        /// Gets a shared instance of the task.
        /// </summary>
        public static readonly GallioTestRunTask Instance = new GallioTestRunTask();

        public GallioTestRunTask()
        {
        }

        public GallioTestRunTask(XmlElement element)
            : base(element)
        {
        }

        public bool Equals(GallioTestRunTask other)
        {
            return other != null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GallioTestRunTask);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override FacadeTaskResult Execute(IFacadeTaskServer server)
        {
            return new GallioTestRunner(server).Run(this);
        }
    }
}
