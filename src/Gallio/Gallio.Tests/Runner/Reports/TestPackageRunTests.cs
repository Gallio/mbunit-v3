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

using Gallio.Runner.Reports;
using Gallio.Model.Serialization;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(TestPackageRun))]
    public class TestPackageRunTests
    {
        private TestPackageRun testPackageRun;

        [SetUp]
        public void TestStart()
        {
            testPackageRun = new TestPackageRun();
        }

        [Test]
        public void StatisticsGetTest()
        {
            Assert.IsNotNull(testPackageRun.Statistics);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StatisticsSetWithNullValueTest()
        {
            testPackageRun.Statistics = null;
        }

        [Test]
        public void StatisticsSetTest()
        {
            Statistics statistics = new Statistics();
            testPackageRun.Statistics = statistics;
            Assert.AreSame(statistics, testPackageRun.Statistics);
        }

        [Test]
        public void ReportTypeIsXmlSerializable()
        {
            Assert.IsXmlSerializableType(testPackageRun.GetType());
        }

        [Test]
        public void RoundTripXmlSerialization()
        {
            testPackageRun.RootTestStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "testId"));

            TestPackageRun deserializedTestPackageRun = Assert.XmlSerializeThenDeserialize(testPackageRun);
            ReportAssert.AreEqual(testPackageRun, deserializedTestPackageRun);
        }

        [Test]
        public void TestInstanceRuns()
        {
            TestStepRun testStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "testId"));
            testStepRun.Children.Add(new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "testId")));
            testPackageRun.RootTestStepRun = testStepRun;

            Assert.AreElementsEqual(new TestStepRun[] { testStepRun, testStepRun.Children[0] },
                testPackageRun.AllTestStepRuns);
        }
    }
}