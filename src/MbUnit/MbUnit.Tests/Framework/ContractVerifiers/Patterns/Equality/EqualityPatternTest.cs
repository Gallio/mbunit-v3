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
using MbUnit.Framework.ContractVerifiers.Patterns.Equality;
using Gallio.Framework.Assertions;
using Gallio.Collections;
using Rhino.Mocks;
using MbUnit.Framework.ContractVerifiers;
using MbUnit.Framework.ContractVerifiers.Patterns;
using System.Reflection;

namespace MbUnit.Tests.Framework.ContractVerifiers.Equality
{
    [TestFixture]
    public class EqualityPatternTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullSettings()
        {
            new EqualityPattern(null);
        }

        private class StringProvider : IEquivalenceClassProvider<string>
        {
            private string[] samples;

            public StringProvider(params string[] samples)
            {
                this.samples = samples;
            }

            public EquivalenceClassCollection<string> GetEquivalenceClasses()
            {
                return EquivalenceClassCollection<string>.FromDistinctInstances(samples);
            }
        }

        private MethodInfo GetStringObjectEqualsMethodInfo()
        {
            return typeof(string).GetMethod("Equals", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null);
        }

        [Test]
        public void EvaluatesStringEquality()
        {
            var mockState = MockRepository.GenerateStub<IContractVerifierPatternInstanceState>();
            mockState.Stub(x => x.FixtureType).Return(typeof(StringProvider));
            mockState.Stub(x => x.FixtureInstance).Return(new StringProvider(String.Empty, "Hello", "World"));
            var pattern = new EqualityPattern(new EqualityPatternSettings(
                typeof(string), GetStringObjectEqualsMethodInfo(), 
                "bool Equals(Object)", false, "ObjectEquals"));
            var failures = AssertionHelper.Eval(() =>
            {
                pattern.Run(mockState);
            });
            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void DetectsIncorrectEquality()
        {
            var mockState = MockRepository.GenerateStub<IContractVerifierPatternInstanceState>();
            mockState.Stub(x => x.FixtureType).Return(typeof(StringProvider));
            mockState.Stub(x => x.FixtureInstance).Return(new StringProvider("Hello", "Hello"));
            var pattern = new EqualityPattern(new EqualityPatternSettings(
                typeof(string), GetStringObjectEqualsMethodInfo(), 
                "bool Equals(Object)", false, "ObjectEquals"));
            var failures = AssertionHelper.Eval(() =>
            {
                pattern.Run(mockState);
            });
            Assert.GreaterThan(failures.Length, 0);
        }
    }
}
