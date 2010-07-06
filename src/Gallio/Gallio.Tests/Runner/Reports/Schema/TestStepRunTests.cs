// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Model;
using Gallio.Common.Markup;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Reports.Schema
{
    [TestFixture]
    [TestsOn(typeof(TestStepRun))]
    [Author("Vadim")]
    public class TestStepRunTests
    {
        private TestStepRun testStepRun;

        [SetUp]
        public void TestStart()
        {
            testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testId"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorExceptionTest()
        {
            new TestStepRun(null);
        }

        [Test]
        public void ConstructorTest()
        {
            TestStepData step = new TestStepData("id", "name", "fullName", "testId");
            TestStepRun testStepRun = new TestStepRun(step);
            Assert.AreSame(step, testStepRun.Step);
            Assert.Count(0, testStepRun.Children);
        }

        [Test]
        public void ResultSetTest()
        {
            testStepRun.Result = new TestResult();
            Assert.IsNotNull(testStepRun.Result);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ResultSetExceptionTest()
        {
            testStepRun.Result = null;
        }

        [Test]
        public void ResultGetTest()
        {
            Assert.IsNotNull(testStepRun.Result);
        }

        [Test]
        public void StepSetTest()
        {
            testStepRun.Step = new TestStepData("stepId", "stepName", "fullName", "testId");
            Assert.IsNotNull(testStepRun.Step);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StepSetExceptionTest()
        {
            testStepRun.Step = null;
        }

        [Test]
        public void StepGetTest()
        {
            Assert.IsNotNull(testStepRun.Step);
        }

        [Test]
        public void TestLogSetTest()
        {
            testStepRun.TestLog = new StructuredDocument();
            Assert.IsNotNull(testStepRun.TestLog);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void TestLogSetExceptionTest()
        {
            testStepRun.TestLog = null;
        }

        [Test]
        public void TestLogGetTest()
        {
            Assert.IsNotNull(testStepRun.TestLog);
        }
    }
}