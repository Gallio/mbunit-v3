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

using Gallio.AutoCAD.Isolation;
using Gallio.AutoCAD.ProcessManagement;
using Gallio.Model.Isolation;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests.Isolation
{
    [TestsOn(typeof(AcadTestIsolationProvider))]
    public class AcadTestIsolationProviderTest
    {
        private readonly ILogger logger = MockRepository.GenerateStub<ILogger>();

        [Test]
        public void CreateContext_ReturnsAnAcadTestIsolationContext()
        {
            var options = new TestIsolationOptions();
            var process = MockRepository.GenerateStub<IAcadProcess>();
            var processFactory = MockRepository.GenerateStub<IAcadProcessFactory>();
            processFactory.Stub(x => x.CreateProcess(Arg.Is(options))).Return(process);

            var provider = new AcadTestIsolationProvider(processFactory);

            var context = provider.CreateContext(options, logger);

            Assert.IsInstanceOfType<AcadTestIsolationContext>(context);
        }
    }
}