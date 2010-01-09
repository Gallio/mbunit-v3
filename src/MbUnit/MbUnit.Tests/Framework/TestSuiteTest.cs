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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Action=Gallio.Common.Action;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(TestSuite))]
    [RunSample(typeof(DynamicSample))]
    [RunSample(typeof(StaticSample))]
    //[RunSample(typeof(ConcreteStaticSample))]
    public class TestSuiteTest : BaseTestWithSampleRunner
    {
        [Test]
        public void ConstructorRequiresName()
        {
            Assert.Throws<ArgumentNullException>(() => new TestSuite(null));

            TestSuite testSuite = new TestSuite("Name");
            Assert.AreEqual("Name", testSuite.Name);
        }

        [Test]
        public void DescriptionStoredInMetadata()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.Description);
            Assert.IsFalse(testSuite.Metadata.ContainsKey(MetadataKeys.Description));

            testSuite.Description = "Description";
            Assert.AreEqual("Description", testSuite.Description);
            Assert.AreEqual("Description", testSuite.Metadata.GetValue(MetadataKeys.Description));

            testSuite.Description = null;
            Assert.IsNull(testSuite.Description);
            Assert.IsFalse(testSuite.Metadata.ContainsKey(MetadataKeys.Description));
        }

        [Test]
        public void TimeoutMustBeNullOrPositive()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.Timeout);

            testSuite.Timeout = TimeSpan.FromSeconds(5);
            Assert.AreEqual(TimeSpan.FromSeconds(5), testSuite.Timeout);

            testSuite.Timeout = null;
            Assert.IsNull(testSuite.Timeout);

            Assert.Throws<ArgumentOutOfRangeException>(() => testSuite.Timeout = TimeSpan.FromSeconds(-1));
        }

        [Test]
        public void CodeElement()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.CodeElement);

            ITypeInfo type = Reflector.Wrap(typeof(TestSuite));
            testSuite.CodeElement = type;
            Assert.AreSame(type, testSuite.CodeElement);
        }

        [Test]
        public void SetUp()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.SetUp);

            Action action = delegate { };
            testSuite.SetUp = action;
            Assert.AreSame(action, testSuite.SetUp);
        }

        [Test]
        public void TearDown()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.TearDown);

            Action action = delegate { };
            testSuite.TearDown = action;
            Assert.AreSame(action, testSuite.TearDown);
        }

        [Test]
        public void SuiteSetUp()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.SuiteSetUp);

            Action action = delegate { };
            testSuite.SuiteSetUp = action;
            Assert.AreSame(action, testSuite.SuiteSetUp);
        }

        [Test]
        public void SuiteTearDown()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.SuiteTearDown);

            Action action = delegate { };
            testSuite.SuiteTearDown = action;
            Assert.AreSame(action, testSuite.SuiteTearDown);
        }

        [Test]
        public void DynamicRun()
        {
            TestData factoryData = Runner.GetTestData(CodeReference.CreateFromMember(
                typeof(DynamicSample).GetMethod("Factory")));
            Assert.AreEqual(0, factoryData.Children.Count);

            TestStepRun factoryRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(
                typeof(DynamicSample).GetMethod("Factory")));
            Assert.AreEqual(1, factoryRun.Children.Count);

            TestStepRun suiteRun = factoryRun.Children[0];
            Assert.AreEqual("Suite", suiteRun.Step.Name);
            Assert.IsTrue(suiteRun.Step.IsDynamic);
            Assert.IsFalse(suiteRun.Step.IsPrimary);
            Assert.IsFalse(suiteRun.Step.IsTestCase);
            Assert.AreEqual(factoryData.Id, suiteRun.Step.TestId);
            Assert.AreEqual("System.Int32", suiteRun.Step.CodeReference.TypeName);
            Assert.AreEqual("Me", suiteRun.Step.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual(TestKinds.Suite, suiteRun.Step.Metadata.GetValue(MetadataKeys.TestKind));
            Assert.AreEqual("*** Log ***\n\nSuiteSetUp\nSuiteTearDown\n", suiteRun.TestLog.ToString());
            Assert.AreEqual(3, suiteRun.Children.Count);

            // Order matters.
            TestStepRun test1Run = suiteRun.Children[0];
            Assert.AreEqual("Test1", test1Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute1\nTearDown\n", test1Run.TestLog.ToString());
            Assert.IsTrue(test1Run.Step.IsDynamic);
            Assert.IsFalse(test1Run.Step.IsPrimary);
            Assert.IsTrue(test1Run.Step.IsTestCase);
            Assert.AreEqual(factoryData.Id, test1Run.Step.TestId);

            TestStepRun test3Run = suiteRun.Children[1];
            Assert.AreEqual("Test3", test3Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute3\nTearDown\n", test3Run.TestLog.ToString());
            Assert.IsTrue(test3Run.Step.IsDynamic);
            Assert.IsFalse(test3Run.Step.IsPrimary);
            Assert.IsTrue(test3Run.Step.IsTestCase);
            Assert.AreEqual(factoryData.Id, test3Run.Step.TestId);

            TestStepRun test2Run = suiteRun.Children[2];
            Assert.AreEqual("Test2", test2Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute2\nTearDown\n", test2Run.TestLog.ToString());
            Assert.IsTrue(test2Run.Step.IsDynamic);
            Assert.IsFalse(test2Run.Step.IsPrimary);
            Assert.IsTrue(test2Run.Step.IsTestCase);
            Assert.AreEqual(factoryData.Id, test2Run.Step.TestId);
        }

        [Test]
        public void StaticRun()
        {
            TestData fixtureData = Runner.GetTestData(CodeReference.CreateFromType(typeof(StaticSample)));
            Assert.AreEqual(1, fixtureData.Children.Count);

            TestData suiteData = fixtureData.Children[0];
            Assert.AreEqual("Suite", suiteData.Name);
            Assert.IsFalse(suiteData.IsTestCase);
            Assert.AreEqual("System.Int32", suiteData.CodeReference.TypeName);
            Assert.AreEqual("Me", suiteData.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual(TestKinds.Suite, suiteData.Metadata.GetValue(MetadataKeys.TestKind));
            Assert.AreEqual(3, suiteData.Children.Count);

            // Order matters.
            TestData test1Data = suiteData.Children[0];
            Assert.AreEqual("Test1", test1Data.Name);

            TestData test3Data = suiteData.Children[1];
            Assert.AreEqual("Test3", test3Data.Name);

            TestData test2Data = suiteData.Children[2];
            Assert.AreEqual("Test2", test2Data.Name);

            TestStepRun fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(StaticSample)));
            Assert.AreEqual(1, fixtureRun.Children.Count);

            TestStepRun suiteRun = fixtureRun.Children[0];
            Assert.AreEqual("Suite", suiteRun.Step.Name);
            Assert.IsFalse(suiteRun.Step.IsDynamic);
            Assert.IsTrue(suiteRun.Step.IsPrimary);
            Assert.IsFalse(suiteRun.Step.IsTestCase);
            Assert.AreEqual(suiteData.Id, suiteRun.Step.TestId);
            Assert.AreEqual("System.Int32", suiteRun.Step.CodeReference.TypeName);
            Assert.AreEqual("Me", suiteRun.Step.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual(TestKinds.Suite, suiteRun.Step.Metadata.GetValue(MetadataKeys.TestKind));
            Assert.AreEqual("*** Log ***\n\nSuiteSetUp\nSuiteTearDown\n", suiteRun.TestLog.ToString());
            Assert.AreEqual(3, suiteRun.Children.Count);

            // Order matters.
            TestStepRun test1Run = suiteRun.Children[0];
            Assert.AreEqual("Test1", test1Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute1\nTearDown\n", test1Run.TestLog.ToString());
            Assert.IsFalse(test1Run.Step.IsDynamic);
            Assert.IsTrue(test1Run.Step.IsPrimary);
            Assert.IsTrue(test1Run.Step.IsTestCase);
            Assert.AreEqual(test1Data.Id, test1Run.Step.TestId);

            TestStepRun test3Run = suiteRun.Children[1];
            Assert.AreEqual("Test3", test3Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute3\nTearDown\n", test3Run.TestLog.ToString());
            Assert.IsFalse(test3Run.Step.IsDynamic);
            Assert.IsTrue(test3Run.Step.IsPrimary);
            Assert.IsTrue(test3Run.Step.IsTestCase);
            Assert.AreEqual(test3Data.Id, test3Run.Step.TestId);

            TestStepRun test2Run = suiteRun.Children[2];
            Assert.AreEqual("Test2", test2Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute2\nTearDown\n", test2Run.TestLog.ToString());
            Assert.IsFalse(test2Run.Step.IsDynamic);
            Assert.IsTrue(test2Run.Step.IsPrimary);
            Assert.IsTrue(test2Run.Step.IsTestCase);
            Assert.AreEqual(test2Data.Id, test2Run.Step.TestId);
        }

        /*[Test]
        [Pending("Issue 525 to be fixed.")]
        public void StaticTestFactoryOnAbstractType() // Issue 525 (http://code.google.com/p/mb-unit/issues/detail?id=525)
        {
            var run = Runner.GetPrimaryTestStepRun(x => x.Step.FullName.EndsWith("DynamicSampleMethod"));
            Assert.IsNotNull(run);
            AssertLogContains(run, "Hello from SampleMethod!");
        }*/

        private static readonly Test[] tests = new Test[]
        {
            new TestSuite("Suite")
            {
                Children = {
                    // Deliberately out of order to verify that tests run in order specified
                    // not in order by name.
                    new TestCase("Test1", () => TestLog.WriteLine("Execute1")),
                    new TestCase("Test3", () => TestLog.WriteLine("Execute3")),
                    new TestCase("Test2", () => TestLog.WriteLine("Execute2"))
                },
                CodeElement = Reflector.Wrap(typeof(Int32)),
                Description = "Description",
                Metadata = { { MetadataKeys.AuthorName, "Me" }},
                SuiteSetUp = () => TestLog.WriteLine("SuiteSetUp"),
                SuiteTearDown = () => TestLog.WriteLine("SuiteTearDown"),
                SetUp = () => TestLog.WriteLine("SetUp"),
                TearDown = () => TestLog.WriteLine("TearDown")
            }
        };

        [Explicit("Sample")]
        public class DynamicSample
        {
            [DynamicTestFactory]
            public IEnumerable<Test> Factory()
            {
                return tests;
            }
        }

        [Explicit("Sample")]
        public class StaticSample
        {
            [StaticTestFactory]
            public static IEnumerable<Test> Factory()
            {
                return tests;
            }
        }

        [Explicit("Sample")]
        public class ReferencedFixture
        {
            [Test]
            public void Test()
            {
                TestLog.WriteLine("Test Ran");
            }
        }

        /*[TestFixture, Explicit("Sample")]
        public abstract class AbstractStaticSample<T>
        {
            [StaticTestFactory]
            public static IEnumerable<Test> TestSuite()
            {
                var methodInfo = typeof(T).GetMethod("SampleMethod");

                yield return new TestSuite("SampleSuite")
                {
                    Children =
                    {
                        new TestCase("DynamicSampleMethod", () => methodInfo.Invoke(null, null))
                    }
                };
            }
        }

        [TestFixture, Explicit("Sample")]
        public class ConcreteStaticSample : AbstractStaticSample<ConcreteStaticSample>
        {
            public static void SampleMethod()
            {
                TestLog.Write("Hello from SampleMethod!");
            }
        }*/
    }
}
