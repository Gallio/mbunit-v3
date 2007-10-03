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

extern alias MbUnit2;
using System;
using System.IO;
using System.Xml.Serialization;
using MbUnit.Core.Model;
using MbUnit.Core.Reporting;
using MbUnit.Framework.Xml;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Reporting
{
    [TestFixture]
    [TestsOn(typeof(TestRun))]
    [Author("Vadim")]
    public class TestRunTests
    {
        private StepRun rootStepRun;
        private TestRun testRun;

        [SetUp]
        public void TestStart()
        {
            rootStepRun = new StepRun(new StepData("root", "root", "test", "testId"));
            testRun = new TestRun("id", rootStepRun);
        }

        [RowTest]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        [Row("newId")]
        public void ConstructorTest(string testId)
        {
            TestRun testRun = new TestRun(testId, rootStepRun);
            Assert.AreEqual(testId, testRun.TestId);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorWithNullStepRun()
        {
            new TestRun("id", null);
        }

        [RowTest]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        [Row("newId")]
        public void TestIdSetTest(string testId)
        {
            testRun.TestId = testId;
            Assert.AreEqual(testId, testRun.TestId);
        }

        [Test]
        public void RootStepRunTest()
        {
            Assert.AreSame(rootStepRun, testRun.RootStepRun);

            StepRun newRoot = new StepRun(new StepData("other", "other", "other", "other"));
            testRun.RootStepRun = newRoot;

            Assert.AreSame(newRoot, testRun.RootStepRun);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void RootStepRunNullTest()
        {
            testRun.RootStepRun = null;
        }

        [Test]
        public void StepRunsEnumerationTest()
        {
            StepRun stepRunChild = new StepRun(new StepData("childId", "child", "fullName", "testId"));
            rootStepRun.Children.Add(stepRunChild);

            CollectionAssert.AreElementsEqual(new StepRun[] { rootStepRun, stepRunChild },
                testRun.StepRuns);
        }

        [Test]
        public void ReportTypeIsXmlSerializable()
        {
            XmlSerializationAssert.IsXmlSerializable(testRun.GetType());
        }

        [Test]
        public void RoundTripXmlSerialization()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestRun));
            StringWriter writer = new StringWriter();
            testRun.RootStepRun.Children.Add(new StepRun(new StepData("childId", "childName", "fullName", "testId")));
            serializer.Serialize(writer, testRun);
            TestRun deserializedTestRun = (TestRun)serializer.Deserialize(new StringReader(writer.ToString()));
            CoreAssert.AreEqual(testRun, deserializedTestRun);
        }

    }
}