namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using System.Collections;
    using TestDriven.UnitTesting;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TestAttribute : TestAttributeBase
    {
        public override ITestCase[] CreateTests(ITestFixture fixture, MethodInfo method)
        {
            if (!method.IsPublic)
            {
                return new ITestCase[] { new IgnoreTestCase(fixture.Name, method.Name, "Test methods must be public.") };
            }

            if (method.GetParameters().Length > 0)
            {
                return new ITestCase[] { new IgnoreTestCase(fixture.Name, method.Name, "Test methods mustn't take parameters.") };
            }

            if (method.ReturnType != typeof(void))
            {
                return new ITestCase[] { new IgnoreTestCase(fixture.Name, method.Name, "Test methods must return void.") };
            }

            return new ITestCase[] { new MethodTestCase(fixture.Name, method) };
        }
    }
}
