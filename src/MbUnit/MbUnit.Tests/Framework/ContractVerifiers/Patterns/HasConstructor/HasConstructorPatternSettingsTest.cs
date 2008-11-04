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
using Gallio.Collections;

namespace MbUnit.Tests.Framework.ContractVerifiers.HasConstructor
{
    [TestFixture]
    public class HasConstructorPatternSettingsTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullTargetType()
        {
            new HasConstructorPatternSettings(null, HasConstructorAccessibility.Public, "Name", EmptyArray<Type>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullFriendlyName()
        {
            new HasConstructorPatternSettings(typeof(object), HasConstructorAccessibility.Public, null, EmptyArray<Type>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullParameterTypes()
        {
            new HasConstructorPatternSettings(typeof(object), HasConstructorAccessibility.Public, "Name", null);
        }

        [Test]
        public void ConstructsOk()
        {
            var settings = new HasConstructorPatternSettings(typeof(object), HasConstructorAccessibility.Public, "Foo", new[] { typeof(object) });
            Assert.AreEqual(typeof(object), settings.TargetType);
            Assert.AreEqual(HasConstructorAccessibility.Public, settings.Accessibility);
            Assert.AreEqual("HasFooConstructor", settings.Name);
            Assert.AreElementsEqual(new[] { typeof(object) }, settings.ParameterTypes);
        }
    }
}
