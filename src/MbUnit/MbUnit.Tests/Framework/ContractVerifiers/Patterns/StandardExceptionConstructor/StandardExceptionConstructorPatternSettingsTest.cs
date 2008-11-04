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
using MbUnit.Framework.ContractVerifiers.Patterns.StandardExceptionConstructor;
using Gallio.Collections;

namespace MbUnit.Tests.Framework.ContractVerifiers.StandardExceptionConstructor
{
    [TestFixture]
    public class StandardExceptionConstructorPatternSettingsTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullTargetType()
        {
            new StandardExceptionConstructorPatternSettings(
                null, true, "Foo", EmptyArray<Type>.Instance,
                EmptyArray<ExceptionConstructorSpec>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullFriendlyName()
        {
            new StandardExceptionConstructorPatternSettings(
                typeof(ArgumentException), true, null, EmptyArray<Type>.Instance,
                EmptyArray<ExceptionConstructorSpec>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullParameterTypes()
        {
            new StandardExceptionConstructorPatternSettings(
                typeof(ArgumentException), true, "Foo", null,
                EmptyArray<ExceptionConstructorSpec>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullConstructorSpecifications()
        {
            new StandardExceptionConstructorPatternSettings(
                typeof(ArgumentException), true, "Foo", EmptyArray<Type>.Instance,
                null);
        }

        [Test]
        public void ConstructsOk()
        {
            var settings = new StandardExceptionConstructorPatternSettings(
                typeof(ArgumentException), true, "Foo", new[] { typeof(object) },
                new[] { new ExceptionConstructorSpec() });
            Assert.AreEqual(typeof(ArgumentException), settings.TargetExceptionType);
            Assert.IsTrue(settings.CheckForSerializationSupport);
            Assert.AreEqual("IsFooConstructorWellDefined", settings.FriendlyName);
            Assert.AreElementsEqual(new[] { typeof(object) }, settings.ParameterTypes);
            Assert.AreEqual(1, settings.ConstructorSpecifications.Count());
        }
    }
}
