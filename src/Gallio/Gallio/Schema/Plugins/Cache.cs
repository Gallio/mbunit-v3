using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gallio.Schema.Plugins
{
    /// <summary>
    /// Describes a plugin cache file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type supports the framework and is not intended to be used by client code.
    /// </para>
    /// </remarks>
    [Serializable]
    [XmlRoot("cache", Namespace = SchemaConstants.XmlNamespace)]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class Cache : IValidatable
    {
        private readonly List<CachePluginInfo> pluginInfos;

        /// <summary>
        /// Creates an empty cache.
        /// </summary>
        public Cache()
        {
            pluginInfos = new List<CachePluginInfo>();
        }

        /// <summary>
        /// Gets the mutable list of plugin information records in the cache.
        /// </summary>
        [XmlArray("pluginInfos", IsNullable = false)]
        [XmlArrayItem("pluginInfo", typeof(CachePluginInfo), IsNullable = false)]
        public List<CachePluginInfo> PluginInfos
        {
            get { return pluginInfos; }
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateElementsAreNotNull("pluginInfos", pluginInfos);
            ValidationUtils.ValidateAll(pluginInfos);
        }
    }
}
