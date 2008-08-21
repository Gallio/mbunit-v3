using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Tests.Integration;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    [TestsOn(typeof(VerifyComparisonContractAttribute))]
    public class VerifyComparisonContractAttributeTest : VerifyContractAttributeBaseTest
    {
        [Test]
        [Row(typeof(FullContractOnSampleComparableTest), "ComparableCompareTo", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleComparableTest), "OperatorGreaterThan", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleComparableTest), "OperatorGreaterThanOrEqual", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleComparableTest), "OperatorLessThan", TestStatus.Passed)]
        [Row(typeof(FullContractOnSampleComparableTest), "OperatorLessThanOrEqual", TestStatus.Passed)]
        [Row(typeof(PartialContractOnSampleComparableTest), "ComparableCompareTo", TestStatus.Passed)]
        [Row(typeof(PartialContractOnSampleComparableTest), "OperatorGreaterThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnSampleComparableTest), "OperatorGreaterThanOrEqual", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnSampleComparableTest), "OperatorLessThan", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnSampleComparableTest), "OperatorLessThanOrEqual", TestStatus.Inconclusive)]
        public void VerifySampleEqualityContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("ComparisonContract", fixtureType, testMethodName, expectedTestStatus);
        }

        [VerifyComparisonContract(typeof(SampleComparable),
            ImplementsOperatorOverloads = true),
        Explicit]
        private class FullContractOnSampleComparableTest : IEquivalenceClassProvider<SampleComparable>
        {
            public EquivalenceClassCollection<SampleComparable> GetEquivalenceClasses()
            {
                return new EquivalenceClassCollection<SampleComparable>(
                    new EquivalenceClass<SampleComparable>(null),
                    new EquivalenceClass<SampleComparable>(new SampleComparable(123), new SampleComparable(123)),
                    new EquivalenceClass<SampleComparable>(new SampleComparable(456)),
                    new EquivalenceClass<SampleComparable>(new SampleComparable(789)));
            }
        }

        [VerifyComparisonContract(typeof(SampleComparable),
            ImplementsOperatorOverloads = false),
        Explicit]
        private class PartialContractOnSampleComparableTest : IEquivalenceClassProvider<SampleComparable>
        {
            public EquivalenceClassCollection<SampleComparable> GetEquivalenceClasses()
            {
                return new EquivalenceClassCollection<SampleComparable>(
                    new EquivalenceClass<SampleComparable>(null),
                    new EquivalenceClass<SampleComparable>(new SampleComparable(123), new SampleComparable(123)),
                    new EquivalenceClass<SampleComparable>(new SampleComparable(456)),
                    new EquivalenceClass<SampleComparable>(new SampleComparable(789)));
            }
        }

        /// <summary>
        /// Sample comparable type.
        /// </summary>
        internal class SampleComparable : IComparable<SampleComparable>
        {
            private int value;

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
    }
}
