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

using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(RemoveFileCommand))]
    public class RemoveFileCommandTest
    {
        [Test]
        public void Execute_should_do_nothing_if_no_filename_is_provided()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var removeFileCommand = new RemoveFileCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.Instance;

            removeFileCommand.Execute(progressMonitor);

            projectController.AssertWasNotCalled(pc => pc.RemoveFile(Arg<string>.Is.Anything));
        }

        [Test]
        public void Execute_should_remove_the_file()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var removeFileCommand = new RemoveFileCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.Instance;
            const string filename = "filename";
            removeFileCommand.FileName = filename;

            removeFileCommand.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.RemoveFile(filename));
        }
    }
}
