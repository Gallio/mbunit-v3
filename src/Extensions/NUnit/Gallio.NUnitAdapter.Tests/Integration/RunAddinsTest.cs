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

using System.Collections.Generic;
using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.NUnitAdapter.TestResources;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

namespace Gallio.NUnitAdapter.Tests.Integration
{
    [TestFixture]
    [RunSample(typeof(AddinsTest))]
    public class RunAddinsTest : BaseTestWithSampleRunner
    {
        [Test]
        public void RowTest()
        {
            var runs = new List<TestStepRun>(Runner.GetTestStepRuns(
                CodeReference.CreateFromMember(typeof(AddinsTest).GetMethod("RowTest"))));

            Assert.AreEqual(2, runs.Count);

            // Note: Description not set by NUnit.
            // Bug: http://sourceforge.net/tracker/index.php?func=detail&aid=2125208&group_id=10749&atid=110749

            //Assert.AreEqual("Pass", runs[0].Step.Metadata.GetValue(MetadataKeys.Description));
            Assert.AreEqual("RowTest(1, 2, 3)", runs[0].Step.Name);
            Assert.AreEqual(TestStatus.Passed, runs[0].Result.Outcome.Status);

            //Assert.AreEqual("Fail", runs[1].Step.Metadata.GetValue(MetadataKeys.Description));
            Assert.AreEqual("RowTest(2, 2, 5)", runs[1].Step.Name);
            Assert.AreEqual(TestStatus.Failed, runs[1].Result.Outcome.Status);
        }
    }
}
