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
using Gallio.Common.IO;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(DeleteReportCommand))]
    internal class DeleteReportCommandTest
    {
        private IFileSystem fileSystem;
        private DeleteReportCommand deleteReportCommand;

        [SetUp]
        public void EstablishContext()
        {
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            deleteReportCommand = new DeleteReportCommand(fileSystem);
        }

        [Test]
        public void FileName_should_return_correct_value()
        {
            const string fileName = "fileName";

            deleteReportCommand.FileName = fileName;

            Assert.AreEqual(fileName, deleteReportCommand.FileName);
        }

        [Test]
        public void Exception_should_be_thrown_if_filename_is_not_set()
        {
            Assert.Throws<Exception>(() => deleteReportCommand.Execute(MockProgressMonitor.Instance));
        }

        [Test]
        public void Execute_should_delete_file()
        {
            const string fileName = "fileName";
            deleteReportCommand.FileName = fileName;
            
            deleteReportCommand.Execute(MockProgressMonitor.Instance);

            fileSystem.AssertWasCalled(fs => fs.DeleteFile(fileName));
        }
    }
}
