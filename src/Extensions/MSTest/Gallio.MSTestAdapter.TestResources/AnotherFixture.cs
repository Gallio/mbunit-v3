using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gallio.MSTestAdapter.TestResources
{
    [TestClass]
    public class AnotherFixture
    {
        private static int staticCounter = 0;
        private int counter = 0;

        public AnotherFixture()
        {
            Console.WriteLine("AnotherFixture constructor");
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanupMethod()
        {
            Assert.Fail("Making Assembly Cleanup fail");
        }

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            staticCounter++;
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            Assert.AreEqual(staticCounter, 2);
        }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Assert.AreEqual(counter, 0);
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Assert.AreEqual(counter, 1);
        }

        [TestMethod]
        [Owner("Julian")]
        public void TestMethod1()
        {
            counter++;
            staticCounter++;
            Assert.AreEqual(1, 0 + 1);
        }

        [TestMethod]
        public void TestMethod2()
        {
            counter++;
            staticCounter++;
            Assert.AreEqual(1, 0 + 1);
        }

        [TestMethod]
        public void FailingTest()
        {
            counter++;
            staticCounter++;
            Assert.AreEqual(1, 2);
        }

        [TestMethod]
        [Ignore]
        public void IgnoredTest()
        {
            counter++;
            staticCounter++;
            Assert.AreEqual(1, 2);
        }

        [TestMethod]
        public void TestMethod3()
        {
            //Thread.Sleep(2000);
            counter++;
            staticCounter++;
            Assert.AreEqual(1, 0 + 1);
        }

        [TestMethod]
        public void TestMethod4()
        {
            counter++;
            staticCounter++;
            Assert.AreEqual(1, 0 + 1);
        }
    }
}
