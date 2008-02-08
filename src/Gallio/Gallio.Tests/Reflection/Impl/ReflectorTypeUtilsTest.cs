using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection.Impl
{
    [TestFixture]
    [TestsOn(typeof(ReflectorTypeUtils))]
    public class ReflectorTypeUtilsTest
    {
        [Test]
        public void GetTypeCodeReturnsEmptyIfTypeIsNull()
        {
            Assert.AreEqual(TypeCode.Empty, ReflectorTypeUtils.GetTypeCode(null));
        }

        [Test]
        public void GetTypeCodeReturnsExpectedCodeForAllBuiltInTypes()
        {
            foreach (TypeCode code in Enum.GetValues(typeof(TypeCode)))
            {
                if (code == TypeCode.Empty)
                    continue;

                Type associatedType = Type.GetType("System." + code);
                Assert.AreEqual(code, ReflectorTypeUtils.GetTypeCode(Reflector.Wrap(associatedType)));
            }
        }

        [Test]
        public void GetTypeCodeReturnsObjectByDefault()
        {
            Assert.AreEqual(TypeCode.Object, ReflectorTypeUtils.GetTypeCode(Reflector.Wrap(GetType())));
        }

        [Test]
        public void GetDefaultValueReturnsNullIfTypeIsNull()
        {
            Assert.IsNull(ReflectorTypeUtils.GetDefaultValue(null));
        }

        [Test]
        public void GetDefaultValueReturnsDefaultValueForAssociatedTypeCode()
        {
            foreach (TypeCode code in Enum.GetValues(typeof(TypeCode)))
            {
                if (code == TypeCode.Empty)
                    continue;

                Type associatedType = Type.GetType("System." + code);
                Assert.AreEqual(ReflectionUtils.GetDefaultValue(code), ReflectorTypeUtils.GetDefaultValue(Reflector.Wrap(associatedType)));
            }
        }
    }
}
