// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
