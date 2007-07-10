namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using System.Collections;
    using TestDriven.UnitTesting;

    using TestDriven.UnitTesting.Exceptions;

    class SetUpTearDownTestCaseDecorator : ITestCaseDecorator
    {
        MethodInfo setUpMethod;
        MethodInfo tearDownMethod;

        public SetUpTearDownTestCaseDecorator(MethodInfo setUpMethod, MethodInfo tearDownMethod)
        {
            this.setUpMethod = setUpMethod;
            this.tearDownMethod = tearDownMethod;
        }

        public ITestCase Decorate(ITestCase testCase)
        {
            return new SetUpTearDownDecoratorTestCase(testCase, this.setUpMethod, this.tearDownMethod);
        }

        class SetUpTearDownDecoratorTestCase : DecoratorTestCaseBase
        {
            MethodInfo setUpMethod;
            MethodInfo tearDownMethod;

            public SetUpTearDownDecoratorTestCase(ITestCase testCase,
                MethodInfo setUpMethod, MethodInfo tearDownMethod)
                : base(testCase)
            {
                this.setUpMethod = setUpMethod;
                this.tearDownMethod = tearDownMethod;
            }

            public override void Run(object fixtureInstance)
            {
                try
                {
                    if (this.setUpMethod != null)
                    {
                        try
                        {
                            this.setUpMethod.Invoke(fixtureInstance, null);
                        }
                        catch (TargetInvocationException e)
                        {
                            throw new UnitTestingException(e.InnerException);
                        }
                    }

                    this.TestCase.Run(fixtureInstance);
                }
                finally
                {
                    if (this.tearDownMethod != null)
                    {
                        try
                        {
                            this.tearDownMethod.Invoke(fixtureInstance, null);
                        }
                        catch (TargetInvocationException e)
                        {
                            throw new UnitTestingException(e.InnerException);
                        }
                    }
                }
            }
        }
    }
}
