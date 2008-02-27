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

using System;
using System.IO;
using System.Xml.Serialization;
using Gallio.Runner.Reports;
using MbUnit.Framework.Xml;
using Gallio.Model.Serialization;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(TestInstanceRun))]
    [Author("Vadim")]
    public class TestInstanceRunTests
    {
        private TestInstanceData testInstanceData;
        private TestStepRun rootTestStepRun;
        private TestInstanceRun testInstanceRun;

        [SetUp]
        public void TestStart()
        {
            testInstanceData = new TestInstanceData("id", "name", "testId", false);

            rootTestStepRun = new TestStepRun(new TestStepData("root", "root", "test", "testId"));
            testInstanceRun = new TestInstanceRun(testInstanceData, rootTestStepRun);
        }

        [Test]
        public void ConstructorTest()
        {
            TestInstanceRun testInstanceRun = new TestInstanceRun(testInstanceData, rootTestStepRun);
            Assert.AreSame(testInstanceData, testInstanceRun.TestInstance);
            Assert.AreSame(rootTestStepRun, testInstanceRun.RootTestStepRun);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorWithNullInstance()
        {
            new TestInstanceRun(null, rootTestStepRun);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorWithNullTestStepRun()
        {
            new TestInstanceRun(testInstanceData, null);
        }

        [Test]
        public void TestInstanceTest()
        {
            Assert.AreSame(testInstanceData, testInstanceRun.TestInstance);

            TestInstanceData newInstance = new TestInstanceData("other", "other", "other", false);
            testInstanceRun.TestInstance = newInstance;

            Assert.AreSame(newInstance, testInstanceRun.TestInstance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void TestInstanceNullTest()
        {
            testInstanceRun.TestInstance = null;
        }

        [Test]
        public void RootTestStepRunTest()
        {
            Assert.AreSame(rootTestStepRun, testInstanceRun.RootTestStepRun);

            TestStepRun newRoot = new TestStepRun(new TestStepData("other", "other", "other", "other"));
            testInstanceRun.RootTestStepRun = newRoot;

            Assert.AreSame(newRoot, testInstanceRun.RootTestStepRun);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void RootTestStepRunNullTest()
        {
            testInstanceRun.RootTestStepRun = null;
        }

        [Test]
        public void StepRunsEnumerationTest()
        {
            TestStepRun testStepRunChild = new TestStepRun(new TestStepData("childId", "child", "fullName", "testId"));
            rootTestStepRun.Children.Add(testStepRunChild);

            CollectionAssert.AreElementsEqual(new TestStepRun[] { rootTestStepRun, testStepRunChild },
                testInstanceRun.TestStepRuns);
        }

        [Test]
        public void ReportTypeIsXmlSerializable()
        {
            XmlSerializationAssert.IsXmlSerializable(testInstanceRun.GetType());
        }

        [Test]
        public void RoundTripXmlSerialization()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestInstanceRun));
            testInstanceRun.RootTestStepRun.Children.Add(new TestStepRun(new TestStepData("childId", "childName", "fullName", "testId")));

            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, testInstanceRun);
            Console.WriteLine(writer.ToString());

            TestInstanceRun deserializedTestRun = (TestInstanceRun)serializer.Deserialize(new StringReader(writer.ToString()));

            ReportAssert.AreEqual(testInstanceRun, deserializedTestRun);
        }
    }
}