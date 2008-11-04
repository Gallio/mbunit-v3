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
using MbUnit.Framework.ContractVerifiers.Patterns.HasConstructor;
using Gallio.Framework.Assertions;

namespace MbUnit.Tests.Framework.ContractVerifiers.HasConstructor
{
    [TestFixture]
    public class HasConstructorPatternTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullSettings()
        {
            new HasConstructorPattern(null);
        }

        private class Foo
        {
            public Foo(int foo)
            {
            }

            private Foo(String foo)
            {
            }
        }

        [Test]
        public void FindsPublicConstructor()
        {
            var pattern = new HasConstructorPattern(new HasConstructorPatternSettings(
                    typeof(Foo), HasConstructorAccessibility.Public, "Hello", new[] { typeof(int) }));
            var failures = AssertionHelper.Eval(() =>
            {
                pattern.Run(null);
            });
            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void FindsPrivateConstructor()
        {
            var pattern = new HasConstructorPattern(new HasConstructorPatternSettings(
                    typeof(Foo), HasConstructorAccessibility.NonPublic, "Hello", new[] { typeof(string) }));
            var failures = AssertionHelper.Eval(() =>
            {
                pattern.Run(null);
            });
            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void DoesNotFindConstructor()
        {
            var pattern = new HasConstructorPattern(new HasConstructorPatternSettings(
                    typeof(Foo), HasConstructorAccessibility.NonPublic, "Hello", new[] { typeof(int) }));
            var failures = AssertionHelper.Eval(() =>
            {
                pattern.Run(null);
            });
            Assert.AreEqual(1, failures.Length);
        }
    }
}
