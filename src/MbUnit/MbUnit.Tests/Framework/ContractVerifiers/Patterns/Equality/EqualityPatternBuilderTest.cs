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
    public class EqualityPatternBuilderTest
    {
        private MethodInfo sampleMethodInfo;

        [SetUp]
        public void Setup()
        {
            sampleMethodInfo = typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullTargetType()
        {
            new EqualityPatternBuilder().SetTargetType(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullFriendlyName()
        {
            new EqualityPatternBuilder().SetName(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullSignatureDescription()
        {
            new EqualityPatternBuilder().SetSignatureDescription(null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeWithMissingTargetType()
        {
            new EqualityPatternBuilder()
                .SetName("Hello")
                .SetSignatureDescription("Signature")
                .SetEqualityMethodInfo(sampleMethodInfo)
                .ToPattern();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeWithMissingFriendlyName()
        {
            new EqualityPatternBuilder()
                .SetTargetType(typeof(string))
                .SetSignatureDescription("Signature")
                .SetEqualityMethodInfo(sampleMethodInfo)
                .ToPattern();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeWithMissingSignatureDescription()
        {
            new EqualityPatternBuilder()
                .SetName("Hello")
                .SetTargetType(typeof(string))
                .SetEqualityMethodInfo(sampleMethodInfo)
                .ToPattern();
        }

        [Test]
        public void MakePatternOk()
        {
            var pattern = new EqualityPatternBuilder()
                .SetName("Hello")
                .SetTargetType(typeof(string))
                .SetSignatureDescription("Signature")
                .SetEqualityMethodInfo(sampleMethodInfo)
                .ToPattern();
            Assert.IsNotNull(pattern);
            Assert.IsInstanceOfType(typeof(EqualityPattern), pattern);
        }
    }
}
