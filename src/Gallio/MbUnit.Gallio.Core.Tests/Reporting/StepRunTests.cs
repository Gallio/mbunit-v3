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
using MbUnit.Core.Reporting;
using MbUnit.Framework.Kernel.Results;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Reporting
{
    [TestFixture]
    [TestsOn(typeof(StepRun))]
    [Author("Vadim")]
    public class StepRunTests
    {
        private StepRun _stepRun;

        [SetUp]
        public void TestStart()
        {
            _stepRun = new StepRun("id", "name", "fullName");
        }

        [RowTest]
        [Row(null, "name", "fullName", ExpectedException = typeof(ArgumentNullException))]
        [Row("id", null, "fullName", ExpectedException = typeof(ArgumentNullException))]
        [Row("id", "name", null, ExpectedException = typeof(ArgumentNullException))]
        public void ConstructorExceptionTest(string id, string name, string fullName)
        {
            new StepRun(id, name, fullName);
        }

        [Test]
        public void ConstructorTest()
        {
            StepRun stepRun = new StepRun("id", "name", "fullName");
            Assert.AreEqual("id", stepRun.StepId);
            Assert.AreEqual("name", stepRun.StepName);
            Assert.AreEqual("fullName", stepRun.StepFullName);
            Assert.AreEqual(0, stepRun.Children.Count);
        }

        [RowTest]
        [Row("newId")]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        public void StepIdSetTest(string stepId)
        {
            _stepRun.StepId = stepId;
            Assert.AreEqual(stepId, _stepRun.StepId);
        }

        [RowTest]
        [Row("newName")]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        public void StepNameSetTest(string stepName)
        {
            _stepRun.StepName = stepName;
            Assert.AreEqual(stepName, _stepRun.StepName);
        }

        [RowTest]
        [Row("newFullName")]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        public void StepFullNameSetTest(string stepFullName)
        {
            _stepRun.StepFullName = stepFullName;
            Assert.AreEqual(stepFullName, _stepRun.StepFullName);
        }

        [Test]
        public void ResultSetTest()
        {
            _stepRun.Result = new TestResult();
            Assert.IsNotNull(_stepRun.Result);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ResultSetExceptionTest()
        {
            _stepRun.Result = null;
        }

        [Test]
        public void ResultGetTest()
        {
            Assert.IsNotNull(_stepRun.Result);
        }

        [Test]
        public void ExecutionLogSetTest()
        {
            _stepRun.ExecutionLog = new ExecutionLog();
            Assert.IsNotNull(_stepRun.ExecutionLog);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ExecutionLogSetExceptionTest()
        {
            _stepRun.ExecutionLog = null;
        }

        [Test]
        public void ExecutionLogGetTest()
        {
            Assert.IsNotNull(_stepRun.ExecutionLog);
        }
    }
}