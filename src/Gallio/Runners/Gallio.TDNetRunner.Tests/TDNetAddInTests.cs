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

using System;
using System.Reflection;
using MbUnit.Framework;
using Gallio.TestResources.MbUnit2;
using Rhino.Mocks;
using TestDriven.Framework;
using TDF = TestDriven.Framework;

namespace Gallio.TDNetRunner.Tests
{
    [TestFixture]
    [TestsOn(typeof(InstrumentedGallioTestRunner))]
    [Author("Julian Hidalgo")]
    public class TDNetAddInTests
    {
        #region Private Members

        private ITestListener stubbedITestListener = null;
        private Assembly testAssembly = null;

        #endregion

        #region SetUp and TearDown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            stubbedITestListener = MockRepository.GenerateStub<ITestListener>();
            testAssembly = typeof(SimpleTest).Assembly;
        }

        #endregion

        #region Tests

        #region Instantiation

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunAssembly_NullITestListener()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            tr.RunAssembly(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunAssembly_NullAssembly()
        {

            ITestRunner tr = new InstrumentedGallioTestRunner();
            tr.RunAssembly(stubbedITestListener, null);
        }

        [Test]
        public void ExecutionTest_NullTestListener()
        {
            //ITestRunner
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNamespace_NullITestListener()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            tr.RunNamespace(null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNamespace_NullAssembly()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            tr.RunNamespace(stubbedITestListener, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNamespace_NullNamespace()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            tr.RunNamespace(stubbedITestListener, Assembly.GetExecutingAssembly(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunMember_NullITestListener()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            tr.RunMember(null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunMember_NullAssembly()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            tr.RunMember(stubbedITestListener, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunMember_NullMember()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            tr.RunMember(stubbedITestListener, Assembly.GetExecutingAssembly(), null);
        }

        [Test]
        public void RunAssembly_NoTests()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            TestRunState result = tr.RunAssembly(stubbedITestListener, Assembly.GetAssembly(typeof(GallioTestRunner)));
            Assert.AreEqual(result, TestRunState.NoTests);
        }

        #endregion

        #region Run*

        [Test]
        public void RunAssembly()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            TestRunState result = tr.RunAssembly(stubbedITestListener, testAssembly);
            Assert.AreEqual(result, TestRunState.Failure);
        }

        [Test]
        public void RunMember_IgnoredTests()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            Type t = typeof(IgnoredTests);
            TestRunState result = tr.RunMember(stubbedITestListener, testAssembly, t);
            Assert.AreEqual(result, TestRunState.Success);
        }

        [Test]
        public void RunMember_PassingTests()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            Type t = typeof(PassingTests);
            TestRunState result = tr.RunMember(stubbedITestListener, testAssembly, t);
            Assert.AreEqual(result, TestRunState.Success);
        }

        [Test]
        public void RunMember_InvalidMember()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            Type t = typeof(PassingTests);
            ConstructorInfo memberInfo = t.GetConstructors()[0];
            TestRunState result = tr.RunMember(stubbedITestListener, testAssembly, memberInfo);
            Assert.AreEqual(result, TestRunState.NoTests);
        }

        [Test]
        public void RunMember_RunPassingTest()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            Type t = typeof(PassingTests);
            MethodInfo memberInfo = t.GetMethod("Pass");
            TestRunState result = tr.RunMember(stubbedITestListener, testAssembly, memberInfo);
            Assert.AreEqual(result, TestRunState.Success);
        }

        [Test]
        public void RunMember_RunFailingTest()
        {
            ITestRunner tr = new InstrumentedGallioTestRunner();
            Type t = typeof(FailingFixture);
            MethodInfo memberInfo = t.GetMethod("Fail");
            TestRunState result = tr.RunMember(stubbedITestListener, testAssembly, memberInfo);
            Assert.AreEqual(result, TestRunState.Failure);
        }

        #endregion

        #endregion
    }
}
