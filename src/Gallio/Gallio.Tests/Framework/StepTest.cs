// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Framework
{
    [TestFixture]
    public class StepTest
    {
        [Test]
        public void CurrentStepHasCorrectTestName()
        {
            StringAssert.Like(Step.CurrentStep.FullName, "Gallio.Tests/StepTest/CurrentStepHasCorrectTestName$");

            Step.RunStep("Step1", delegate
            {
                StringAssert.Like(Step.CurrentStep.FullName, "Gallio.Tests/StepTest/CurrentStepHasCorrectTestName:Step1$");

                Step.RunStep("Step2", delegate
                {
                    StringAssert.Like(Step.CurrentStep.FullName, "Gallio.Tests/StepTest/CurrentStepHasCorrectTestName:Step1/Step2$");
                });
            });
        }

        [Test]
        public void MetadataAdditionsAreVisibleInStepInfo()
        {
            Assert.IsNull(Step.CurrentStep.Metadata.GetValue("New"));

            Step.AddMetadata("New", "And improved!");
            Assert.AreEqual("And improved!", Step.CurrentStep.Metadata.GetValue("New"));

            Step.AddMetadata("New", "Now with less sugar.");
            CollectionAssert.AreElementsEqual(new string[] { "And improved!", "Now with less sugar." },
                Step.CurrentStep.Metadata["New"]);
        }
    }
}