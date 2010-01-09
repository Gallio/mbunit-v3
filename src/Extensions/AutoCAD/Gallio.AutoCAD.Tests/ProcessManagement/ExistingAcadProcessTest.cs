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

using Gallio.AutoCAD.Commands;
using Gallio.AutoCAD.ProcessManagement;
using Gallio.Common.Concurrency;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests.ProcessManagement
{
    [TestsOn(typeof(ExistingAcadProcess))]
    public class ExistingAcadProcessTest
    {
        [Test]
        public void Dispose_CallsDisposeOnActualProcess()
        {
            var logger = MockRepository.GenerateStub<ILogger>();
            var commandRunner = MockRepository.GenerateStub<IAcadCommandRunner>();
            var actualProcess = MockRepository.GenerateMock<IProcess>();
            var acadProcess = new ExistingAcadProcess(logger, commandRunner, actualProcess);

            acadProcess.Dispose();

            actualProcess.AssertWasCalled(x => x.Dispose());
        }
    }
}
