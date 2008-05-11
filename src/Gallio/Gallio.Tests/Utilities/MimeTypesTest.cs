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
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Utilities
{
    [TestFixture]
    [TestsOn(typeof(MimeTypes))]
    public class MimeTypesTest
    {
        [Test]
        [Row(".txt", MimeTypes.PlainText)]
        [Row(".xml", MimeTypes.Xml)]
        [Row(".html", MimeTypes.Html)]
        [Row(".htm", MimeTypes.Html)]
        [Row(".xhtml", MimeTypes.XHtml)]
        [Row(".mhtml", MimeTypes.MHtml)]
        [Row(".mht", MimeTypes.MHtml)]
        [Row(".css", MimeTypes.Css)]
        [Row(".js", MimeTypes.JavaScript)]
        [Row(".gif", MimeTypes.Gif)]
        [Row(".png", MimeTypes.Png)]
        [Row(".unknown", null)]
        [Row("", null)]
        public void SupportsMappingExtensionToMimeType(string extension, string expectedMimeType)
        {
            Assert.AreEqual(expectedMimeType, MimeTypes.GetMimeTypeByExtension(extension));
        }
    }
}
