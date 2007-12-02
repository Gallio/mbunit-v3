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

namespace Gallio.ReSharperRunner.Tasks
{
    [Serializable]
    public class GallioTestRunTask : GallioRemoteTask
    {
        private readonly string testId;
        private readonly string assemblyLocation;

        public GallioTestRunTask(string testId, string assemblyLocation)
        {
            this.testId = testId;
            this.assemblyLocation = assemblyLocation;
        }

        public GallioTestRunTask(XmlElement element)
            : base(element)
        {
            testId = GetXmlAttribute(element, "TestId");
            assemblyLocation = GetXmlAttribute(element, "AssemblyLocation");
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            SetXmlAttribute(element, "TestId", testId);
            SetXmlAttribute(element, "AssemblyLocation", assemblyLocation);
        }

        public override GallioRemoteAction CreateAction()
        {
            return new GallioTestRunAction(this);
        }
    }
}
