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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Gallio.Loader;
using Gallio.Model;
using Gallio.Model.Serialization;
using Microsoft.VisualStudio.TestTools.Common;
using System.Runtime.InteropServices;

namespace Gallio.MSTestRunner
{
    [Serializable]
    [Guid(Guids.GallioTestTypeGuidString)]
    internal sealed class GallioTestElement : TestElement
    {
        [PersistenceElementName("gallioTestId")]
        private string gallioTestId;

        static GallioTestElement()
        {
            GallioAssemblyResolver.Install(typeof(GallioPackage).Assembly);
        }

        public GallioTestElement(TestData test, string assemblyPath, ProjectData projectData)
            : base(GenerateTestId(test), test.Name, test.Metadata.GetValue(MetadataKeys.Description) ?? "", assemblyPath)
        {
            foreach (KeyValuePair<string, IList<string>> pair in test.Metadata)
            {
                Properties[pair.Key] = pair.Value.Count == 1 ? (object)pair.Value[0] : pair.Value;
            }

            Owner = test.Metadata.GetValue(MetadataKeys.AuthorName) ?? "";

            gallioTestId = test.Id;
            ProjectData = projectData;
        }

        public GallioTestElement(GallioTestElement element)
            : base(element)
        {
            gallioTestId = element.gallioTestId;
        }

        private GallioTestElement(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override object Clone()
        {
            return new GallioTestElement(this);
        }

        public string GallioTestId
        {
            get { return gallioTestId; }
        }

        public override string HumanReadableId
        {
            get { return Name; }
        }

        public override string Adapter
        {
            get { return typeof(GallioTestAdapter).AssemblyQualifiedName; }
        }

        public override bool CanBeAggregated
        {
            get { return false; }
        }

        public override bool IsLoadTestCandidate
        {
            get { return false; }
        }

        public override string ControllerPlugin
        {
            get { return null; }
        }

        public override bool ReadOnly
        {
            get { return false; }
            set { throw new NotSupportedException(); }
        }

        public override TestType TestType
        {
            get { return Guids.GallioTestType; }
        }

        private static TestId GenerateTestId(TestData test)
        {
            Guid guid = new Guid(Encoding.ASCII.GetBytes(test.Id.PadRight(16)));
            return new TestId(guid);
        }
    }
}
