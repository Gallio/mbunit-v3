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
    [TestsOn(typeof(ContentFile))]
    public class ContentFileTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_file_content_from_null_systemFile_should_throw_exception()
        {
            new ContentFile(@"c:\Path", null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_file_content_from_null_path_should_throw_exception()
        {
            var mockFileSystem = MockRepository.GenerateStub<IFileSystem>();
            new ContentFile(null, mockFileSystem);
        }

        [Test]
        public void OpenStream_from_file_content()
        {
            var mockFileSystem = MockRepository.GenerateMock<IFileSystem>();
            mockFileSystem.Expect(x => x.OpenRead(@"c:\Path")).Return(new MemoryStream(new byte[] { 1, 2, 3 }));
            var content = new ContentFile(@"c:\Path", mockFileSystem);

            using (var stream = content.OpenStream())
            {
                var actual = new byte[stream.Length];
                stream.Read(actual, 0, (int)stream.Length);
                Assert.AreElementsEqual(new byte[] { 1, 2, 3 }, actual);
            }

            mockFileSystem.VerifyAllExpectations();
        }

        [Test]
        public void OpenTextReader_from_file_content()
        {
            var mockFileSystem = MockRepository.GenerateMock<IFileSystem>();
            mockFileSystem.Expect(x => x.OpenRead(@"c:\Path")).Return(new MemoryStream(Encoding.ASCII.GetBytes("Hello")));
            var content = new ContentFile(@"c:\Path", mockFileSystem);

            using (var reader = content.OpenTextReader())
            {
                var actual = reader.ReadToEnd();
                Assert.AreEqual("Hello", actual);
            }
        }
    }
}
