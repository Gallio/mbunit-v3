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
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Tests;
using Gallio.Tests.Integration;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    [TestsOn(typeof(ComparisonContract<>))]
    [RunSample(typeof(FullContractOnComparableSample))]
    [RunSample(typeof(PartialContractOnComparableSample))]
    [RunSample(typeof(PartialContractOnInterfaceComparableSample))]
    [RunSample(typeof(PartialContractInheritedComparableSample))]
    [RunSample(typeof(PartialContractUnrelatedComparableSample))]
    public class ComparisonContractTest : AbstractContractTest
    {
        [Test]
        [Row(typeof(FullContractOnComparableSample), "ComparableCompareTo", TestStatus.Passed)]
        [Row(typeof(FullContractOnComparableSample), "OperatorGreaterThan", TestStatus.Passed)]
        [Row(typeof(FullContractOnComparableSample), "OperatorGreaterThanOrEqual", TestStatus.Passed)]
        [Row(typeof(FullContractOnComparableSample), "OperatorLessThan", TestStatus.Passed)]
        [Row(typeof(FullContractOnComparableSample), "OperatorLessThanOrEqual", TestStatus.Passed)]
        [Row(typeof(PartialContractOnComparableSample), "ComparableCompareTo", TestStatus.Passed)]
        [Row(typeof(PartialContractOnComparableSample), "OperatorGreaterThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnComparableSample), "OperatorGreaterThanOrEqual", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnComparableSample), "OperatorLessThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnComparableSample), "OperatorLessThanOrEqual", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnInterfaceComparableSample), "ComparableCompareTo_ISampleComparable", TestStatus.Passed)]
        [Row(typeof(PartialContractOnInterfaceComparableSample), "ComparableCompareTo_SampleParentComparable", TestStatus.Passed)]
        [Row(typeof(PartialContractOnInterfaceComparableSample), "OperatorGreaterThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnInterfaceComparableSample), "OperatorGreaterThanOrEqual", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnInterfaceComparableSample), "OperatorLessThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnInterfaceComparableSample), "OperatorLessThanOrEqual", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractInheritedComparableSample), "ComparableCompareTo_ISampleComparable", TestStatus.Passed)]
        [Row(typeof(PartialContractInheritedComparableSample), "ComparableCompareTo_SampleParentComparable", TestStatus.Passed)]
        [Row(typeof(PartialContractInheritedComparableSample), "ComparableCompareTo_SampleChildComparable", TestStatus.Passed)]
        [Row(typeof(PartialContractInheritedComparableSample), "OperatorGreaterThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractInheritedComparableSample), "OperatorGreaterThanOrEqual", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractInheritedComparableSample), "OperatorLessThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractInheritedComparableSample), "OperatorLessThanOrEqual", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractUnrelatedComparableSample), "ComparableCompareTo_SampleUnrelatedComparable", TestStatus.Passed)]
        [Row(typeof(PartialContractUnrelatedComparableSample), "ComparableCompareTo_Int32", TestStatus.Passed)]
        [Row(typeof(PartialContractUnrelatedComparableSample), "ComparableCompareTo_String", TestStatus.Passed)]
        [Row(typeof(PartialContractUnrelatedComparableSample), "OperatorGreaterThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractUnrelatedComparableSample), "OperatorGreaterThanOrEqual", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractUnrelatedComparableSample), "OperatorLessThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractUnrelatedComparableSample), "OperatorLessThanOrEqual", TestStatus.Inconclusive)]
        public void VerifySampleEqualityContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("ComparisonTests", fixtureType, testMethodName, expectedTestStatus);
        }

        [TestFixture, Explicit("Sample")]
        private class FullContractOnComparableSample
        {
            [VerifyContract]
            public readonly IContract ComparisonTests = new ComparisonContract<SampleComparable>
            {
                ImplementsOperatorOverloads = true,
                EquivalenceClasses =
                {
                    { new SampleComparable(123), new SampleComparable(123), new SampleComparable(123) },
                    { new SampleComparable(456) },
                    { new SampleComparable(789), new SampleComparable(789) },
                }
            };
        }

        [TestFixture, Explicit("Sample")]
        private class PartialContractOnComparableSample
        {
            [VerifyContract]
            public readonly IContract ComparisonTests = new ComparisonContract<SampleComparable>
            {
                ImplementsOperatorOverloads = false,
                EquivalenceClasses =
                {
                    { new SampleComparable(123), new SampleComparable(123), new SampleComparable(123) },
                    { new SampleComparable(456) },
                    { new SampleComparable(789), new SampleComparable(789) },
                }
            };
        }

        [TestFixture, Explicit("Sample")]
        private class PartialContractOnInterfaceComparableSample
        {
            [VerifyContract]
            public readonly IContract ComparisonTests = new ComparisonContract<ISampleComparable>
            {
                ImplementsOperatorOverloads = false,
                EquivalenceClasses =
                {
                    { new SampleParentComparable(123) },
                    { new SampleParentComparable(456) },
                    { new SampleParentComparable(789) },
                }
            };
        }

        [TestFixture, Explicit("Sample")]
        private class PartialContractInheritedComparableSample
        {
            [VerifyContract]
            public readonly IContract ComparisonTests = new ComparisonContract<SampleParentComparable>
            {
                ImplementsOperatorOverloads = false,
                EquivalenceClasses =
                {
                    { new SampleParentComparable(123), new SampleChildComparable(123) },
                    { new SampleChildComparable(456) },
                    { new SampleParentComparable(789) },
                }
            };
        }

        [TestFixture, Explicit("Sample")]
        private class PartialContractUnrelatedComparableSample
        {
            [VerifyContract]
            public readonly IContract ComparisonTests = new ComparisonContract<SampleUnrelatedComparable>
            {
                ImplementsOperatorOverloads = false,
                EquivalenceClasses =
                {
                    { new SampleUnrelatedComparable(123), 123, "123", " 123 " },
                    { new SampleUnrelatedComparable(456), 456, "456", " 456 " },
                    { new SampleUnrelatedComparable(789), 789, "789", " 789 " },
                }
            };
        }

        #region Samples
        
        private class SampleComparable : IComparable<SampleComparable>
        {
            private int value;

            public int Value
            {
                get { return value; }
            }

            public SampleComparable(int value)
            {
                this.value = value;
            }

            public int CompareTo(SampleComparable other)
            {
                return (other == null) ? Int32.MaxValue : value.CompareTo(other.value);
            }

            public static bool operator >=(SampleComparable left, SampleComparable right)
            {
                return ((left == null) && (right == null)) || ((left != null) && (left.CompareTo(right) >= 0));
            }

            public static bool operator <=(SampleComparable left, SampleComparable right)
            {
                return (left == null) || (left.CompareTo(right) <= 0);
            }

            public static bool operator >(SampleComparable left, SampleComparable right)
            {
                return (left != null) && (left.CompareTo(right) > 0);
            }

            public static bool operator <(SampleComparable left, SampleComparable right)
            {
                return ((left != null) || (right != null)) && ((left == null) || (left.CompareTo(right) < 0));
            }
        }

        private interface ISampleComparable : IComparable<ISampleComparable>
        { 
        }

        private class SampleParentComparable : ISampleComparable, IComparable<SampleParentComparable>
        {
            private int value;

            public int Value
            {
                get { return value; }
            }

            public SampleParentComparable(int value)
            {
                this.value = value;
            }

            public int CompareTo(SampleParentComparable other)
            {
                return (other == null) ? Int32.MaxValue : value.CompareTo(other.Value);
            }

            int IComparable<ISampleComparable>.CompareTo(ISampleComparable other)
            {
                return CompareTo(other as SampleParentComparable);
            }
        }

        private class SampleChildComparable : SampleParentComparable, IComparable<SampleChildComparable>
        {
            public SampleChildComparable(int value)
                : base(value)
            {
            }

            public int CompareTo(SampleChildComparable other)
            {
                return base.CompareTo(other);
            }
        }

        private class SampleUnrelatedComparable : IComparable<SampleUnrelatedComparable>, IComparable<int>, IComparable<string>
        {
            private int value;

            public int Value
            {
                get { return value; }
            }

            public SampleUnrelatedComparable(int value)
            {
                this.value = value;
            }

            public int CompareTo(SampleUnrelatedComparable other)
            {
                return (other == null) ? Int32.MaxValue : CompareTo(other.value);
            }

            public int CompareTo(int other)
            {
                return value.CompareTo(other);
            }

            public int CompareTo(string other)
            {
                return (other == null) ? Int32.MaxValue : CompareTo(Int32.Parse(other.Trim()));
            }
        }

        #endregion
    }
}
