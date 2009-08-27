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
using Gallio.AutoCAD.Commands;
using MbUnit.Framework;

namespace Gallio.AutoCAD.Tests.Commands
{
    [TestsOn(typeof(CreateEndpointAndWaitCommand))]
    public class CreateEndpointAndWaitCommandTest
    {
        private CreateEndpointAndWaitCommand command;

        [Row(null)]
        [Row(@"c:\path\to\Gallio.Loader.dll")]
        public string GallioLoaderAssemblyPath
        { get; set; }

        [SetUp]
        public void SetUp()
        {
            command = new CreateEndpointAndWaitCommand("MyIpcPort", Guid.NewGuid(), GallioLoaderAssemblyPath);
        }

        [Test]
        public void GetArguments_ReturnsArgumentsInExpectedOrder()
        {
            Assert.Over.Pairs(command.GetArguments(), ExpectedOrder, Assert.AreEqual);
        }

        private IEnumerable<string> ExpectedOrder
        {
            get
            {
                yield return command.IpcPortName;
                yield return command.LinkId.ToString();
                yield return command.GallioLoaderAssemblyPath;
            }
        }

        [Test]
        [Row(null)]
        [Row("")]
        public static void Constructor_WhenIpcPortNameIsNullOrEmpty_ThrowsArgumentException(string ipcPortName)
        {
            Assert.Throws<ArgumentException>(() => new CreateEndpointAndWaitCommand(ipcPortName, Guid.NewGuid(), null));
        }
    }
}
