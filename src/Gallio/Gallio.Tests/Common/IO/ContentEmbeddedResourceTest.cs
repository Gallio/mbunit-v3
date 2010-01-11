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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Common.IO;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Tests.Model.Filters;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Common.IO
{
    [TestsOn(typeof(ContentEmbeddedResource))]
    public class ContentEmbeddedResourceTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_embeddedResource_content_from_null_name_should_throw_exception()
        {
           new  ContentEmbeddedResource(null, GetType());
        }

        [Test]
        public void OpenStream_from_embedded_resource_content_with_qualified_name()
        {
            var content = new ContentEmbeddedResource("Gallio.Tests.Common.IO.SampleResource.txt", GetType());

            using (var stream = content.OpenStream())
            {
                var actual = new byte[stream.Length];
                stream.Read(actual, 0, (int)stream.Length);
                var byteOrderMarkUtf8 = new byte[] {0xEF, 0xBB, 0xBF};
                Assert.AreElementsEqual(byteOrderMarkUtf8.Concat(Encoding.UTF8.GetBytes("Hello World!")), actual);
            }
        }

        [Test]
        public void OpenTextReader_from_embedded_resource_content_with_qualified_name()
        {
            var content = new ContentEmbeddedResource("Gallio.Tests.Common.IO.SampleResource.txt", GetType());

            using (var reader = content.OpenTextReader())
            {
                var actual = reader.ReadToEnd();
                Assert.AreEqual("Hello World!", actual);
            }
        }

        [Test]
        public void OpenTextReader_from_embedded_resource_content_qualified_by_type()
        {
            var content = new ContentEmbeddedResource("SampleResource.txt", GetType());

            using (var reader = content.OpenTextReader())
            {
                var actual = reader.ReadToEnd();
                Assert.AreEqual("Hello World!", actual);
            }
        }

        [Test]
        public void OpenTextReader_from_embedded_resource_content_qualified_by_codeElementInfo()
        {
            var content = new ContentEmbeddedResource("SampleResource.txt", null);
            content.CodeElementInfo = Reflector.Wrap(GetType());

            using (var reader = content.OpenTextReader())
            {
                var actual = reader.ReadToEnd();
                Assert.AreEqual("Hello World!", actual);
            }
        }

        [Test]
        public void OpenTextReader_from_embedded_resource_not_found_should_throw_exception()
        {
            var content = new ContentEmbeddedResource("SomeInvalidResource", null);
            Assert.Throws<PatternUsageErrorException>(() => content.OpenTextReader());
        }
    }
}
