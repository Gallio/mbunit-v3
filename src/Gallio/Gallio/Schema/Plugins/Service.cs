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

namespace Gallio.Schema.Plugins
{
    /// <summary>
    /// Represents a service descriptor in Xml.
    /// </summary>
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public class Service : IValidatable
    {
        private string serviceId;
        private string serviceType;

        /// <summary>
        /// Creates an uninitialized service descriptor for XML deserialization.
        /// </summary>
        private Service()
        {
        }

        /// <summary>
        /// Creates a service descriptor.
        /// </summary>
        /// <param name="serviceId">The service id</param>
        /// <param name="serviceType">The service type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceId"/> or
        /// <paramref name="serviceType"/> is null</exception>
        public Service(string serviceId, string serviceType)
            : this()
        {
            if (serviceId == null)
                throw new ArgumentNullException("serviceId");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            this.serviceId = serviceId;
            this.serviceType = serviceType;
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
        /// Gets or sets the assembly-qualified service type name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("serviceType")]
        public string ServiceType
        {
            get { return serviceType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                serviceType = value;
            }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("serviceId", serviceId);
            ValidationUtils.ValidateNotNull("serviceType", serviceType);
        }
    }
}
