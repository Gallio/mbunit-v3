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
using System.Text;
using Gallio.Model;
using Gallio.Model.Serialization;
using Microsoft.VisualStudio.TestTools.Common;

namespace Gallio.MSTestRunner
{
    internal static class GallioTestElementFactory
    {
        public static GallioTestElement CreateTestElement(TestData test, string assemblyPath, ProjectData projectData)
        {
            GallioTestElement testElement = new GallioTestElement(test.Id, test.Name,
                test.Metadata.GetValue(MetadataKeys.Description) ?? "", assemblyPath);
            testElement.ProjectData = projectData;

            foreach (KeyValuePair<string, IList<string>> pair in test.Metadata)
                testElement.Properties[pair.Key] = pair.Value.Count == 1 ? (object)pair.Value[0] : pair.Value;

            testElement.Owner = test.Metadata.GetValue(MetadataKeys.AuthorName) ?? "";

            testElement.SetCodeReference(test.CodeReference.AssemblyName, test.CodeReference.NamespaceName,
                test.CodeReference.TypeName, test.CodeReference.MemberName, test.CodeReference.ParameterName);
            testElement.SetCodeLocation(test.CodeLocation.ToString());
            return testElement;
        }
    }
}
