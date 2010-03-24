// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// Wraps a <see cref="FacadeTask" /> to represent it as a <see cref="RemoteTask"/> for ReSharper.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class FacadeTaskWrapper : RemoteTask, IEquatable<FacadeTaskWrapper>
    {
        private readonly FacadeTask facadeTask;

        internal FacadeTaskWrapper(FacadeTask facadeTask)
            : base(GallioTestProvider.ProviderId)
        {
            this.facadeTask = facadeTask;
        }

        public FacadeTaskWrapper(XmlElement element)
            : base(element)
        {
            string typeName = element.GetAttribute("facadeTaskType");
            facadeTask = (FacadeTask) Activator.CreateInstance(typeof(FacadeTask).Assembly.GetType(typeName), new object[] { element });
        }

        public bool Equals(FacadeTaskWrapper other)
        {
            return other != null
                && RunnerID == other.RunnerID
                && facadeTask.Equals(other.facadeTask);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as FacadeTaskWrapper);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FacadeTaskWrapper);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ facadeTask.GetHashCode();
        }

        public override void SaveXml(XmlElement element)
        {
            element.SetAttribute("facadeTaskType", facadeTask.GetType().FullName);
            facadeTask.SaveXml(element);
        }

        public FacadeTask FacadeTask
        {
            get { return facadeTask; }
        }
    }
}
