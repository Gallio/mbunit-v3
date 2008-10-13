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
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Gallio.VisualStudio.Tip.Resources;
using Microsoft.VisualStudio.TestTools.Common;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.Common.Xml;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Represents a Gallio test element.
    /// </summary>
    /// <remarks>
    /// Be VERY careful not to refer to any Gallio types that are not
    /// in the Tip Proxy assembly.  They are not in the GAC, consequently Visual
    /// Studio may be unable to load them when it needs to pass the test element instance
    /// across a remoting channel.
    /// </remarks>
    [Serializable]
    [Guid(Guids.GallioTestTypeGuidString)]
    public sealed class GallioTestElement : TestElement
    {
        private const string GallioTestIdKey = "Gallio.TestId";
        private const string AssemblyNameKey = "Gallio.AssemblyName";
        private const string NamespaceNameKey = "Gallio.NamespaceName";
        private const string TypeNameKey = "Gallio.TypeName";
        private const string MemberNameKey = "Gallio.MemberName";
        private const string ParameterNameKey = "Gallio.ParameterName";
        private const string PathKey = "Gallio.Path";
        private const string LineKey = "Gallio.Line";
        private const string ColumnKey = "Gallio.Column";

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

        [PersistenceElementName("path")]
        private string path;

        [PersistenceElementName("line")]
        private int line;

        [PersistenceElementName("column")]
        private int column;

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
            path = element.path;
            line = element.line;
            column = element.column;
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
            path = info.GetString(PathKey);
            line = info.GetInt32(LineKey);
            column = info.GetInt32(ColumnKey);
        }

        public override object Clone()
        {
            return new GallioTestElement(this);
        }

        public string AssemblyPath
        {
            get { return Storage; }
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
            get { return typeof(GallioTestAdapterProxy).AssemblyQualifiedName; }
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
            info.AddValue(PathKey, path);
            info.AddValue(LineKey, line);
            info.AddValue(ColumnKey, column);
        }

        public override void Load(XmlElement element, XmlTestStoreParameters parameters)
        {
            base.Load(element, parameters);

            gallioTestId = XmlPersistenceUtils.LoadFromAttribute(element, GallioTestIdKey);
            assemblyName = XmlPersistenceUtils.LoadFromAttribute(element, AssemblyNameKey);
            namespaceName = XmlPersistenceUtils.LoadFromAttribute(element, NamespaceNameKey);
            typeName = XmlPersistenceUtils.LoadFromAttribute(element, TypeNameKey);
            memberName = XmlPersistenceUtils.LoadFromAttribute(element, MemberNameKey);
            parameterName = XmlPersistenceUtils.LoadFromAttribute(element, ParameterNameKey);
            path = XmlPersistenceUtils.LoadFromAttribute(element, PathKey);
            line = Convert.ToInt32(XmlPersistenceUtils.LoadFromAttribute(element, LineKey), CultureInfo.InvariantCulture);
            column = Convert.ToInt32(XmlPersistenceUtils.LoadFromAttribute(element, ColumnKey), CultureInfo.InvariantCulture);
        }

        public override void Save(XmlElement element, XmlTestStoreParameters parameters)
        {
            base.Save(element, parameters);

            XmlPersistenceUtils.SaveToAttribute(element, GallioTestIdKey, gallioTestId);
            XmlPersistenceUtils.SaveToAttribute(element, AssemblyNameKey, assemblyName);
            XmlPersistenceUtils.SaveToAttribute(element, NamespaceNameKey, namespaceName);
            XmlPersistenceUtils.SaveToAttribute(element, TypeNameKey, typeName);
            XmlPersistenceUtils.SaveToAttribute(element, MemberNameKey, memberName);
            XmlPersistenceUtils.SaveToAttribute(element, ParameterNameKey, parameterName);
            XmlPersistenceUtils.SaveToAttribute(element, PathKey, path);
            XmlPersistenceUtils.SaveToAttribute(element, LineKey, line.ToString(CultureInfo.InvariantCulture));
            XmlPersistenceUtils.SaveToAttribute(element, ColumnKey, column.ToString(CultureInfo.InvariantCulture));
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
        public string CodeLocation
        {
            get
            {
                if (line != 0)
                {
                    if (column != 0)
                        return String.Format("{0}({1},{2})", path, line, column);
                    return String.Format("{0}({1})", path, line);
                }

                return path ?? "(unknown)";
            }
        }

        public string Path
        {
            get { return path; }
        }

        public int Line
        {
            get { return line; }
        }

        public int Column
        {
            get { return column; }
        }

        public void SetCodeReference(string assemblyName, string namespaceName, string typeName, string memberName, string parameterName)
        {
            this.assemblyName = assemblyName;
            this.namespaceName = namespaceName;
            this.typeName = typeName;
            this.memberName = memberName;
            this.parameterName = parameterName;
        }

        public void SetCodeLocation(string path, int line, int column)
        {
            this.path = path;
            this.line = line;
            this.column = column;
        }

        private static TestId GenerateTestId(string testId)
        {
            Guid guid = new Guid(Encoding.ASCII.GetBytes(testId.PadRight(16)));
            return new TestId(guid);
        }
    }
}
