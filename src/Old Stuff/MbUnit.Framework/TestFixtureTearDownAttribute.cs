namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using System.Collections;
    using TestDriven.UnitTesting;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TestFixtureTearDownAttribute : Attribute, ITestCaseFactory
    {
        public ITestCase[] CreateTests(ITestFixture fixture, MethodInfo method)
        {
            ArrayList tests = new ArrayList();
            object fixtureObject = fixture.CreateInstance();
            ITestCase testFixtureTearDown = new TestFixtureTearDownTestCase(fixture.Name, method, fixtureObject);
            tests.Add(testFixtureTearDown);
            return (ITestCase[])tests.ToArray(typeof(ITestCase));
        }

        class TestFixtureTearDownTestCase : MethodTestCase
        {
            object fixtureObject;

            public TestFixtureTearDownTestCase(string fixtureName, MethodInfo method, object fixtureObject)
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
