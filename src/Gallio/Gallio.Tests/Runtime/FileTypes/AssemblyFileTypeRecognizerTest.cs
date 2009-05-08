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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Runtime.FileTypes;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.FileTypes
{
    [TestsOn(typeof(AssemblyFileTypeRecognizer))]
    public class AssemblyFileTypeRecognizerTest
    {
        [Test]
        public void IsRecognizedFile_WhenStreamIsACLRAssembly_ReturnsTrue()
        {
            var recognizer = new AssemblyFileTypeRecognizer();
            var path = Assembly.GetExecutingAssembly().Location;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var inspector = MockRepository.GenerateStub<IFileInspector>();
                Stream dummy;
                inspector.Expect(x => x.TryGetStream(out dummy)).OutRef(stream).Return(true);

                Assert.IsTrue(recognizer.IsRecognizedFile(inspector));
            }
        }

        [Test]
        public void IsRecognizedFile_WhenStreamIsAPEFileButNotAnAssembly_ReturnsFalse()
        {
            var recognizer = new AssemblyFileTypeRecognizer();
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"kernel32.dll");
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var inspector = MockRepository.GenerateStub<IFileInspector>();
                Stream dummy;
                inspector.Expect(x => x.TryGetStream(out dummy)).OutRef(stream).Return(true);

                Assert.IsFalse(recognizer.IsRecognizedFile(inspector));
            }
        }

        [Test]
        public void IsRecognizedFile_WhenStreamIsNotAvailable_ReturnsFalse()
        {
            var recognizer = new AssemblyFileTypeRecognizer();
            var inspector = MockRepository.GenerateStub<IFileInspector>();
            Stream dummy;
            inspector.Expect(x => x.TryGetStream(out dummy)).OutRef(new object[] { null }).Return(false);

            Assert.IsFalse(recognizer.IsRecognizedFile(inspector));
        }
    }
}
