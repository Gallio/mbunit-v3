using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Model.Serialization;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(TestFixtureReference))]
    //[RunSample(typeof(DynamicSample))]
    [RunSample(typeof(StaticSample))]
    public class TestFixtureReferenceTest : BaseTestWithSampleRunner
    {
        [Test]
        public void ConstructorRequiresFixtureType()
        {
            Assert.Throws<ArgumentNullException>(() => new TestFixtureReference(null));

            TestFixtureReference reference = new TestFixtureReference(typeof(TestFixtureReferenceTest));
            Assert.AreEqual(typeof(TestFixtureReferenceTest), reference.TestFixtureType);
        }

        [Test, Pending("Dynamic references to test fixtures not supported yet.")]
        public void DynamicRun()
        {
        }

        [Test]
        public void StaticRun()
        {
            TestData fixtureTest = Runner.GetTestData(CodeReference.CreateFromType(typeof(StaticSample)));
            Assert.AreEqual(1, fixtureTest.Children.Count);

            TestData referenceData = fixtureTest.Children[0];
            Assert.AreEqual("ReferencedFixture", referenceData.Name);
            Assert.IsFalse(referenceData.IsTestCase);
            Assert.AreEqual(1, referenceData.Children.Count);

            TestData testData = referenceData.Children[0];
            Assert.AreEqual("Test", testData.Name);

            TestStepRun fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(StaticSample)));
            Assert.AreEqual(1, fixtureRun.Children.Count);

            TestStepRun suiteRun = fixtureRun.Children[0];
            Assert.AreEqual("ReferencedFixture", suiteRun.Step.Name);
            Assert.IsFalse(suiteRun.Step.IsDynamic);
            Assert.IsTrue(suiteRun.Step.IsPrimary);
            Assert.IsFalse(suiteRun.Step.IsTestCase);
            Assert.AreEqual("", suiteRun.TestLog.ToString());
            Assert.AreEqual(1, suiteRun.Children.Count);

            TestStepRun testRun = suiteRun.Children[0];
            Assert.AreEqual("Test", testRun.Step.Name);
            Assert.AreEqual("*** Log ***\n\nExecute\n", testRun.TestLog.ToString());
        }

        private static readonly Test[] tests = new Test[]
        {
            new TestFixtureReference(typeof(ReferencedFixture))
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

        public class ReferencedFixture
        {
            [Test]
            public void Test()
            {
                TestLog.WriteLine("Execute");
            }
        }
    }
}
