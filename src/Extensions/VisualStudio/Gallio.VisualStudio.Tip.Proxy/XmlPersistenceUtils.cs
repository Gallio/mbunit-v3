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

using System;
using System.Xml;

namespace Gallio.VisualStudio.Tip
{
    internal static class XmlPersistenceUtils
    {
        private const string GallioNamespace = "http://www.gallio.org/";

        public static void SaveToAttribute(XmlElement parent, string name, string value)
        {
            parent.SetAttribute(name, GallioNamespace, value);
        }

        public static void SaveToElement(XmlElement parent, string name, string value)
        {
            XmlElement element = parent.OwnerDocument.CreateElement(name, GallioNamespace);
            element.InnerText = value;
        }

        public static string LoadFromAttribute(XmlElement parent, string name)
        {
            return parent.GetAttribute(name, GallioNamespace);
        }

        public static string LoadFromElement(XmlElement parent, string name)
        {
            XmlNodeList list = parent.GetElementsByTagName(name, GallioNamespace);
            if (list.Count == 0)
                return null;

            XmlElement element = (XmlElement) list.Item(0);
            return element.InnerText;
        }
    }
}
