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
