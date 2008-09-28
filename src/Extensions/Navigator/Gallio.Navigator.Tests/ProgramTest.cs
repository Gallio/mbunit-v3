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

using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Navigator.Tests
{
    [TestsOn(typeof(Program))]
    public class ProgramTest
    {
        [Test]
        public void CallsHelpWhenTooFewArguments()
        {
            InstrumentedProgram program = new InstrumentedProgram();
            int returnCode = program.Run(new string[] { });

            Assert.IsTrue(program.HelpCalled);
            Assert.AreEqual(1, returnCode);
        }

        [Test]
        public void CallsHelpWhenTooManyArguments()
        {
            InstrumentedProgram program = new InstrumentedProgram();
            int returnCode = program.Run(new string[] { "a", "b" });

            Assert.IsTrue(program.HelpCalled);
            Assert.AreEqual(1, returnCode);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void NavigateTo(bool simulateSuccess)
        {
            MockRepository mocks = new MockRepository();

            IGallioNavigator navigator = mocks.CreateMock<IGallioNavigator>();
            using (mocks.Record())
            {
                Expect.Call(navigator.NavigateTo(
                        @"C:\Source\MbUnit\v3\src\Gallio\Gallio.Tests\Reflection\Impl\CecilReflectionPolicyTest.cs", 5, 11))
                    .Return(simulateSuccess);
            }

            using (mocks.Playback())
            {
                InstrumentedProgram program = new InstrumentedProgram();
                program.Navigator = navigator;
                int returnCode = program.Run(new string[]
                {
                    @"gallio:navigateTo?path=C:\Source\MbUnit\v3\src\Gallio\Gallio.Tests\Reflection\Impl\CecilReflectionPolicyTest.cs&line=5&column=11"
                });

                Assert.IsFalse(program.HelpCalled);
                Assert.AreEqual(simulateSuccess ? 0 : 1, returnCode);

                mocks.VerifyAll();
            }
        }

        private class InstrumentedProgram : Program
        {
            public bool HelpCalled { get; set; }

            protected override void ShowHelp()
            {
                HelpCalled = true;
            }
        }
    }
}
