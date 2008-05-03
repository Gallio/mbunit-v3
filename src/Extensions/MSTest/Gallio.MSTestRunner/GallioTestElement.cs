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
using System.Runtime.Serialization;
using System.Text;
using Gallio.MSTestRunner.Resources;
using Microsoft.VisualStudio.TestTools.Common;
using System.Runtime.InteropServices;

namespace Gallio.MSTestRunner
{
    [Serializable]
    [Guid(Guids.GallioTestTypeGuidString)]
    internal sealed class GallioTestElement : TestElement
    {
        private const string GallioTestIdKey = "Gallio.TestId";
        private const string AssemblyNameKey = "Gallio.AssemblyName";
        private const string NamespaceNameKey = "Gallio.NamespaceName";
        private const string TypeNameKey = "Gallio.TypeName";
        private const string MemberNameKey = "Gallio.MemberName";
        private const string ParameterNameKey = "Gallio.ParameterName";
        private const string LocationKey = "Gallio.Location";

        [PersistenceElementName("gallioTestId")]
        private string gallioTestId;

        [PersistenceElementName("assemblyName")]
        private string assemblyName;

        [PersistenceElementName("namespaceName")]
        private string namespaceName;

        [PersistenceElementName("typeName")]
        private string typeName;

        [PersistenceElementName("memberName")]
        private string memberName;

        [PersistenceElementName("parameterName")]
        private string parameterName;

        [PersistenceElementName("location")]
        private string location;

        public GallioTestElement(string id, string name, string description, string assemblyPath)
            : base(GenerateTestId(id), name, description, assemblyPath)
        {
            gallioTestId = id;
        }

        public GallioTestElement(GallioTestElement element)
            : base(element)
        {
            gallioTestId = element.gallioTestId;
            assemblyName = element.assemblyName;
            namespaceName = element.namespaceName;
            typeName = element.typeName;
            memberName = element.memberName;
            parameterName = element.parameterName;
            location = element.location;
        }

        private GallioTestElement(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            gallioTestId = info.GetString(GallioTestIdKey);
            assemblyName = info.GetString(AssemblyNameKey);
            namespaceName = info.GetString(NamespaceNameKey);
            typeName = info.GetString(TypeNameKey);
            memberName = info.GetString(MemberNameKey);
            parameterName = info.GetString(ParameterNameKey);
            location = info.GetString(LocationKey);
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

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(GallioTestIdKey, gallioTestId);
            info.AddValue(AssemblyNameKey, assemblyName);
            info.AddValue(NamespaceNameKey, namespaceName);
            info.AddValue(TypeNameKey, typeName);
            info.AddValue(MemberNameKey, memberName);
            info.AddValue(ParameterNameKey, parameterName);
            info.AddValue(LocationKey, location);
        }

        [PropertyWindow]
        [UserVisibleProperty("{2b5a8f49-d24c-4296-8b24-c96c42b707eb}")]
        [TestCaseManagementDisplayName(typeof(VSPackage), VSPackageResourceIds.AssemblyNamePropertyNameKey)]
        [LocalizedDescription(typeof(VSPackage), VSPackageResourceIds.AssemblyNamePropertyDescriptionKey)]
        [GroupingProperty]
        //[HelpKeyword("key")]
        public string AssemblyName
        {
            get { return assemblyName; }
        }

        [PropertyWindow]
        [UserVisibleProperty("{a6c1dfdb-bb98-4e3b-9bb5-523e4550e82f}")]
        [TestCaseManagementDisplayName(typeof(VSPackage), VSPackageResourceIds.TypeNamePropertyNameKey)]
        [LocalizedDescription(typeof(VSPackage), VSPackageResourceIds.TypeNamePropertyDescriptionKey)]
        [GroupingProperty]
        //[HelpKeyword("key")]
        public string TypeName
        {
            get { return typeName; }
        }

        [PropertyWindow]
        [UserVisibleProperty("{81728549-1eb7-4df1-9ce6-7f9836c9581f}")]
        [TestCaseManagementDisplayName(typeof(VSPackage), VSPackageResourceIds.MemberNamePropertyNameKey)]
        [LocalizedDescription(typeof(VSPackage), VSPackageResourceIds.MemberNamePropertyDescriptionKey)]
        [GroupingProperty]
        //[HelpKeyword("key")]
        public string MemberName
        {
            get { return memberName; }
        }

        [PropertyWindow]
        [UserVisibleProperty("{36ee7063-fa7f-4ae2-87f5-b93d69ec1657}")]
        [TestCaseManagementDisplayName(typeof(VSPackage), VSPackageResourceIds.LocationPropertyNameKey)]
        [LocalizedDescription(typeof(VSPackage), VSPackageResourceIds.LocationPropertyDescriptionKey)]
        [GroupingProperty]
        //[HelpKeyword("key")]
        public string Location
        {
            get { return location; }
        }

        internal void SetCodeReference(string assemblyName, string namespaceName, string typeName, string memberName, string parameterName)
        {
            this.assemblyName = assemblyName;
            this.namespaceName = namespaceName;
            this.typeName = typeName;
            this.memberName = memberName;
            this.parameterName = parameterName;
        }

        internal void SetCodeLocation(string location)
        {
            this.location = location;
        }

        private static TestId GenerateTestId(string testId)
        {
            Guid guid = new Guid(Encoding.ASCII.GetBytes(testId.PadRight(16)));
            return new TestId(guid);
        }
    }
}
