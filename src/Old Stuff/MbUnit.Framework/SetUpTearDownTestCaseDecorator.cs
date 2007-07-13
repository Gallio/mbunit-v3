// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
