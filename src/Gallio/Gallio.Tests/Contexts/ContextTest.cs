// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using Gallio.Contexts;
using MbUnit.Framework;

namespace Gallio.Tests.Contexts
{
    [TestFixture]
    [TestsOn(typeof(Context))]
    public class ContextTest
    {
        [Test]
        public void CurrentTestHasCorrectTestName()
        {
            Assert.AreEqual("CurrentTestHasCorrectTestName", Context.CurrentTest.Name);
        }

        [Test]
        public void CurrentStepHasCorrectTestName()
        {
            Assert.AreEqual("CurrentStepHasCorrectTestName", Context.CurrentStep.FullName);

            Step.Run("Step1", delegate
            {
                Assert.AreEqual("CurrentStepHasCorrectTestName:Step1", Context.CurrentStep.FullName);

                Step.Run("Step2", delegate
                {
                    Assert.AreEqual("CurrentStepHasCorrectTestName:Step1/Step2", Context.CurrentStep.FullName);
                });
            });
        }
    }
}