using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Gallio.Common.Validation;

namespace Gallio.Schema.Preferences
{
    /// <summary>
    /// Represents a preference setting in Xml.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public class PreferenceSetting : IValidatable
    {
        private string name;
        private string value;

        /// <summary>
        /// Creates an uninitialized preference setting for XML deserialization.
        /// </summary>
        private PreferenceSetting()
        {
        }

        /// <summary>
        /// Creates a preference setting.
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <param name="value">The setting value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="value"/> is null</exception>
        public PreferenceSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the preference setting name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("name")]
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

        /// <summary>
        /// Gets or sets the preference setting value.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("value")]
        public string Value
        {
            get { return value; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.value = value;
            }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("name", Name);
            ValidationUtils.ValidateNotNull("value", Value);
        }
    }
}
