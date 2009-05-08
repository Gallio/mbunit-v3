// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Gallio.Common.Collections;

namespace Gallio.Schema.Plugins
{
    /// <summary>
    /// Represents a component descriptor in Xml.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class Component : IValidatable
    {
        private string componentId;
        private string serviceId;
        private string componentType;

        /// <summary>
        /// Creates an uninitialized component descriptor for XML deserialization.
        /// </summary>
        private Component()
        {
        }

        /// <summary>
        /// Creates a component descriptor.
        /// </summary>
        /// <param name="componentId">The component id</param>
        /// <param name="serviceId">The service id</param>
        /// <param name="componentType">The assembly-qualified component type name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="componentId"/>
        /// or <paramref name="serviceId"/> is null</exception>
        public Component(string componentId, string serviceId, string componentType)
        {
            if (componentId == null)
                throw new ArgumentNullException("componentId");
            if (serviceId == null)
                throw new ArgumentNullException("serviceId");
            if (componentType == null)
                throw new ArgumentNullException("componentType");

            this.componentId = componentId;
            this.serviceId = serviceId;
            this.componentType = componentType;
        }

        /// <summary>
        /// Gets or sets the component id.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("componentId")]
        public string ComponentId
        {
            get { return componentId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                componentId = value;
            }
        }

        /// <summary>
        /// Gets or sets the service id.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("serviceId")]
        public string ServiceId
        {
            get { return serviceId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                serviceId = value;
            }
        }

        /// <summary>
        /// Gets or sets the assembly-qualified component type name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("componentType")]
        public string ComponentType
        {
            get { return componentType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                componentType = value;
            }
        }

        /// <summary>
        /// Gets or sets the component parameters, or null if there are none.
        /// </summary>
        [XmlElement("parameters", IsNullable = false)]
        public KeyValueTable Parameters { get; set; }

        /// <summary>
        /// Gets or sets the component traits, or null if there are none.
        /// </summary>
        [XmlElement("traits", IsNullable = false)]
        public KeyValueTable Traits { get; set; }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("componentId", componentId);
            ValidationUtils.ValidateNotNull("serviceId", serviceId);
            ValidationUtils.ValidateNotNull("componentType", componentType);
        }
    }
}
