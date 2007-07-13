// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Xml.Serialization;
using MbUnit.Framework.Model;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Utilities;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a metadata map in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="MetadataMap"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class MetadataMapInfo
    {
        private MetadataMapEntry[] entries;

        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public MetadataMapInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public MetadataMapInfo(MetadataMap obj)
        {
            entries = ListUtils.ConvertAllToArray<KeyValuePair<string, IList<object>>, MetadataMapEntry>(
                obj.Entries, delegate(KeyValuePair<string, IList<object>> pair)
                {
                    MetadataMapEntry entry = new MetadataMapEntry();
                    entry.Key = pair.Key;
                    entry.Values = ListUtils.ConvertAllToArray<object, string>(pair.Value, delegate(object value)
                    {
                        return value.ToString();
                    });
                    return entry;
                });
        }

        /// <summary>
        /// Gets or sets the array of metadata map entries.  (non-null but possibly empty)
        /// </summary>
        [XmlArray("entries", IsNullable=false)]
        [XmlArrayItem("entry", IsNullable=false)]
        public MetadataMapEntry[] Entries
        {
            get { return entries; }
            set { entries = value; }
        }
    }
}