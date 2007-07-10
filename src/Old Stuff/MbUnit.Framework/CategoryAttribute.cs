namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using TestDriven.UnitTesting;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CategoryAttribute : TestDecoratorAttributeBase
    {  
        string name;

        public CategoryAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
        }

        public override ITestCase Decorate(ITestCase testCase)
        {
            testCase.Categories.Add(this.name);
            return testCase;
        }
    }
}
