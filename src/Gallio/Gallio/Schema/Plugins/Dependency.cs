using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gallio.Schema.Plugins
{
    /// <summary>
    /// Describes a dependency on another plugin.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class Dependency : IValidatable
    {
        private string pluginId;

        /// <summary>
        /// Creates an uninitialized dependency for XML deserialization.
        /// </summary>
        private Dependency()
        {
        }

        /// <summary>
        /// Creates an plugin dependency reference.
        /// </summary>
        /// <param name="pluginId">The referenced plugin id</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginId"/> is null</exception>
        public Dependency(string pluginId)
        {
            if (pluginId == null)
                throw new ArgumentNullException("pluginId");

            this.pluginId = pluginId;
        }

        /// <summary>
        /// Gets or sets the referenced plugin id.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("pluginId")]
        public string PluginId
        {
            get { return pluginId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                pluginId = value;
            }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("pluginId", pluginId);
        }
    }
}
