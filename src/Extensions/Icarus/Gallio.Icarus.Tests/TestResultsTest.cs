using System.Drawing;

using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [TestFixture]
    public class TestResultsTest
    {
        private TestResults testResults;

        [SetUp]
        public void SetUp()
        {
            testResults = new TestResults();
        }

        [Test]
        public void Passed_Test()
        {
            Assert.AreEqual(0, testResults.Passed);
            testResults.Passed = 55;
            Assert.AreEqual(55, testResults.Passed);
        }

        [Test]
        public void Failed_Test()
        {
            Assert.AreEqual(0, testResults.Failed);
            testResults.Failed = 55;
            Assert.AreEqual(55, testResults.Failed);
        }

        [Test]
        public void Inconclusive_Test()
        {
            Assert.AreEqual(0, testResults.Inconclusive);
            testResults.Inconclusive = 55;
            Assert.AreEqual(55, testResults.Inconclusive);
        }

        [Test]
        public void Total_Test()
        {
            Assert.AreEqual(0, testResults.Total);
            testResults.Total = 55;
            Assert.AreEqual(55, testResults.Total);
        }

        [Test]
        public void Reset_Test()
        {
            Assert.AreEqual(0, testResults.Passed);
            Assert.AreEqual(0, testResults.Failed);
            Assert.AreEqual(0, testResults.Inconclusive);
            testResults.Passed = 55;
            testResults.Failed = 10;
            testResults.Inconclusive = 5;
            Assert.AreEqual(55, testResults.Passed);
            Assert.AreEqual(10, testResults.Failed);
            Assert.AreEqual(5, testResults.Inconclusive);
            testResults.Reset();
            Assert.AreEqual(0, testResults.Passed);
            Assert.AreEqual(0, testResults.Failed);
            Assert.AreEqual(0, testResults.Inconclusive);
        }

        [Test]
        public void UpdateTestResults_Test()
        {
            testResults.UpdateTestResults("test", "outcome", Color.Black, "duration", "type", "namespace", "assembly");
        }
    }
}