// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Validation;

namespace Gallio.Runtime.Extensibility.Schema
{
    /// <summary>
    /// Specifies an old version range for an assembly binding redirect.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class BindingRedirect : IValidatable
    {
        private string oldVersion;

        /// <summary>
        /// Creates an uninitialized assembly binding redirect for XML deserialization.
        /// </summary>
        private BindingRedirect()
        {
        }

        /// <summary>
        /// Creates an assembly binding redirect.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Accepts a version number like "1.2.3.4" or a range like "1.0.0.0-1.1.65535.65535".
        /// </para>
        /// </remarks>
        /// <param name="oldVersion">The old assembly version number or range.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="oldVersion"/> is null.</exception>
        public BindingRedirect(string oldVersion)
        {
            if (oldVersion == null)
                throw new ArgumentNullException("oldVersion");

            this.oldVersion = oldVersion;
        }

        /// <summary>
        /// Gets or sets the old assembly version number or range.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Accepts a version number like "1.2.3.4" or a range like "1.0.0.0-1.1.65535.65535".
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("oldVersion")]
        public string OldVersion
        {
            get { return oldVersion; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                oldVersion = value;
            }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateAssemblyName("oldVersion", oldVersion);
        }
    }
}
