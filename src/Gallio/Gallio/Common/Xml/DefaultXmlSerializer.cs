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

using System.Xml.Serialization;

namespace Gallio.Common.Xml
{
    ///<summary>
    /// Default IXmlSerializer implementation.
    ///</summary>
    public class DefaultXmlSerializer : IXmlSerializer
    {
        /// <summary>
        /// Saves an object graph to a pretty-printed Xml file using <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="filename">The filename.</param>
        /// <typeparam name="T">The root object type.</typeparam>
        public void SaveToXml<T>(T root, string filename)
        {
            XmlSerializationUtils.SaveToXml(root, filename);
        }

        /// <summary>
        /// Loads an object graph from an Xml file using <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The root object</returns>
        /// <typeparam name="T">The root object type.</typeparam>
        public T LoadFromXml<T>(string filename)
        {
            return XmlSerializationUtils.LoadFromXml<T>(filename);
        }
    }
}