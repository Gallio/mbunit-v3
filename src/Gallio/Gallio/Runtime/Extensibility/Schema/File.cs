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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Validation;

namespace Gallio.Runtime.Extensibility.Schema
{
    /// <summary>
    /// Describes a file that belongs to a plugin.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class File : IValidatable
    {
        private string path;

        /// <summary>
        /// Creates an uninitialized file description for XML deserialization.
        /// </summary>
        private File()
        {
        }

        /// <summary>
        /// Creates a file description.
        /// </summary>
        /// <param name="path">The path of the file relative to the plugin base directory.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null.</exception>
        public File(string path)
            : this()
        {
            if (path == null)
                throw new ArgumentNullException("path");

            this.path = path;
        }

        /// <summary>
        /// Gets or sets the path of the file relative to the plugin base directory.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("path")]
        public string Path
        {
            get { return path; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                path = value;
            }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("path", path);
        }
    }
}
