// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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

using System;
using System.Linq;
using System.Runtime.Serialization;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Tests;
using Gallio.Tests.Integration;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [RunSample(typeof(SampleWith1ArgumentConstructorTest))]
    [RunSample(typeof(SampleWith1ArgumentMethodTest))]
    [RunSample(typeof(SampleWith1ArgumentMethodAndCustomConstructorTest))]
    [RunSample(typeof(SampleWith2ArgumentsConstructorTest))]
    [RunSample(typeof(SampleWith2ArgumentsMethodTest))]
    [RunSample(typeof(SampleWith2ArgumentsMethodAndCustomConstructorTest))]
    public class ArgumentValidationContractTest : AbstractContractTest
    {
        [Test]
        [Row(typeof(SampleWith1ArgumentConstructorTest), "Test1", TestStatus.Passed)]
        [Row(typeof(SampleWith1ArgumentConstructorTest), "Test2", TestStatus.Passed)]
        [Row(typeof(SampleWith1ArgumentMethodTest), "Test1", TestStatus.Passed)]
        [Row(typeof(SampleWith1ArgumentMethodTest), "Test2", TestStatus.Passed)]
        [Row(typeof(SampleWith1ArgumentMethodAndCustomConstructorTest), "Test1", TestStatus.Passed)]
        [Row(typeof(SampleWith1ArgumentMethodAndCustomConstructorTest), "Test2", TestStatus.Passed)]
        [Row(typeof(SampleWith2ArgumentsConstructorTest), "Test1", TestStatus.Passed)]
        [Row(typeof(SampleWith2ArgumentsConstructorTest), "Test2", TestStatus.Passed)]
        [Row(typeof(SampleWith2ArgumentsConstructorTest), "Test3", TestStatus.Passed)]
        [Row(typeof(SampleWith2ArgumentsMethodTest), "Test1", TestStatus.Passed)]
        [Row(typeof(SampleWith2ArgumentsMethodTest), "Test2", TestStatus.Passed)]
        [Row(typeof(SampleWith2ArgumentsMethodTest), "Test3", TestStatus.Passed)]
        [Row(typeof(SampleWith2ArgumentsMethodAndCustomConstructorTest), "Test1", TestStatus.Passed)]
        [Row(typeof(SampleWith2ArgumentsMethodAndCustomConstructorTest), "Test2", TestStatus.Passed)]
        [Row(typeof(SampleWith2ArgumentsMethodAndCustomConstructorTest), "Test3", TestStatus.Passed)]
        public void VerifyArgumentValidationContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("ArgumentValidationTests", fixtureType, testMethodName, expectedTestStatus);
        }

        [Explicit]
        internal class SampleWith1ArgumentConstructorTest
        {
            [VerifyContract]
            public readonly IContract ArgumentValidationTests = ArgumentValidationContract<SampleWith1Argument>
                .For<int>(x => new SampleWith1Argument(x))
                .ShouldThrow<ArgumentOutOfRangeException>(0)
                .ShouldThrow<ArgumentOutOfRangeException>(-1);

            internal class SampleWith1Argument
            {
                internal SampleWith1Argument(int number)
                {
                    if (number <= 0)
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Explicit]
        internal class SampleWith1ArgumentMethodTest
        {
            [VerifyContract]
            public readonly IContract ArgumentValidationTests = ArgumentValidationContract<SampleWith1ArgumentMethod>
                .For<int>((o, x) => o.Method(x))
                .ShouldThrow<ArgumentOutOfRangeException>(0)
                .ShouldThrow<ArgumentOutOfRangeException>(-1);

            internal class SampleWith1ArgumentMethod
            {
                internal void Method(int number)
                {
                    if (number <= 0)
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Explicit]
        internal class SampleWith1ArgumentMethodAndCustomConstructorTest
        {
            [VerifyContract]
            public readonly IContract ArgumentValidationTests = ArgumentValidationContract<Sample>
                .For<int>((o, x) => o.Method(x))
                .With(() => new Sample(Math.PI))
                .ShouldThrow<ArgumentOutOfRangeException>(0)
                .ShouldThrow<ArgumentOutOfRangeException>(-1);

            internal class Sample
            {
                internal Sample(double dumb)
                {
                }

                internal void Method(int number)
                {
                    if (number <= 0)
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Explicit]
        internal class SampleWith2ArgumentsConstructorTest
        {
            [VerifyContract]
            public readonly IContract ArgumentValidationTests = ArgumentValidationContract<Sample>
                .For<int, string>((x, y) => new Sample(x, y))
                .ShouldThrow<ArgumentOutOfRangeException>(0, "Hello")
                .ShouldThrow<ArgumentOutOfRangeException>(-1, "Hello")
                .ShouldThrow<ArgumentNullException>(123, null);

            internal class Sample
            {
                internal Sample(int number, string text)
                {
                    if (text == null)
                        throw new ArgumentNullException("text");
                    if (number <= 0)
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Explicit]
        internal class SampleWith2ArgumentsMethodTest
        {
            [VerifyContract]
            public readonly IContract ArgumentValidationTests = ArgumentValidationContract<Sample>
                .For<int, string>((o, x, y) => o.Method(x, y))
                .ShouldThrow<ArgumentOutOfRangeException>(0, "Hello")
                .ShouldThrow<ArgumentOutOfRangeException>(-1, "Hello")
                .ShouldThrow<ArgumentNullException>(123, null);

            internal class Sample
            {
                internal void Method(int number, string text)
                {
                    if (text == null)
                        throw new ArgumentNullException("text");
                    if (number <= 0)
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Explicit]
        internal class SampleWith2ArgumentsMethodAndCustomConstructorTest
        {
            [VerifyContract]
            public readonly IContract ArgumentValidationTests = ArgumentValidationContract<Sample>
                .For<int, string>((o, x, y) => o.Method(x, y))
                .With(() => new Sample(Math.PI))
                .ShouldThrow<ArgumentOutOfRangeException>(0, "Hello")
                .ShouldThrow<ArgumentOutOfRangeException>(-1, "Hello")
                .ShouldThrow<ArgumentNullException>(123, null);

            internal class Sample
            {
                internal Sample(double dumb)
                {
                }

                internal void Method(int number, string text)
                {
                    if (text == null)
                        throw new ArgumentNullException("text");
                    if (number <= 0)
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
