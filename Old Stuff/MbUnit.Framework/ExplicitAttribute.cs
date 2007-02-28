namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using TestDriven.UnitTesting.Exceptions;
    using TestDriven.UnitTesting;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ExplicitAttribute : TestDecoratorAttributeBase
    {
        public ExplicitAttribute()
        {
        }

        public override ITestCase Decorate(ITestCase testCase)
        {
            return new ExplicitDecorator(testCase);
        }

        class ExplicitDecorator : DecoratorTestCaseBase
        {
            public ExplicitDecorator(ITestCase testCase)
                : base(testCase)
            {
            }

            public override bool IsExplicit
            {
                get { return true; }
            }

            public override void Run(object fixtureInstance)
            {
                this.TestCase.Run(fixtureInstance);
            }
        }
    }
}
