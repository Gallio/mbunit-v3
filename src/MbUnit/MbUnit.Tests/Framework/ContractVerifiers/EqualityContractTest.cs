// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Gallio.Tests;
using Gallio.Model;

using System.Reflection;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [RunSample(typeof(FullContractOnSimpleSample))]
    [RunSample(typeof(PartialContractOnSimpleSample))]
    [RunSample(typeof(StaticPartialContractOnSimpleSample))]
    [RunSample(typeof(FullContractOnInterfaceSample))]
    [RunSample(typeof(FullContractOnSampleWithEqualityInheritance))]
    public class EqualityContractTest : AbstractContractTest
    {
        [Test]
        [Row(typeof(FullContractOnSimpleSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnSimpleSample), "HashCode", TestStatus.Passed)]
        [Row(typeof(FullContractOnSimpleSample), "EquatableEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnSimpleSample), "OperatorEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnSimpleSample), "OperatorNotEquals", TestStatus.Passed)]
        [Row(typeof(PartialContractOnSimpleSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(PartialContractOnSimpleSample), "HashCode", TestStatus.Passed)]
        [Row(typeof(PartialContractOnSimpleSample), "EquatableEquals", TestStatus.Passed)]
        [Row(typeof(PartialContractOnSimpleSample), "OperatorEquals", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnSimpleSample), "OperatorNotEquals", TestStatus.Inconclusive)]
        [Row(typeof(StaticPartialContractOnSimpleSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(StaticPartialContractOnSimpleSample), "HashCode", TestStatus.Passed)]
        [Row(typeof(StaticPartialContractOnSimpleSample), "EquatableEquals", TestStatus.Passed)]
        [Row(typeof(StaticPartialContractOnSimpleSample), "OperatorEquals", TestStatus.Inconclusive)]
        [Row(typeof(StaticPartialContractOnSimpleSample), "OperatorNotEquals", TestStatus.Inconclusive)]
        [Row(typeof(FullContractOnInterfaceSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnInterfaceSample), "HashCode", TestStatus.Passed)]
        [Row(typeof(FullContractOnInterfaceSample), "EquatableEquals_ISampleEquatable", TestStatus.Passed)]
        [Row(typeof(FullContractOnInterfaceSample), "EquatableEquals_SampleParentEquatable", TestStatus.Passed)]
        [Row(typeof(FullContractOnInterfaceSample), "OperatorEquals", TestStatus.Failed)]
        [Row(typeof(FullContractOnInterfaceSample), "OperatorNotEquals", TestStatus.Failed)]
        [Row(typeof(FullContractOnSampleWithEqualityInheritance), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleWithEqualityInheritance), "HashCode", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleWithEqualityInheritance), "EquatableEquals_ISampleEquatable", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleWithEqualityInheritance), "EquatableEquals_SampleParentEquatable", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleWithEqualityInheritance), "EquatableEquals_SampleChildEquatable", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleWithEqualityInheritance), "OperatorEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleWithEqualityInheritance), "OperatorNotEquals", TestStatus.Passed)]
        public void VerifySampleEqualityContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("EqualityTests", fixtureType, testMethodName, expectedTestStatus);
        }

        [Explicit]
        private class FullContractOnSimpleSample
        {
            [VerifyContract]
            public readonly IContract EqualityTests = new EqualityContract<SampleSimpleEquatable>
            {
                ImplementsOperatorOverloads = true,
                EquivalenceClasses =
                {
                    { new SampleSimpleEquatable(123) },
                    { new SampleSimpleEquatable(456) },
                    { new SampleSimpleEquatable(789) }
                }                
            };
        }

        [Explicit]
        private class PartialContractOnSimpleSample
        {
            [VerifyContract]
            public readonly IContract EqualityTests = new EqualityContract<SampleSimpleEquatable>
            {
                ImplementsOperatorOverloads = false,
                EquivalenceClasses =
                {
                    { new SampleSimpleEquatable(123) },
                    { new SampleSimpleEquatable(456) },
                    { new SampleSimpleEquatable(789) }
                }
            };
        }

        [Explicit]
        private class StaticPartialContractOnSimpleSample
        {
            [VerifyContract]
            public readonly static IContract EqualityTests = new EqualityContract<SampleSimpleEquatable>
            {
                ImplementsOperatorOverloads = false,
                EquivalenceClasses =
                {
                    { new SampleSimpleEquatable(123) },
                    { new SampleSimpleEquatable(456) },
                    { new SampleSimpleEquatable(789) }
                }
            };
        }

        [Explicit]
        private class FullContractOnInterfaceSample
        {
            [VerifyContract]
            public readonly static IContract EqualityTests = new EqualityContract<ISampleEquatable>
            {
                ImplementsOperatorOverloads = true,
                EquivalenceClasses =
                {
                    { new SampleParentEquatable(123) },
                    { new SampleParentEquatable(456) },
                    { new SampleParentEquatable(789) }
                }
            };
        }

        [Explicit]
        private class FullContractOnSampleWithEqualityInheritance
        {
            [VerifyContract]
            public readonly static IContract EqualityTests = new EqualityContract<SampleParentEquatable>
            {
                ImplementsOperatorOverloads = true,
                EquivalenceClasses =
                {
                    { new SampleParentEquatable(123), new SampleChildEquatable(123) },
                    { new SampleChildEquatable(456) },
                    { new SampleParentEquatable(789) }
                }
            };
        }

        private class SampleSimpleEquatable : IEquatable<SampleSimpleEquatable>
        {
            private int value;

            public SampleSimpleEquatable(int value)
            {
                this.value = value;
            }

            public override int GetHashCode()
            {
                return value.GetHashCode();
            }

            public override bool Equals(object other)
            {
                return Equals(other as SampleSimpleEquatable);
            }

            public bool Equals(SampleSimpleEquatable other)
            {
                return (other != null) && (value == other.value);
            }

            public static bool operator ==(SampleSimpleEquatable left, SampleSimpleEquatable right)
            {
                return (ReferenceEquals(null, left) && ReferenceEquals(null, right))
                    || (!ReferenceEquals(null, left) && left.Equals(right));
            }

            public static bool operator !=(SampleSimpleEquatable left, SampleSimpleEquatable right)
            {
                return !(left == right);
            }
        }

        private interface ISampleEquatable : IEquatable<ISampleEquatable>
        {
        }

        private class SampleParentEquatable : ISampleEquatable, IEquatable<SampleParentEquatable>
        {
            private int value;

            public SampleParentEquatable(int value)
            {
                this.value = value;
            }

            public override int GetHashCode()
            {
                return value.GetHashCode();
            }

            public override bool Equals(object other)
            {
                return Equals(other as SampleParentEquatable);
            }

            public bool Equals(SampleParentEquatable other)
            {
                return (other != null) && (value == other.value);
            }

            bool IEquatable<ISampleEquatable>.Equals(ISampleEquatable other)
            {
                return Equals(other as SampleParentEquatable);
            }

            public static bool operator ==(SampleParentEquatable left, SampleParentEquatable right)
            {
                return (ReferenceEquals(null, left) && ReferenceEquals(null, right)) 
                    || (!ReferenceEquals(null, left) && left.Equals(right));
            }

            public static bool operator !=(SampleParentEquatable left, SampleParentEquatable right)
            {
                return !(left == right);
            }
        }

        private class SampleChildEquatable : SampleParentEquatable, IEquatable<SampleChildEquatable>
        {
            public SampleChildEquatable(int value)
                : base(value)
            {
            }

            public bool Equals(SampleChildEquatable other)
            {
                return base.Equals(other);
            }
        }
    }
}
