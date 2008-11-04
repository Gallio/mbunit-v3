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
using MbUnit.Framework.ContractVerifiers.Patterns.Comparison;
using Gallio.Collections;
using System.Reflection;

namespace MbUnit.Tests.Framework.ContractVerifiers.Comparison
{
    [TestFixture]
    public class ComparisonPatternSettingsTest
    {
        private MethodInfo sampleMethodInfo;

        [SetUp]
        public void Setup()
        {
            sampleMethodInfo = typeof(int).GetMethod("CompareTo", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int) }, null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullTargetType()
        {
            new ComparisonPatternSettings<int>(null, sampleMethodInfo, "Signature", (i, j) => i.CompareTo(j), x => x.ToString(), x => x, "Name");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullSignatureDescription()
        {
            new ComparisonPatternSettings<int>(typeof(int), sampleMethodInfo, null, (i, j) => i.CompareTo(j), x => x.ToString(), x => x, "Name");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullFriendlyName()
        {
            new ComparisonPatternSettings<int>(typeof(int), sampleMethodInfo, "Signature", (i, j) => i.CompareTo(j), x => x.ToString(), x => x, null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullFormattingFunction()
        {
            new ComparisonPatternSettings<int>(typeof(int), sampleMethodInfo, "Signature", (i, j) => i.CompareTo(j), null, x => x, "Name");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullReferenceFunction()
        {
            new ComparisonPatternSettings<int>(typeof(int), sampleMethodInfo, "Signature", null, x => x.ToString(), x => x, "Name");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructsWithNullPostProcessingFunction()
        {
            new ComparisonPatternSettings<int>(typeof(int), sampleMethodInfo, "Signature", (i, j) => i.CompareTo(j), x => x.ToString(), null, "Name");
        }

        [Test]
        public void ConstructsOk()
        {
            var settings = new ComparisonPatternSettings<int>(typeof(int), sampleMethodInfo, "Signature", (i, j) => i.CompareTo(j), x => x.ToString(), x => 2 * x, "Name");
            Assert.AreEqual(typeof(int), settings.TargetType);
            Assert.AreSame(sampleMethodInfo, settings.ComparisonMethodInfo);
            Assert.AreEqual("Name", settings.Name);
            Assert.AreEqual("Signature", settings.SignatureDescription);
            Assert.AreEqual("123", settings.Formats(123));
            Assert.AreEqual(246, settings.PostProcesses(123));
        }
    }
}
