using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Gallio.NUnitAdapter.TestResources.SetUpFixtureSample
{
    [SetUpFixture]
    public class SetUpFixture
    {
        public static bool IsSetUp;

        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("[SetUpFixture] SetUp");

            IsSetUp = true;
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("[SetUpFixture] TearDown");

            IsSetUp = false;
        }
    }
}