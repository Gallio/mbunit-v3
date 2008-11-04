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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using MbUnit.Framework.ContractVerifiers.Patterns.ObjectHashCode;
using Gallio.Framework.Assertions;
using Rhino.Mocks;
using MbUnit.Framework.ContractVerifiers.Patterns;

namespace MbUnit.Tests.Framework.ContractVerifiers.ObjectHashCode
{
    [TestFixture]
    public class ObjectHashCodePatternTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullSettings()
        {
            new ObjectHashCodePattern(null);
        }

        private class Foo
        {
            private int value;

            public Foo(int value)
            {
                this.value = value;
            }

            public override int GetHashCode()
            {
                return value;
            }
        }

        private class GoodFooProvider : IEquivalenceClassProvider<Foo>
        {
            public EquivalenceClassCollection<Foo> GetEquivalenceClasses()
            {
                return new EquivalenceClassCollection<Foo>(
                    new EquivalenceClass<Foo>(new Foo(1), new Foo(1)),
                    new EquivalenceClass<Foo>(new Foo(2), new Foo(2)),
                    new EquivalenceClass<Foo>(new Foo(3)));
            }
        }

        private class WrongFooProvider : IEquivalenceClassProvider<Foo>
        {
            public EquivalenceClassCollection<Foo> GetEquivalenceClasses()
            {
                return new EquivalenceClassCollection<Foo>(
                    new EquivalenceClass<Foo>(new Foo(1), new Foo(2)));
            }
        }

        [Test]
        public void DetectsNotEqualHashCodeInSameEquivalenceClass()
        {
            var mockState = MockRepository.GenerateStub<IContractVerifierPatternInstanceState>();
            mockState.Stub(x => x.FixtureType).Return(typeof(WrongFooProvider));
            mockState.Stub(x => x.FixtureInstance).Return(new WrongFooProvider());
            var pattern = new ObjectHashCodePattern(new ObjectHashCodePatternSettings(typeof(Foo)));
            var failures = AssertionHelper.Eval(() =>
            {
                pattern.Run(mockState);
            });
            Assert.AreEqual(1, failures.Length);
        }

        [Test]
        public void DetectsEqualHashCodeInSameEquivalenceClass()
        {
            var mockState = MockRepository.GenerateStub<IContractVerifierPatternInstanceState>();
            mockState.Stub(x => x.FixtureType).Return(typeof(GoodFooProvider));
            mockState.Stub(x => x.FixtureInstance).Return(new GoodFooProvider());
            var pattern = new ObjectHashCodePattern(new ObjectHashCodePatternSettings(typeof(Foo)));
            var failures = AssertionHelper.Eval(() =>
            {
                pattern.Run(mockState);
            });
            Assert.AreEqual(0, failures.Length);
        }
    }
}
