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

using Gallio.Common.IO;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Models;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runner.Projects;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(DeleteAllReportsCommand))]
    internal class DeleteAllReportsCommandTest
    {
        private IFileSystem fileSystem;
        private DeleteAllReportsCommand deleteAllReportsCommand;
        private IProjectTreeModel projectTreeModel;

        [SetUp]
        public void EstablishContext()
        {
            projectTreeModel = MockRepository.GenerateStub<IProjectTreeModel>();
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            deleteAllReportsCommand = new DeleteAllReportsCommand(projectTreeModel, fileSystem);
        }

        [Test]
        public void Execute_should_delete_report_directory()
        {
            const string reportDirectory = "dklfjdsfh";
            projectTreeModel.TestProject = new TestProject { ReportDirectory = reportDirectory };

            deleteAllReportsCommand.Execute(MockProgressMonitor.Instance);

            fileSystem.AssertWasCalled(fs => fs.DeleteDirectory(reportDirectory, true));
        }
    }
}
