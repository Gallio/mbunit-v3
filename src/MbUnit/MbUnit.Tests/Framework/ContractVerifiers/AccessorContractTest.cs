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
    [RunSample(typeof(NoGetterSpecifiedSampleTest))]
    [RunSample(typeof(NoSetterSpecifiedSampleTest))]
    [RunSample(typeof(SetterGetterAndNameSpecifiedSampleTest))]
    [RunSample(typeof(NoValidValuesSampleTest))]
    [RunSample(typeof(AutoValueTypePropertySampleTest))]
    [RunSample(typeof(AutoNullableTypePropertySampleTest))]
    [RunSample(typeof(NonNullableTypePropertySampleTest))]
    [RunSample(typeof(AutoBuggyNonNullableTypePropertySampleTest))]
    [RunSample(typeof(BuggyNullableTypePropertySampleTest))]
    [RunSample(typeof(NoAssignmentPropertySampleTest))]
    [RunSample(typeof(FailingDefaultInstanceSampleTest))]
    [RunSample(typeof(ComplexReferenceTypePropertySampleTest))]
    [RunSample(typeof(ComplexReferenceTypePropertyByNameSampleTest))]
    public class AccessorContractTest : AbstractContractTest
    {
        [Test]
        [Row(typeof(NoGetterSpecifiedSampleTest), "SetValidValues", TestStatus.Failed)]
        [Row(typeof(NoGetterSpecifiedSampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(NoGetterSpecifiedSampleTest), "SetNullValue", TestStatus.Inconclusive)]
        [Row(typeof(NoSetterSpecifiedSampleTest), "SetValidValues", TestStatus.Failed)]
        [Row(typeof(NoSetterSpecifiedSampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(NoSetterSpecifiedSampleTest), "SetNullValue", TestStatus.Inconclusive)]
        [Row(typeof(SetterGetterAndNameSpecifiedSampleTest), "SetValidValues", TestStatus.Failed)]
        [Row(typeof(SetterGetterAndNameSpecifiedSampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(SetterGetterAndNameSpecifiedSampleTest), "SetNullValue", TestStatus.Failed)]
        [Row(typeof(NoValidValuesSampleTest), "SetValidValues", TestStatus.Failed)]
        [Row(typeof(NoValidValuesSampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(NoValidValuesSampleTest), "SetNullValue", TestStatus.Inconclusive)]
        [Row(typeof(AutoValueTypePropertySampleTest), "SetValidValues", TestStatus.Passed)]
        [Row(typeof(AutoValueTypePropertySampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(AutoValueTypePropertySampleTest), "SetNullValue", TestStatus.Inconclusive)]
        [Row(typeof(AutoNullableTypePropertySampleTest), "SetValidValues", TestStatus.Passed)]
        [Row(typeof(AutoNullableTypePropertySampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(AutoNullableTypePropertySampleTest), "SetNullValue", TestStatus.Passed)]
        [Row(typeof(NonNullableTypePropertySampleTest), "SetValidValues", TestStatus.Passed)]
        [Row(typeof(NonNullableTypePropertySampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(NonNullableTypePropertySampleTest), "SetNullValue", TestStatus.Passed)]
        [Row(typeof(AutoBuggyNonNullableTypePropertySampleTest), "SetValidValues", TestStatus.Passed)]
        [Row(typeof(AutoBuggyNonNullableTypePropertySampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(AutoBuggyNonNullableTypePropertySampleTest), "SetNullValue", TestStatus.Failed)]
        [Row(typeof(BuggyNullableTypePropertySampleTest), "SetValidValues", TestStatus.Passed)]
        [Row(typeof(BuggyNullableTypePropertySampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(BuggyNullableTypePropertySampleTest), "SetNullValue", TestStatus.Failed)]
        [Row(typeof(NoAssignmentPropertySampleTest), "SetValidValues", TestStatus.Failed)]
        [Row(typeof(NoAssignmentPropertySampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(NoAssignmentPropertySampleTest), "SetNullValue", TestStatus.Inconclusive)]
        [Row(typeof(FailingDefaultInstanceSampleTest), "SetValidValues", TestStatus.Failed)]
        [Row(typeof(FailingDefaultInstanceSampleTest), "SetIncompetentValues", TestStatus.Inconclusive)]
        [Row(typeof(FailingDefaultInstanceSampleTest), "SetNullValue", TestStatus.Failed)]
        [Row(typeof(ComplexReferenceTypePropertySampleTest), "SetValidValues", TestStatus.Passed)]
        [Row(typeof(ComplexReferenceTypePropertySampleTest), "SetIncompetentValues", TestStatus.Passed)]
        [Row(typeof(ComplexReferenceTypePropertySampleTest), "SetNullValue", TestStatus.Passed)]
        [Row(typeof(ComplexReferenceTypePropertyByNameSampleTest), "SetValidValues", TestStatus.Passed)]
        [Row(typeof(ComplexReferenceTypePropertyByNameSampleTest), "SetIncompetentValues", TestStatus.Passed)]
        [Row(typeof(ComplexReferenceTypePropertyByNameSampleTest), "SetNullValue", TestStatus.Passed)]
        public void VerifySampleAccessorContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("AccessorTests", fixtureType, testMethodName, expectedTestStatus);
        }

        [Explicit]
        internal class NoGetterSpecifiedSampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, int>
            {
                Setter = (target, value) => target.Int32Value = value,
            };
        }

        [Explicit]
        internal class NoSetterSpecifiedSampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, int>
            {
                Getter = target => target.Int32Value,
            };
        }

        [Explicit]
        internal class SetterGetterAndNameSpecifiedSampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, string>
            {
                Getter = target => target.NullableStringValue,
                Setter = (target, value) => target.NullableStringValue = value,
                PropertyName = "NullableStringValue"
            };
        }

        [Explicit]
        internal class NoValidValuesSampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, int>
            {
                Getter = target => target.Int32Value,
                Setter = (target, value) => target.Int32Value = value,
            };
        }

        [Explicit]
        internal class AutoValueTypePropertySampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, int>
            {
                Getter = target => target.Int32Value,
                Setter = (target, value) => target.Int32Value = value,
                ValidValues = { 123, 456, 789 }
            };
        }

        [Explicit]
        internal class AutoNullableTypePropertySampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, string>
            {
                Getter = target => target.NullableStringValue,
                Setter = (target, value) => target.NullableStringValue = value,
                ValidValues = { "Alpha", "Beta", "Gamma" },
                AcceptNullValue = true
            };
        }

        [Explicit]
        internal class NonNullableTypePropertySampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, string>
            {
                Getter = target => target.NonNullableStringValue,
                Setter = (target, value) => target.NonNullableStringValue = value,
                ValidValues = { "Alpha", "Beta", "Gamma" },
                AcceptNullValue = false
            };
        }

        [Explicit]
        internal class AutoBuggyNonNullableTypePropertySampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, string>
            {
                Getter = target => target.NullableStringValue,
                Setter = (target, value) => target.NullableStringValue = value,
                ValidValues = { "Alpha", "Beta", "Gamma" },
                AcceptNullValue = false
            };
        }

        [Explicit]
        internal class BuggyNullableTypePropertySampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, string>
            {
                Getter = target => target.NonNullableStringValue,
                Setter = (target, value) => target.NonNullableStringValue = value,
                ValidValues = { "Alpha", "Beta", "Gamma" },
                AcceptNullValue = true
            };
        }

        [Explicit]
        internal class NoAssignmentPropertySampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<Sample, int>
            {
                Getter = target => target.NoAssignmentInt32Value,
                Setter = (target, value) => target.NoAssignmentInt32Value = value,
                ValidValues = { 123, 456, 789 }
            };
        }

        [Explicit]
        internal class FailingDefaultInstanceSampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<SampleWithNoDefaultConstructor, Foo>
            {
                Getter = target => target.Foo,
                Setter = (target, value) => target.Foo = value,
                ValidValues = { new Foo(123), new Foo(456), new Foo(789) },
                AcceptNullValue = false
            };
        }

        [Explicit]
        internal class ComplexReferenceTypePropertySampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<SampleWithNoDefaultConstructor, Foo>
            {
                Getter = target => target.Foo,
                Setter = (target, value) => target.Foo = value,
                ValidValues = { new Foo(123), new Foo(456), new Foo(789) },
                AcceptNullValue = false,
                DefaultInstance = () => new SampleWithNoDefaultConstructor("Hello"),
                IncompetentValues =
                {
                    { typeof(ArgumentOutOfRangeException), new Foo(-123), new Foo(-456) },
                    { typeof(ArgumentException), new Foo(666) }
                }
            };
        }

        [Explicit]
        internal class ComplexReferenceTypePropertyByNameSampleTest
        {
            [VerifyContract]
            public readonly IContract AccessorTests = new AccessorContract<SampleWithNoDefaultConstructor, Foo>
            {
                PropertyName = "Foo",
                ValidValues = { new Foo(123), new Foo(456), new Foo(789) },
                AcceptNullValue = false,
                DefaultInstance = () => new SampleWithNoDefaultConstructor("Hello"),
                IncompetentValues =
                {
                    { typeof(ArgumentOutOfRangeException), new Foo(-123), new Foo(-456) },
                    { typeof(ArgumentException), new Foo(666) }
                }
            };
        }

        internal class Sample
        {
            public int Int32Value
            {
                get;
                set;
            }

            public string NullableStringValue
            {
                get;
                set;
            }

            private string nonNullableStringValue;

            public string NonNullableStringValue
            {
                get
                {
                    return nonNullableStringValue;
                }

                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }

                    nonNullableStringValue = value;
                }
            }

            public int NoAssignmentInt32Value
            {
                get
                {
                    return 0;
                }

                set
                {
                }
            }
        }

        internal class Foo
        {
            private readonly int value;

            public int Value
            {
                get
                {
                    return value;
                }
            }

            public Foo (int value)
	        {
                this.value = value;
	        }
        }

        internal class SampleWithNoDefaultConstructor
        {
            private Foo foo;

            public SampleWithNoDefaultConstructor(string unusedArgument)
            {
            }

            public Foo Foo
            {
                get
                {
                    return foo;
                }

                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }

                    if (value.Value < 0)
                    {
                        throw new ArgumentOutOfRangeException();
                    }

                    if (value.Value == 666)
                    {
                        throw new ArgumentException("Inferno value strictly forbidden.");
                    }

                    this.foo = value;
                }
            }
        }

    }
}
