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
    public class ComparisonPatternBuilderTest
    {
        private MethodInfo sampleMethodInfo;

        [SetUp]
        public void Setup()
        {
            sampleMethodInfo = typeof(int).GetMethod("CompareTo", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int) }, null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullTargetType()
        {
            new ComparisonPatternBuilder<int>().SetTargetType(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullFriendlyName()
        {
            new ComparisonPatternBuilder<int>().SetName(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullSignatureDescription()
        {
            new ComparisonPatternBuilder<int>().SetSignatureDescription(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullReferenceFunction()
        {
            new ComparisonPatternBuilder<int>().SetFunctionRefers(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullFormattingFunction()
        {
            new ComparisonPatternBuilder<int>().SetFunctionFormats(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullPostProcessingFunction()
        {
            new ComparisonPatternBuilder<int>().SetFunctionPostProcesses(null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeWithMissingTargetType()
        {
            new ComparisonPatternBuilder<int>()
                .SetName("Hello")
                .SetSignatureDescription("Signature")
                .SetFunctionRefers((i, j) => i.CompareTo(j))
                .SetComparisonMethodInfo(sampleMethodInfo)
                .ToPattern();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeWithMissingFriendlyName()
        {
            new ComparisonPatternBuilder<int>()
                .SetTargetType(typeof(string))
                .SetSignatureDescription("Signature")
                .SetFunctionRefers((i, j) => i.CompareTo(j))
                .SetComparisonMethodInfo(sampleMethodInfo)
                .ToPattern();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeWithMissingSignatureDescription()
        {
            new ComparisonPatternBuilder<int>()
                .SetTargetType(typeof(string))
                .SetName("Hello")
                .SetFunctionRefers((i, j) => i.CompareTo(j))
                .SetComparisonMethodInfo(sampleMethodInfo)
                .ToPattern();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeWithMissingReferenceFunction()
        {
            new ComparisonPatternBuilder<int>()
                .SetTargetType(typeof(string))
                .SetName("Hello")
                .SetSignatureDescription("Signature")
                .SetComparisonMethodInfo(sampleMethodInfo)
                .ToPattern();
        }

        [Test]
        public void MakePatternOk()
        {
            var pattern = new ComparisonPatternBuilder<int>()
                .SetTargetType(typeof(string))
                .SetName("Hello")
                .SetSignatureDescription("Signature")
                .SetFunctionRefers((i, j) => i.CompareTo(j))
                .SetComparisonMethodInfo(sampleMethodInfo)
                .ToPattern();
            Assert.IsNotNull(pattern);
            Assert.IsInstanceOfType(typeof(ComparisonPattern<int>), pattern);
        }
    }
}
