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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITemplateComponent" />.
    /// </summary>
    public abstract class BaseModelComponent : IModelComponent
    {
        private string id;
        private string name;
        private CodeReference codeReference;
        private MetadataMap metadata;

        /// <summary>
        /// Initializes a test component.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition of the component</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="codeReference"/>
        /// is null</exception>
        public BaseModelComponent(/*string id, */string name, CodeReference codeReference)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (codeReference == null)
                throw new ArgumentNullException("codeReference");

            this.id = Guid.NewGuid().ToString(); // interim value until initialized to a stable identifier
            this.name = name;
            this.codeReference = codeReference;
            this.metadata = new MetadataMap();
        }

        /// <inheritdoc />
        public string Id
        {
            get { return id; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                id = value;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                name = value;
            }
        }

        /// <inheritdoc />
        public MetadataMap Metadata
        {
            get { return metadata; }
        }

        /// <inheritdoc />
        public CodeReference CodeReference
        {
            get { return codeReference; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                codeReference = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="MetadataConstants.ComponentKindKey" />
        /// metadata entry.  (This is a convenience method.)
        /// </summary>
        /// <value>
        /// One of the <see cref="ComponentKind" /> constants.
        /// </value>
        public string Kind
        {
            get { return (string)Metadata.GetValue(MetadataConstants.ComponentKindKey); }
            set { Metadata.SetValue(MetadataConstants.ComponentKindKey, value); }
        }
    }
}
