namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using System.Collections;
    using TestDriven.UnitTesting;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TestFixtureSetUpAttribute : Attribute, ITestCaseFactory
    {
        public ITestCase[] CreateTests(ITestFixture fixture, MethodInfo method)
        {
            object fixtureObject = fixture.CreateInstance();
            return new ITestCase[] { new TestFixtureSetUpTestCase(fixture.Name, method, fixtureObject) };
        }

        class TestFixtureSetUpTestCase : MethodTestCase
        {
            object fixtureObject;

            public TestFixtureSetUpTestCase(string fixtureName, MethodInfo method, object fixtureObject)
                : base(fixtureName, method)
            {
                this.fixtureObject = fixtureObject;
            }

            public override void Run(object fixture)
            {
                base.Run(this.fixtureObject);
            }
        }
    }
}
