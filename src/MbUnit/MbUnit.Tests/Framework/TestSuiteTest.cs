using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(TestSuite))]
    [RunSample(typeof(DynamicSample))]
    [RunSample(typeof(StaticSample))]
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

            Gallio.Action action = delegate { };
            testSuite.SetUp = action;
            Assert.AreSame(action, testSuite.SetUp);
        }

        [Test]
        public void TearDown()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.TearDown);

            Gallio.Action action = delegate { };
            testSuite.TearDown = action;
            Assert.AreSame(action, testSuite.TearDown);
        }

        [Test]
        public void SuiteSetUp()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.SuiteSetUp);

            Gallio.Action action = delegate { };
            testSuite.SuiteSetUp = action;
            Assert.AreSame(action, testSuite.SuiteSetUp);
        }

        [Test]
        public void SuiteTearDown()
        {
            TestSuite testSuite = new TestSuite("Name");
            Assert.IsNull(testSuite.SuiteTearDown);

            Gallio.Action action = delegate { };
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
            Assert.AreEqual("System.Int32", suiteRun.Step.CodeReference.TypeName);
            Assert.AreEqual("Me", suiteRun.Step.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual(TestKinds.Suite, suiteRun.Step.Metadata.GetValue(MetadataKeys.TestKind));
            Assert.AreEqual("*** Log ***\n\nSuiteSetUp\nSuiteTearDown\n", suiteRun.TestLog.ToString());
            Assert.AreEqual(2, suiteRun.Children.Count);

            TestStepRun test1Run = suiteRun.Children[0];
            Assert.AreEqual("Test1", test1Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute1\nTearDown\n", test1Run.TestLog.ToString());

            TestStepRun test2Run = suiteRun.Children[1];
            Assert.AreEqual("Test2", test2Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute2\nTearDown\n", test2Run.TestLog.ToString());
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
            Assert.AreEqual(2, suiteData.Children.Count);

            TestData test1Data = suiteData.Children[0];
            Assert.AreEqual("Test1", test1Data.Name);

            TestData test2Data = suiteData.Children[1];
            Assert.AreEqual("Test2", test2Data.Name);

            TestStepRun fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(
                typeof(DynamicSample).GetMethod("Factory")));
            Assert.AreEqual(1, fixtureRun.Children.Count);

            TestStepRun suiteRun = fixtureRun.Children[0];
            Assert.AreEqual("Suite", suiteRun.Step.Name);
            Assert.IsTrue(suiteRun.Step.IsDynamic);
            Assert.IsFalse(suiteRun.Step.IsPrimary);
            Assert.IsFalse(suiteRun.Step.IsTestCase);
            Assert.AreEqual("System.Int32", suiteRun.Step.CodeReference.TypeName);
            Assert.AreEqual("Me", suiteRun.Step.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual(TestKinds.Suite, suiteRun.Step.Metadata.GetValue(MetadataKeys.TestKind));
            Assert.AreEqual("*** Log ***\n\nSuiteSetUp\nSuiteTearDown\n", suiteRun.TestLog.ToString());
            Assert.AreEqual(2, suiteRun.Children.Count);

            TestStepRun test1Run = suiteRun.Children[0];
            Assert.AreEqual("Test1", test1Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute1\nTearDown\n", test1Run.TestLog.ToString());

            TestStepRun test2Run = suiteRun.Children[1];
            Assert.AreEqual("Test2", test2Run.Step.Name);
            Assert.AreEqual("*** Log ***\n\nSetUp\nExecute2\nTearDown\n", test2Run.TestLog.ToString());
        }

        private static readonly Test[] tests = new Test[]
        {
            new TestSuite("Suite")
            {
                Children = {
                    new TestCase("Test1", () => TestLog.WriteLine("Execute1")),
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
    }
}
