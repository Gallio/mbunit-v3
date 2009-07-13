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
using Gallio.Common;
using Gallio.Common.Validation;

namespace Gallio.Runtime.Extensibility.Schema
{
    /// <summary>
    /// Describes an assembly used by a plugin and how the loader should bind to it.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class Assembly : IValidatable
    {
        private string fullName;
        private string codeBase;
        private readonly List<BindingRedirect> bindingRedirects;

        /// <summary>
        /// Creates an uninitialized assembly reference for XML deserialization.
        /// </summary>
        private Assembly()
        {
            bindingRedirects = new List<BindingRedirect>();
            ApplyPublisherPolicy = true;
        }

        /// <summary>
        /// Creates an assembly reference.
        /// </summary>
        /// <param name="fullName">The assembly full name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fullName"/> is null.</exception>
        public Assembly(string fullName)
            : this()
        {
            if (fullName == null)
                throw new ArgumentNullException("fullName");

            this.fullName = fullName;
        }

        /// <summary>
        /// Gets or sets the assembly full name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("fullName")]
        public string FullName
        {
            get { return fullName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                fullName = value;
            }
        }

        /// <summary>
        /// Gets or sets the assembly code base as a relative path to the plugin
        /// base directory or to the 'bin' directory within, or null if none.
        /// </summary>
        [XmlAttribute("codeBase")]
        public string CodeBase
        {
            get { return codeBase; }
            set { codeBase = value; }
        }

        /// <summary>
        /// Gets or sets whether the assembly full name should be used to qualify all partial
        /// name references to the assembly.
        /// </summary>
        [XmlAttribute("qualifyPartialName")]
        public bool QualifyPartialName { get; set; }

        /// <summary>
        /// Gets or sets whether to apply the assembly publisher policy.  Default is <c>true</c>.
        /// </summary>
        [XmlAttribute("applyPublisherPolicy")]
        public bool ApplyPublisherPolicy { get; set; }

        /// <summary>
        /// Gets the mutable list of assembly binding redirects.
        /// </summary>
        [XmlArray("bindingRedirects", IsNullable = false)]
        [XmlArrayItem("bindingRedirect", typeof(BindingRedirect), IsNullable = false)]
        public List<BindingRedirect> BindingRedirects
        {
            get { return bindingRedirects; }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateAssemblyName("fullName", fullName);
            ValidationUtils.ValidateElementsAreNotNull("bindingRedirects", bindingRedirects);
            ValidationUtils.ValidateAll(bindingRedirects);
        }
    }
}
