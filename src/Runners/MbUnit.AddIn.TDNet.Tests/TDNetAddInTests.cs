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
using System.Reflection;
using MbUnit2::MbUnit.Framework;
using System;
using MbUnit.AddIn.TDNet;
using Rhino.Mocks;
using TDF = TestDriven.Framework;

namespace MbUnit.AddIn.TDNet.Tests
{
    [TestFixture]
    [TestsOn(typeof(MbUnitTestRunner))]
    [Author("Julian Hidalgo")]
    public class TDNetAddInTests
    {
        private TDF.ITestListener stubbedITestListener = null;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            stubbedITestListener = MockRepository.GenerateStub<TDF.ITestListener>();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunAssembly_NullITestListener()
        {
            TDF.ITestRunner tr = new MbUnitTestRunner();
            tr.RunAssembly(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunAssembly_NullAssembly()
        {

            TDF.ITestRunner tr = new MbUnitTestRunner();
            tr.RunAssembly(stubbedITestListener, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNamespace_NullITestListener()
        {
            TDF.ITestRunner tr = new MbUnitTestRunner();
            tr.RunNamespace(null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNamespace_NullAssembly()
        {
            TDF.ITestRunner tr = new MbUnitTestRunner();
            tr.RunNamespace(stubbedITestListener, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunNamespace_NullNamespace()
        {
            TDF.ITestRunner tr = new MbUnitTestRunner();
            tr.RunNamespace(stubbedITestListener, Assembly.GetExecutingAssembly(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunMember_NullITestListener()
        {
            TDF.ITestRunner tr = new MbUnitTestRunner();
            tr.RunMember(null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunMember_NullAssembly()
        {
            TDF.ITestRunner tr = new MbUnitTestRunner();
            tr.RunMember(stubbedITestListener, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RunMember_NullMember()
        {
            TDF.ITestRunner tr = new MbUnitTestRunner();
            tr.RunMember(stubbedITestListener, Assembly.GetExecutingAssembly(), null);
        }
    }
}
