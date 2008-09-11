using System;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    [VerifyEqualityContract(typeof(SampleEquatable))]
    public class SampleEquatableTest : IEquivalenceClassProvider<SampleEquatable>
    {
        public EquivalenceClassCollection<SampleEquatable> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<SampleEquatable>.FromDistinctInstances(
                new SampleEquatable(1),
                new SampleEquatable(2),
                new SampleEquatable(3),
                new SampleEquatable(4),
                new SampleEquatable(5));
        }
    }

    public class SampleEquatable : IEquatable<SampleEquatable>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as SampleEquatable);
        }

        public bool Equals(SampleEquatable other)
        {
            return !Object.ReferenceEquals(other, null) &&
                (value == other.value);
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

        public override string ToString()
        {
            return "SampleEquatable(" + value + ")";
        }
    }
}
