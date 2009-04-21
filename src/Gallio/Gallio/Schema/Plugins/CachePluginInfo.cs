using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gallio.Schema.Plugins
{
    /// <summary>
    /// Describes information about a plugin whose metadata has been cached.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type supports the framework and is not intended to be used by client code.
    /// </para>
    /// </remarks>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class CachePluginInfo : IValidatable
    {
        /// <summary>
        /// Gets or sets the plugin file path.
        /// </summary>
        [XmlAttribute("pluginFile")]
        public string PluginFile { get; set; }

        /// <summary>
        /// Gets or sets the plugin file modification time stamp.
        /// </summary>
        [XmlAttribute("pluginFileModificationTime")]
        public DateTime PluginFileModificationTime { get; set; }

        /// <summary>
        /// Gets or sets the plugin metadata.
        /// </summary>
        [XmlElement("plugin", IsNullable = false)]
        public Plugin Plugin { get; set; }

        /// <summary>
        /// Gets or sets the plugin base directory path.
        /// </summary>
        [XmlAttribute("baseDirectory")]
        public string BaseDirectory { get; set; }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("pluginFile", PluginFile);
            ValidationUtils.ValidateNotNull("plugin", Plugin);
            Plugin.Validate();
            ValidationUtils.ValidateNotNull("baseDirectory", BaseDirectory);
        }
    }
}
