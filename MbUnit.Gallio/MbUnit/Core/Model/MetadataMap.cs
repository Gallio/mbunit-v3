using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Collections;
using MbUnit.Core.Serialization;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// A metadata map is a multi-valued dictionary of metadata keys and values associated 
    /// with a model element.  Metadata is used to communicate declarative 
    /// properties of the model in an extensible manner.
    /// </summary>
    public class MetadataMap
    {
        private MultiMap<string, object> entries;

        /// <summary>
        /// Creates an empty metadata map.
        /// </summary>
        public MetadataMap()
        {
            entries = new MultiMap<string, object>();
        }

        /// <summary>
        /// Gets the multi-valued dictionary of metadata entries.
        /// </summary>
        public MultiMap<string, object> Entries
        {
            get { return entries; }
        }

        /// <summary>
        /// Gets the value associated with the metadata key.
        /// If there are multiple values, returns only the first one.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value, or null if none</returns>
        public object GetValue(string key)
        {
            IList<object> values = entries[key];
            if (values.Count == 0)
                return null;
            return values[0];
        }

        /// <summary>
        /// Sets the value associated with the metadata key.
        /// Removes all values previously associated with that key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The new value</param>
        public void SetValue(string key, object value)
        {
            entries.RemoveKey(key);
            entries.Add(key, value);
        }

        /// <summary>
        /// Gets a serializable description of the metadata map.
        /// </summary>
        /// <returns>The metadata map info</returns>
        public virtual MetadataMapInfo ToInfo()
        {
            MetadataMapInfo info = new MetadataMapInfo();
            info.Entries = ListUtils.ConvertAllToArray<KeyValuePair<string, IList<object>>, MetadataMapEntry>(
                entries, delegate(KeyValuePair<string, IList<object>> pair)
                {
                    MetadataMapEntry entry = new MetadataMapEntry();
                    entry.Key = pair.Key;
                    entry.Values = ListUtils.ConvertAllToArray<object, string>(pair.Value, delegate(object value)
                    {
                        return value.ToString();
                    });
                    return entry;
                });
            return info;
        }
    }
}
