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
    [TestsOn(typeof(ContentInline))]
    public class ContentInlineTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_inline_content_from_null_contents_should_throw_exception()
        {
            new ContentInline(null);
        }

        [Test]
        public void OpenStream_from_inline_content()
        {
            var content = new  ContentInline("Hello");

            using (var stream = content.OpenStream())
            {
                var actual = new byte[stream.Length];
                stream.Read(actual, 0, (int)stream.Length);
                Assert.AreElementsEqual(Encoding.UTF8.GetBytes("Hello"), actual);
            }
        }

        [Test]
        public void OpenTextReader_from_inline_content()
        {
            var content = new ContentInline("Hello");

            using (var reader = content.OpenTextReader())
            {
                var actual = reader.ReadToEnd();
                Assert.AreEqual("Hello", actual);
            }
        }
    }
}
