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
using Gallio.Model;
using Gallio.Model.Reflection;

namespace Gallio.Model.Serialization
{
    /// <summary>
    /// Describes a test model component in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITestComponent"/>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public abstract class TestComponentData
    {
        private string id;
        private string name;
        private CodeReference codeReference;
        private MetadataMap metadata;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        protected TestComponentData()
        {
        }

        /// <summary>
        /// Creates a test component.
        /// </summary>
        /// <param name="id">The component id</param>
        /// <param name="name">The component name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/> is null</exception>
        public TestComponentData(string id, string name)
        {
            if (id == null)
                throw new ArgumentNullException(@"id");
            if (name == null)
                throw new ArgumentNullException(@"name");

            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Copies the contents of a test component.
        /// </summary>
        /// <param name="source">The source object</param>
        public TestComponentData(ITestComponent source)
        {
            if (source == null)
                throw new ArgumentNullException(@"source");

            id = source.Id;
            name = source.Name;
            codeReference = source.CodeElement != null ? source.CodeElement.CodeReference : CodeReference.Unknown;
            metadata = source.Metadata.Copy();
        }

        /// <summary>
        /// Gets or sets the test component id.  (non-null)
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        /// <seealso cref="ITestComponent.Id"/>
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                id = value;
            }
        }

        /// <summary>
        /// Gets or sets the test component name.  (non-null)
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        /// <seealso cref="ITestComponent.Name"/>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the code reference.  (non-null)
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        /// <seealso cref="ITestComponent.CodeElement"/>
        [XmlElement("codeReference", IsNullable=false)]
        public CodeReference CodeReference
        {
            get
            {
                if (codeReference == null)
                    codeReference = new CodeReference();
                return codeReference;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                codeReference = value;
            }
        }

        /// <summary>
        /// Gets or sets the metadata map.  (non-null)
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        /// <seealso cref="ITestComponent.Metadata"/>
        [XmlElement("metadata", IsNullable=false)]
        public MetadataMap Metadata
        {
            get
            {
                if (metadata == null)
                    metadata = new MetadataMap();
                return metadata;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                metadata = value;
            }
        }
    }
}