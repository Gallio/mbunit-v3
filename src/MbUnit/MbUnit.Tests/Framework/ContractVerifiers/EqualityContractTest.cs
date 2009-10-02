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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Gallio.Tests;
using Gallio.Model;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [RunSample(typeof(FullContractOnEquatableSample))]
    [RunSample(typeof(PartialContractOnEquatableSample))]
    [RunSample(typeof(StaticPartialContractOnEquatableSample))]
    [RunSample(typeof(FullContractOnInterfaceSample))]
    public class EqualityContractTest : AbstractContractTest
    {
        [Test]
        [Row(typeof(FullContractOnEquatableSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnEquatableSample), "HashCode", TestStatus.Passed)]
        [Row(typeof(FullContractOnEquatableSample), "EquatableEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnEquatableSample), "OperatorEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnEquatableSample), "OperatorNotEquals", TestStatus.Passed)]
        [Row(typeof(PartialContractOnEquatableSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(PartialContractOnEquatableSample), "HashCode", TestStatus.Passed)]
        [Row(typeof(PartialContractOnEquatableSample), "EquatableEquals", TestStatus.Passed)]
        [Row(typeof(PartialContractOnEquatableSample), "OperatorEquals", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnEquatableSample), "OperatorNotEquals", TestStatus.Inconclusive)]
        [Row(typeof(StaticPartialContractOnEquatableSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(StaticPartialContractOnEquatableSample), "HashCode", TestStatus.Passed)]
        [Row(typeof(StaticPartialContractOnEquatableSample), "EquatableEquals", TestStatus.Passed)]
        [Row(typeof(StaticPartialContractOnEquatableSample), "OperatorEquals", TestStatus.Inconclusive)]
        [Row(typeof(StaticPartialContractOnEquatableSample), "OperatorNotEquals", TestStatus.Inconclusive)]
        [Row(typeof(FullContractOnInterfaceSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnInterfaceSample), "HashCode", TestStatus.Passed)]
        [Row(typeof(FullContractOnInterfaceSample), "EquatableEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnInterfaceSample), "OperatorEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnInterfaceSample), "OperatorNotEquals", TestStatus.Passed)]
        public void VerifySampleEqualityContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("EqualityTests", fixtureType, testMethodName, expectedTestStatus);
        }

        [Explicit]
        private class FullContractOnEquatableSample
        {
            [VerifyContract]
            public readonly IContract EqualityTests = new EqualityContract<SampleEquatable>
            {
                ImplementsOperatorOverloads = true,
                EquivalenceClasses =
                {
                    { new SampleEquatable(123) },
                    { new SampleEquatable(456) },
                    { new SampleEquatable(789) }
                }                
            };
        }

        [Explicit]
        private class PartialContractOnEquatableSample
        {
            [VerifyContract]
            public readonly IContract EqualityTests = new EqualityContract<SampleEquatable>
            {
                ImplementsOperatorOverloads = false,
                EquivalenceClasses =
                {
                    { new SampleEquatable(123) },
                    { new SampleEquatable(456) },
                    { new SampleEquatable(789) }
                }
            };
        }

        [Explicit]
        private class StaticPartialContractOnEquatableSample
        {
            [VerifyContract]
            public readonly static IContract EqualityTests = new EqualityContract<SampleEquatable>
            {
                ImplementsOperatorOverloads = false,
                EquivalenceClasses =
                {
                    { new SampleEquatable(123) },
                    { new SampleEquatable(456) },
                    { new SampleEquatable(789) }
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
                    { new SampleEquatable(123) },
                    { new SampleEquatable(456) },
                    { new SampleEquatable(789) }
                }
            };
        }

        private interface ISampleEquatable : IEquatable<ISampleEquatable>
        {
        }

        /// <summary>
        /// Sample equatable type.
        /// </summary>
        private class SampleEquatable : ISampleEquatable, IEquatable<SampleEquatable>
        {
            private int value;

            public SampleEquatable(int value)
            {
                this.value = value;
            }

            public override int GetHashCode()
            {
                return value.GetHashCode();
            }

            public override bool Equals(object other)
            {
                return Equals(other as SampleEquatable);
            }

            public bool Equals(SampleEquatable other)
            {
                return (other != null) && (value == other.value);
            }

            bool IEquatable<ISampleEquatable>.Equals(ISampleEquatable other)
            {
                return Equals(other as SampleEquatable);
            }

            public static bool operator ==(SampleEquatable left, SampleEquatable right)
            {
                return
                    (((object)left == null) && ((object)right == null)) ||
                    (((object)left != null) && left.Equals(right));
            }

            public static bool operator !=(SampleEquatable left, SampleEquatable right)
            {
                return !(left == right);
            }
        }

    }
}
