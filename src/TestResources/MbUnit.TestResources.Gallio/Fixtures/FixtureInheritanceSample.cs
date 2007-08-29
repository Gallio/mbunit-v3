using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace MbUnit.TestResources.Gallio.Fixtures
{
    /// <summary>
    /// This fixture is used to ensure that test fixtures with inherited attributes
    /// compose in the desired manner.
    /// </summary>
    [TestFixture]
    public class FixtureInheritanceSample
    {
        [TestFixtureSetUp]
        public void BaseTestFixtureSetUp()
        {
            Console.WriteLine("BaseTestFixtureSetUp");
        }

        [TestFixtureTearDown]
        public void BaseTestFixtureTearDown()
        {
            Console.WriteLine("BaseTestFixtureTearDown");
        }

        [SetUp]
        public void BaseSetUp()
        {
            Console.WriteLine("BaseSetUp");
        }

        [TearDown]
        public void BaseTearDown()
        {
            Console.WriteLine("BaseTearDown");
        }

        [Test]
        public void BaseTest()
        {
            Console.WriteLine("BaseTest");
        }

        [TestFixture]
        public class DerivedFixture : FixtureInheritanceSample
        {
            [TestFixtureSetUp]
            public void DerivedTestFixtureSetUp()
            {
                Console.WriteLine("DerivedTestFixtureSetUp");
            }

            [TestFixtureTearDown]
            public void DerivedTestFixtureTearDown()
            {
                Console.WriteLine("DerivedTestFixtureTearDown");
            }

            [SetUp]
            public void DerivedSetUp()
            {
                Console.WriteLine("DerivedSetUp");
            }

            [TearDown]
            public void DerivedTearDown()
            {
                Console.WriteLine("DerivedTearDown");
            }

            [Test]
            public void DerivedTest()
            {
                Console.WriteLine("DerivedTest");
            }
        }
    }
}
