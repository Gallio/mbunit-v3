// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Common.Reflection.Impl;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Reflection.Impl
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
