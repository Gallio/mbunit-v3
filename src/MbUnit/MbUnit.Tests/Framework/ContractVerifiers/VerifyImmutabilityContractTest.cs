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
using System.Runtime.Serialization;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Tests;
using Gallio.Tests.Integration;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [RunSample(typeof(ImmutableSampleTest))]
    [RunSample(typeof(NonReadOnlyMutableSampleTest))]
    [RunSample(typeof(IndirectMutableSampleTest))]
    [RunSample(typeof(SetterMutableSampleTest))]
    public class VerifyImmutabilityContractTest : AbstractContractVerifierTest
    {
        [Test]
        [Row(typeof(ImmutableSampleTest), "AreReadOnlyFields", TestStatus.Passed)]
        [Row(typeof(NonReadOnlyMutableSampleTest), "AreReadOnlyFields", TestStatus.Failed)]
        [Row(typeof(IndirectMutableSampleTest), "AreReadOnlyFields", TestStatus.Failed)]
        [Row(typeof(SetterMutableSampleTest), "AreReadOnlyFields", TestStatus.Failed)]
        public void VerifySampleImmutabilityContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("ImmutabilityTests", fixtureType, testMethodName, expectedTestStatus);
        }

        [Explicit]
        internal class ImmutableSampleTest
        {
            [ContractVerifier]
            public readonly IContractVerifier ImmutabilityTests = new VerifyImmutabilityContract<ImmutableSample>();
        }

        [Explicit]
        internal class NonReadOnlyMutableSampleTest
        {
            [ContractVerifier]
            public readonly IContractVerifier ImmutabilityTests = new VerifyImmutabilityContract<NonReadOnlyMutableSample>();
        }

        [Explicit]
        internal class IndirectMutableSampleTest
        {
            [ContractVerifier]
            public readonly IContractVerifier ImmutabilityTests = new VerifyImmutabilityContract<IndirectMutableSample>();
        }

        [Explicit]
        internal class SetterMutableSampleTest
        {
            [ContractVerifier]
            public readonly IContractVerifier ImmutabilityTests = new VerifyImmutabilityContract<SetterMutableSample>();
        }

        internal class ImmutableSample
        {
            private readonly double number;
            private readonly string text;
            private readonly ImmutableSubSample foo;

            public ImmutableSample(int number, string text, ImmutableSubSample foo)
            {
                this.number = number;
                this.text = text;
                this.foo = foo;
            }
        }

        internal class ImmutableSubSample
        {
            private readonly double number;

            public ImmutableSubSample(double number)
            {
                this.number = number;
            }
        }

        internal class IndirectMutableSample
        {
            private readonly int number;
            private readonly string text;
            private readonly NonReadOnlyMutableSample foo;

            public IndirectMutableSample(int number, string text, NonReadOnlyMutableSample foo)
            {
                this.number = number;
                this.text = text;
                this.foo = foo;
            }
        }

        internal class NonReadOnlyMutableSample
        {
            private double number; // Not marked as read only!

            public NonReadOnlyMutableSample(double number)
            {
                this.number = number;
            }
        }

        internal class SetterMutableSample
        {
            public double Number
            {
                get;
                set;
            }
        }
    }
}
