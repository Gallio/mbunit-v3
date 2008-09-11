using System;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    [VerifyComparisonContract(typeof(SampleComparable), 
        ImplementsOperatorOverloads = true)]
    public class SampleComparableTest : IEquivalenceClassProvider<SampleComparable>
    {
        public EquivalenceClassCollection<SampleComparable> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<SampleComparable>.FromDistinctInstances(
                new SampleComparable(1),
                new SampleComparable(2),
                new SampleComparable(3),
                new SampleComparable(4),
                new SampleComparable(5));
        }
    }

    public class SampleComparable : IComparable<SampleComparable>
    {
        private int value;

        public SampleComparable(int value)
        {
            this.value = value;
        }

        public int CompareTo(SampleComparable other)
        {
            return Object.ReferenceEquals(other, null) ? Int32.MaxValue : value.CompareTo(other.value);
        }

        public static bool operator >=(SampleComparable left, SampleComparable right)
        {
            return (Object.ReferenceEquals(left, null) && Object.ReferenceEquals(right, null)) ||
                (!Object.ReferenceEquals(left, null) && (left.CompareTo(right) >= 0));
        }

        public static bool operator <=(SampleComparable left, SampleComparable right)
        {
            return Object.ReferenceEquals(left, null) || (left.CompareTo(right) <= 0);
        }

        public static bool operator >(SampleComparable left, SampleComparable right)
        {
            return !Object.ReferenceEquals(left, null) && (left.CompareTo(right) > 0);
        }

        public static bool operator <(SampleComparable left, SampleComparable right)
        {
            return (!Object.ReferenceEquals(left, null) || !Object.ReferenceEquals(right, null)) &&
                (Object.ReferenceEquals(left, null) || (left.CompareTo(right) < 0));
        }

        public override string ToString()
        {
            return "SampleComparable(" + value + ")";
        }
    }
}
