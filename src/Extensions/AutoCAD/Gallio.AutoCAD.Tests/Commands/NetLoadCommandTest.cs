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

using System.IO;
using System.Linq;
using Gallio.AutoCAD.Commands;
using Gallio.Common.IO;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests.Commands
{
    [TestsOn(typeof(NetLoadCommand))]
    public class NetLoadCommandTest
    {
        private IFileSystem fileSystem;
        private NetLoadCommand command;

        [SetUp]
        public void SetUp()
        {
            var logger = MockRepository.GenerateStub<ILogger>();
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var pluginLocator = new AcadPluginLocator(logger, fileSystem);

            command = new NetLoadCommand(logger, pluginLocator);
        }

        [Test]
        [Row("19.0s (LMS Tech)", "Gallio.AutoCAD.Plugin190.dll")]
        public void GetArguments_ReturnsArgumentsInExpectedOrder(string version, string plugin)
        {
            StubAvailablePlugins(plugin);
            Assert.AreEqual(plugin, command.GetArguments(new {Version = version}).Single());
        }

        private void StubAvailablePlugins(params string[] plugins)
        {
            fileSystem
                .Stub(fs => fs.GetFilesInDirectory(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<SearchOption>.Is.Anything))
                .Return(plugins);
        }
    }
}
