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
using Gallio.Collections;
using System.Reflection;

namespace MbUnit.Tests.Framework.ContractVerifiers.Equality
{
    [TestFixture]
    public class EqualityPatternSettingsTest
    {
        private MethodInfo sampleMethodInfo;

        [SetUp]
        public void Setup()
        {
            sampleMethodInfo = typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullTargetType()
        {
            new EqualityPatternSettings(null, sampleMethodInfo, "Signature", false, "Name");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullSignatureDescription()
        {
            new EqualityPatternSettings(typeof(object), sampleMethodInfo, null, false, "Name");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullFriendlyName()
        {
            new EqualityPatternSettings(typeof(object), sampleMethodInfo, "Signature", false, null);
        }

        [Test]
        public void ConstructsOk()
        {
            var settings = new EqualityPatternSettings(typeof(object), sampleMethodInfo, "Signature", true, "Name");
            Assert.AreEqual(typeof(object), settings.TargetType);
            Assert.AreSame(sampleMethodInfo, settings.EqualityMethodInfo);
            Assert.AreEqual("Name", settings.Name);
            Assert.AreEqual("Signature", settings.SignatureDescription);
            Assert.IsTrue(settings.Inequality);
        }
    }
}
