using System;

using System.Reflection;
using System.Collections;

using TestDriven.UnitTesting;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Row : TestAttributeBase
    {
        object[] args;

        public Row(params object[] args)
        {
            this.args = args;
        }
        
        public override ITestCase[] CreateTests(ITestFixture fixture, MethodInfo method)
        {
            return new ITestCase[] { new MethodTestCase(fixture.Name, method, this.args) };
        }
    }
}
