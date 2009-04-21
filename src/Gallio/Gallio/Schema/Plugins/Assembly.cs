using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gallio.Schema.Plugins
{
    /// <summary>
    /// Describes a reference to an assembly used by a plugin.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class Assembly : IValidatable
    {
        private string fullName;
        private string codeBase;

        /// <summary>
        /// Creates an uninitialized assembly reference for XML deserialization.
        /// </summary>
        private Assembly()
        {
        }

        /// <summary>
        /// Creates an assembly reference.
        /// </summary>
        /// <param name="fullName">The assembly full name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fullName"/> is null</exception>
        public Assembly(string fullName)
        {
            if (fullName == null)
                throw new ArgumentNullException("fullName");

            this.fullName = fullName;
        }

        /// <summary>
        /// Gets or sets the assembly full name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
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

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateAssemblyName("fullName", fullName);
        }
    }
}
