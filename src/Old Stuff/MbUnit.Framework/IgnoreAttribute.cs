namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using TestDriven.UnitTesting.Exceptions;
    using TestDriven.UnitTesting;

    public class IgnoreAttribute : TestDecoratorAttributeBase
    {
        string reason;

        public IgnoreAttribute()
        {
        }

        public IgnoreAttribute(string reason)
        {
            this.reason = reason;
        }

        public string Reason
        {
            get { return this.reason; }
        }

        public override ITestCase Decorate(ITestCase testCase)
        {
            return new IgnoreTestCase(testCase.FixtureName, testCase.Name, this.reason);
        }
    }
}
