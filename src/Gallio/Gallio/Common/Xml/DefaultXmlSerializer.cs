// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using System;
using System.Xml;
using System.Xml.Serialization;

namespace Gallio.Common.Xml
{
    ///<summary>
    /// Default IXmlSerializer implementation.
    ///</summary>
    public class DefaultXmlSerializer : IXmlSerializer
    {
        /// <inheritdoc />
        public void SaveToXml<T>(T root, string filename)
        {
            if (root == null)
                throw new ArgumentNullException("root");
            if (filename == null)
                throw new ArgumentNullException("filename");

            var serializer = new XmlSerializer(typeof(T));

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;

            using (var writer = XmlWriter.Create(filename, settings))
                serializer.Serialize(writer, root);
        }

        /// <inheritdoc />
        public T LoadFromXml<T>(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            var serializer = new XmlSerializer(typeof(T));

            using (XmlReader reader = XmlReader.Create(filename))
                return (T)serializer.Deserialize(reader);
        }
    }
}