namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using System.Collections;
    using TestDriven.UnitTesting;
 
    public sealed class TestFixtureAttribute : TestFixtureAttributeBase 
    {
        public TestFixtureAttribute()
        {
        }

        public override ITestFixture[] CreateFixtures(Type fixtureType)
        {
            if (fixtureType.IsAbstract)
            {
                return new ITestFixture[0];
            }

            return new ITestFixture[] { new TestFixture(fixtureType) };
        }
    }
}
