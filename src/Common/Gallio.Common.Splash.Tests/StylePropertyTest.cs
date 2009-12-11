using System;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Common.Splash.Tests
{
    public class StylePropertyTest
    {
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
