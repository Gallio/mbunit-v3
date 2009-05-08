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
using System.Text;
using Gallio.Runtime.FileTypes;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.FileTypes
{
    [TestsOn(typeof(LazyFileInspector))]
    public class LazyFileInspectorTest
    {
        [Test]
        public void Constructor_WhenFileInfoIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LazyFileInspector(null));
        }

        [Test]
        public void TryGetFileInfo_Always_ReturnsInitializedFileInfoAndTrue()
        {
            var fileInfo = new FileInfo(@"C:\foo.txt");
            using (var inspector = new LazyFileInspector(fileInfo))
            {
                FileInfo returnedFileInfo;
                Assert.IsTrue(inspector.TryGetFileInfo(out returnedFileInfo));
                Assert.AreSame(fileInfo, returnedFileInfo);
            }
        }

        [Test]
        public void TryGetContents_WhenFileExists_ReturnsContentsAndTrue()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, "Contents");

            var fileInfo = new FileInfo(path);
            using (var inspector = new LazyFileInspector(fileInfo))
            {
                string returnedContents;
                Assert.IsTrue(inspector.TryGetContents(out returnedContents));
                Assert.AreEqual("Contents", returnedContents);
            }
        }

        [Test]
        public void TryGetContents_WhenFileDoesNotExist_ReturnsNullAndFalse()
        {
            var fileInfo = new FileInfo(@"C:\This\File\Does\Not\Exist.xxx");
            using (var inspector = new LazyFileInspector(fileInfo))
            {
                string returnedContents;
                Assert.IsFalse(inspector.TryGetContents(out returnedContents));
                Assert.IsNull(returnedContents);
            }
        }

        [Test]
        public void TryGetStream_WhenFileExists_ReturnsStreamAndTrue()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, "Contents");

            var fileInfo = new FileInfo(path);
            using (var inspector = new LazyFileInspector(fileInfo))
            {
                Stream returnedStream;
                Assert.IsTrue(inspector.TryGetStream(out returnedStream));
                string contents = new StreamReader(returnedStream).ReadToEnd();
                Assert.AreEqual("Contents", contents);
            }
        }

        [Test]
        public void TryGetStream_WhenFileDoesNotExist_ReturnsNullAndFalse()
        {
            var fileInfo = new FileInfo(@"C:\This\File\Does\Not\Exist.xxx");
            using (var inspector = new LazyFileInspector(fileInfo))
            {
                Stream returnedStream;
                Assert.IsFalse(inspector.TryGetStream(out returnedStream));
                Assert.IsNull(returnedStream);
            }
        }

        [Test]
        public void TryGetStream_WhenPreviouslyReturnedStreamStillOpen_ReturnsSameStreamAtPositionZero()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, "Contents");

            var fileInfo = new FileInfo(path);
            using (var inspector = new LazyFileInspector(fileInfo))
            {
                Stream returnedStream;
                inspector.TryGetStream(out returnedStream);
                returnedStream.Position = 2;

                Stream secondReturnedStream;
                Assert.IsTrue(inspector.TryGetStream(out secondReturnedStream));
                Assert.AreEqual(0, secondReturnedStream.Position, "Should have moved stream position back to 0.");
                Assert.AreSame(returnedStream, secondReturnedStream, "Should have returned same stream since it was still open.");
            }
        }

        [Test]
        public void TryGetStream_WhenPreviouslyReturnedStreamWasClosed_ReturnsNewStream()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, "Contents");

            var fileInfo = new FileInfo(path);
            using (var inspector = new LazyFileInspector(fileInfo))
            {
                Stream returnedStream;
                inspector.TryGetStream(out returnedStream);
                returnedStream.Close();

                Stream secondReturnedStream;
                Assert.IsTrue(inspector.TryGetStream(out secondReturnedStream));
                Assert.AreNotSame(returnedStream, secondReturnedStream, "Should have returned a new stream since the previous one was closed.");
            }
        }

        [Test]
        public void Dispose_WhenStreamWasOpen_ShouldCloseTheStream()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, "Contents");

            var fileInfo = new FileInfo(path);
            using (var inspector = new LazyFileInspector(fileInfo))
            {
                Stream returnedStream;
                inspector.TryGetStream(out returnedStream);

                inspector.Dispose();

                Assert.DoesNotThrow(() => File.Delete(path), "Should be able to delete the file because the stream was closed.");
            }
        }
    }
}
