// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Common.Splash.Tests
{
    public class StylePropertyTest
    {
        [VerifyContract]
        public readonly IContract EqualityAndHashCode = new EqualityContract<StyleProperty<int>>()
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses =
            {
                StyleProperty<int>.Inherit,
                { new StyleProperty<int>(1), new StyleProperty<int>(1) },
                { new StyleProperty<int>(2), new StyleProperty<int>(2) },
            }
        };

        public class WhenPropertyIsInherited
        {
            [Test]
            public void Inherited_ReturnsTrue()
            {
                var property = StyleProperty<int>.Inherit;
                Assert.IsTrue(property.Inherited);
            }

            [Test]
            public void Value_Throws()
            {
                var property = StyleProperty<int>.Inherit;
                Assert.Throws<InvalidOperationException>(() => { int x = property.Value; });
            }

            [Test]
            public void GetValueOrInherit_ReturnsInheritedValue()
            {
                var property = StyleProperty<int>.Inherit;
                Assert.AreEqual(5, property.GetValueOrInherit(5));
            }
        }

        public class WhenPropertyIsNotInherited
        {
            [Test]
            public void Inherited_ReturnsFalse()
            {
                var property = new StyleProperty<int>(5);
                Assert.IsFalse(property.Inherited);
            }

            [Test]
            public void Value_ReturnsValue()
            {
                var property = new StyleProperty<int>(5);
                Assert.AreEqual(5, property.Value);
            }

            [Test]
            public void GetValueOrInherit_ReturnsValue()
            {
                var property = new StyleProperty<int>(5);
                Assert.AreEqual(5, property.GetValueOrInherit(5));
            }

            [Test]
            public void ImplicitConversion_ReturnsProperty()
            {
                StyleProperty<int> property = 5;
                Assert.AreEqual(5, property.Value);
            }
        }
    }
}
