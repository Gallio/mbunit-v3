using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MbUnit.TestResources.Xunit
{
    /// <summary>
    /// A test fixture with before/after sections.
    /// </summary>
    public class BeforeAndAfterSample : ITestFixture
    {
        public void BeforeAllTests()
        {
            Console.WriteLine("Before");
        }

        [Test]
        public void Test1()
        {
            Console.WriteLine("Test1");
        }

        [Test]
        public void Test2()
        {
            Console.WriteLine("Test2");
        }

        public void AfterAllTests()
        {
            Console.WriteLine("After");
        }
    }
}
