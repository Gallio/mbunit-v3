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

using Gallio.Common.IO;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(OpenReportCommand))]
    internal class OpenReportCommandTest
    {
        [Test]
        public void Execute_should_call_open_file_on_file_system_if_filename_is_supplied()
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var command = new OpenReportCommand(fileSystem);
            const string fileName = "fileName";
            command.FileName = fileName;
            var progressMonitor = MockProgressMonitor.Instance;

            command.Execute(progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.OpenFile(fileName));
        }

        [Test]
        public void Execute_should_not_call_open_file_on_file_system_if_filename_is_not_supplied()
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var command = new OpenReportCommand(fileSystem);
            var progressMonitor = MockProgressMonitor.Instance;

            command.Execute(progressMonitor);

            fileSystem.AssertWasNotCalled(fs => fs.OpenFile(Arg<string>.Is.Anything));
        }
    }
}
