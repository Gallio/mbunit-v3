// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests
{
    [TestsOn(typeof(AcadTestDriverFactory))]
    public class AcadTestDriverFactoryTest : BaseTestWithMocks
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsExceptionWhenAcadProcessFactoryArgumentIsNull()
        {
            new AcadTestDriverFactory(null);
        }

        [Test]
        public void CanCreateAcadTestDriver()
        {
            IAcadProcessFactory processFactory = Mocks.StrictMock<IAcadProcessFactory>();
            IAcadProcess process = Mocks.StrictMock<IAcadProcess>();
            using (Mocks.Record())
            {
                Expect.Call(processFactory.CreateProcess()).Return(process);
                Expect.Call(process.GetRemoteTestDriver()).Return(Mocks.Stub<IRemoteTestDriver>());
            }

            using (Mocks.Playback())
            {
                AcadTestDriverFactory driverFactory = new AcadTestDriverFactory(processFactory);
                Assert.IsNotNull(driverFactory.CreateTestDriver());
            }
        }
    }
}
