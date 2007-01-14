
using System;
using MbUnit.Framework;
using TestDriven.UnitTesting.Exceptions;
using TestDriven.UnitTesting;

namespace MbUnit.Framework
{
	
	/// <summary>
	/// Tags method that should throw an exception.
	/// </summary>
	/// <include file="MbUnit.Framework.Doc.xml" path="doc/remarkss/remarks[@name='ExpectedExceptionAttribute']"/>	
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ExpectedExceptionAttribute : TestDecoratorAttributeBase
    {
		Type exceptionType;
		
		public ExpectedExceptionAttribute(Type exceptionType)
		{
			if (exceptionType==null)
				throw new ArgumentNullException("exceptionType");
			this.exceptionType = exceptionType;
		}
	
		public Type ExceptionType
		{
			get
			{
				return this.exceptionType;
			}
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
                try
                {

                }
                catch { }
            }
        }

        //public override IRunInvoker GetInvoker(IRunInvoker invoker)
        //{
        //    return new ExpectedExceptionRunInvoker(invoker, this.ExceptionType, this.Description);
        //}
	}
}
