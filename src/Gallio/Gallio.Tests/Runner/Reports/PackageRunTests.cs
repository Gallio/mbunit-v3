// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using Gallio.Runner.Reports;
using MbUnit.Framework.Xml;
using Gallio.Model.Serialization;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(PackageRun))]
    [Author("Vadim")]
    public class PackageRunTests
    {
        private PackageRun packageRun;

        [SetUp]
        public void TestStart()
        {
            packageRun = new PackageRun();
        }

        [Test]
        public void StatisticsGetTest()
        {
            Assert.IsNotNull(packageRun.Statistics);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StatisticsSetWithNullValueTest()
        {
            packageRun.Statistics = null;
        }

        [Test]
        public void StatisticsSetTest()
        {
            PackageRunStatistics statistics = new PackageRunStatistics();
            packageRun.Statistics = statistics;
            Assert.AreSame(statistics, packageRun.Statistics);
        }

        [Test]
        public void ReportTypeIsXmlSerializable()
        {
            XmlSerializationAssert.IsXmlSerializable(packageRun.GetType());
        }

        [Test]
        public void RoundTripXmlSerialization()
        {
            packageRun.RootTestStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "testId"));

            PackageRun deserializedPackageRun = XmlSerializationAssert.RoundTrip(packageRun);
            ReportAssert.AreEqual(packageRun, deserializedPackageRun);
        }

        [Test]
        public void TestInstanceRuns()
        {
            TestStepRun testStepRun = new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "testId"));
            testStepRun.Children.Add(new TestStepRun(new TestStepData("stepId", "stepName", "stepFullName", "testId")));
            packageRun.RootTestStepRun = testStepRun;

            CollectionAssert.AreElementsEqual(new TestStepRun[] { testStepRun, testStepRun.Children[0] },
                packageRun.TestStepRuns);
        }
    }
}