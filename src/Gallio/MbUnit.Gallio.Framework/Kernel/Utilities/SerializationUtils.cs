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
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MbUnit.Framework.Kernel.Utilities
{
    /// <summary>
    /// Utilities and constants used for serialization.
    /// </summary>
    public static class SerializationUtils
    {
        /// <summary>
        /// The XML namespace for all MbUnit Gallio XML types.
        /// </summary>
        public const string XmlNamespace = "http://www.mbunit.com/gallio";

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