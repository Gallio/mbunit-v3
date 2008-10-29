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
    [TestsOn(typeof(VerifyEqualityContractAttribute))]
    [RunSample(typeof(FullContractOnEquatableSample))]
    [RunSample(typeof(PartialContractOnEquatableSample))]
    public class VerifyEqualityContractAttributeTest : VerifyContractAttributeBaseTest
    {
        [Test]
        [Row(typeof(FullContractOnEquatableSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnEquatableSample), "ObjectGetHashCode", TestStatus.Passed)]
        [Row(typeof(FullContractOnEquatableSample), "EquatableEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnEquatableSample), "OperatorEquals", TestStatus.Passed)]
        [Row(typeof(FullContractOnEquatableSample), "OperatorNotEquals", TestStatus.Passed)]
        [Row(typeof(PartialContractOnEquatableSample), "ObjectEquals", TestStatus.Passed)]
        [Row(typeof(PartialContractOnEquatableSample), "ObjectGetHashCode", TestStatus.Passed)]
        [Row(typeof(PartialContractOnEquatableSample), "EquatableEquals", TestStatus.Passed)]
        [Row(typeof(PartialContractOnEquatableSample), "OperatorEquals", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnEquatableSample), "OperatorNotEquals", TestStatus.Inconclusive)]
        public void VerifySampleEqualityContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("EqualityContract", fixtureType, testMethodName, expectedTestStatus);
        }

        [VerifyEqualityContract(typeof(SampleEquatable),
            ImplementsOperatorOverloads = true)]
        [Explicit]
        private class FullContractOnEquatableSample : IEquivalenceClassProvider<SampleEquatable>
        {
            public EquivalenceClassCollection<SampleEquatable> GetEquivalenceClasses()
            {
                return EquivalenceClassCollection<SampleEquatable>.FromDistinctInstances(
                    new SampleEquatable(123),
                    new SampleEquatable(456),
                    new SampleEquatable(789));
            }
        }

        [VerifyEqualityContract(typeof(SampleEquatable),
            ImplementsOperatorOverloads = false)]
        [Explicit]
        private class PartialContractOnEquatableSample : IEquivalenceClassProvider<SampleEquatable>
        {
            public EquivalenceClassCollection<SampleEquatable> GetEquivalenceClasses()
            {
                return EquivalenceClassCollection<SampleEquatable>.FromDistinctInstances(
                    new SampleEquatable(123),
                    new SampleEquatable(456),
                    new SampleEquatable(789));
            }
        }

        /// <summary>
        /// Sample equatable type.
        /// </summary>
        internal class SampleEquatable : IEquatable<SampleEquatable>
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
