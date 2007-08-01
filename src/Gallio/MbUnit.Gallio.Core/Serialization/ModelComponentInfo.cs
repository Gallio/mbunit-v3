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
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a model component in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="IModelComponent"/>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class ModelComponentInfo
    {
        private string id;
        private string name;
        private CodeReferenceInfo codeReference;
        private MetadataMapInfo metadata;

        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public ModelComponentInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public ModelComponentInfo(IModelComponent obj)
        {
            id = obj.Id;
            name = obj.Name;
            codeReference = new CodeReferenceInfo(obj.CodeReference);
            metadata = new MetadataMapInfo(obj.Metadata);
        }

        /// <summary>
        /// Gets or sets the test component id.  (non-null)
        /// </summary>
        /// <seealso cref="IModelComponent.Id"/>
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the test component name.  (non-null)
        /// </summary>
        /// <seealso cref="IModelComponent.Name"/>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the code reference.  (non-null)
        /// </summary>
        /// <seealso cref="IModelComponent.CodeReference"/>
        [XmlElement("codeReference", IsNullable=false)]
        public CodeReferenceInfo CodeReference
        {
            get { return codeReference; }
            set { codeReference = value; }
        }

        /// <summary>
        /// Gets or sets the metadata map.  (non-null)
        /// </summary>
        /// <seealso cref="IModelComponent.Metadata"/>
        [XmlElement("metadata", IsNullable=false)]
        public MetadataMapInfo Metadata
        {
            get { return metadata; }
            set { metadata = value; }
        }
    }
}