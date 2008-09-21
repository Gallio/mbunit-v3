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

using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Framework
{
    [TestFixture]
    public class TestStepTest
    {
        [Test]
        public void CurrentStepHasCorrectTestName()
        {
            Assert.Like(TestStep.CurrentStep.FullName, "Gallio.Tests/TestStepTest/CurrentStepHasCorrectTestName$");

            TestStep.RunStep("Step1", delegate
            {
                Assert.Like(TestStep.CurrentStep.FullName, "Gallio.Tests/TestStepTest/CurrentStepHasCorrectTestName/Step1$");

                TestStep.RunStep("Step2", delegate
                {
                    Assert.Like(TestStep.CurrentStep.FullName, "Gallio.Tests/TestStepTest/CurrentStepHasCorrectTestName/Step1/Step2$");
                });
            });
        }

        [Test]
        public void MetadataAdditionsAreVisibleInStepInfo()
        {
            Assert.IsNull(TestStep.CurrentStep.Metadata.GetValue("New"));

            TestStep.AddMetadata("New", "And improved!");
            Assert.AreEqual("And improved!", TestStep.CurrentStep.Metadata.GetValue("New"));

            TestStep.AddMetadata("New", "Now with less sugar.");
            Assert.AreElementsEqual(new string[] { "And improved!", "Now with less sugar." },
                TestStep.CurrentStep.Metadata["New"]);
        }
    }
}