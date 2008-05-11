// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System.Xml;
using System.Xml.Serialization;

namespace Gallio.Utilities
{
    /// <summary>
    /// Utilities and constants used for serialization.
    /// </summary>
    public static class XmlSerializationUtils
    {
        /// <summary>
        /// The XML namespace for all Gallio XML types.
        /// </summary>
        public const string GallioNamespace = "http://www.gallio.org/";

        /// <summary>
        /// Saves an object graph to a pretty-printed Xml file using <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="root">The root object</param>
        /// <param name="filename">The filename</param>
        /// <typeparam name="T">The root object type</typeparam>
        public static void SaveToXml<T>(T root, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
                serializer.Serialize(writer, root);
        }

        /// <summary>
        /// Loads an object graph from an Xml file using <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <returns>The root object</returns>
        /// <typeparam name="T">The root object type</typeparam>
        public static T LoadFromXml<T>(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (XmlReader reader = XmlReader.Create(filename))
                return (T) serializer.Deserialize(reader);
        }
    }
}