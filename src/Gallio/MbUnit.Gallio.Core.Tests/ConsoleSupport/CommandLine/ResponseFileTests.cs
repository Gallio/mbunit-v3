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

extern alias MbUnit2;
using MbUnit.Core.ConsoleSupport.CommandLine;
using MbUnit.Core.Utilities;
using MbUnit2::MbUnit.Framework;
using Rhino.Mocks;

namespace MbUnit.Core.Tests.ConsoleSupport.CommandLine
{
    [TestFixture]
    public class ResponseFileTests
    {
        [Test]
        public void DefaultConstructorTest()
        {
            ResponseFile responseFile = new ResponseFile("RespondFile");
            Assert.AreEqual(typeof (FileManager), responseFile.FileManager.GetType());
            Assert.AreEqual(0, responseFile.Arguments.Count);
        }

        [Test]
        public void ConstructorWithMockFileManagerTest()
        {
            MockRepository mocks = new MockRepository();
            IFileManager fileMgr = mocks.CreateMock<IFileManager>();
            ResponseFile responseFile = new ResponseFile("RespondFile", fileMgr);
            Assert.AreNotEqual(typeof(FileManager), responseFile.FileManager.GetType());
            Assert.AreEqual(0, responseFile.Arguments.Count);
        }

        [Test]
        public void IsEmptyTest()
        {
            MockRepository mocks = new MockRepository();
            IFileManager fileMgr = mocks.CreateMock<IFileManager>();
            ResponseFile responseFile = new ResponseFile("RespondFile", fileMgr);
            Assert.AreEqual(true, responseFile.IsEmpty);
        }
    }
}