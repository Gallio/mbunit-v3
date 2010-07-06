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
using System.IO;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(TextDataAttribute))]
    [RunSample(typeof(TextDataSample))]
    public class TextDataAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("Inline", "(Ga-Bu-Zo-Meu)")]
        [Row("ImplicitlyScopedResource", "(Hello World!)")]
        [Row("ExplicitlyScopedResource", "(Hello World!)")]
        public void VerifySampleOutput(string sampleName, string expectedOutput)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(TextDataSample).GetMethod(sampleName)));
            Assert.Count(1, run.Children, "Different number of runs than expected.");
            AssertLogContains(run.Children[0], expectedOutput);
        }

        [TestFixture, Explicit("Sample")]
        internal class TextDataSample
        {
            [Test]
            [TextData(Contents = "Ga-Bu-Zo-Meu")]
            public void Inline(string text)
            {
                TestLog.WriteLine("({0})", text);
            }

            [Test]
            [TextData(ResourcePath = "SampleTextResource.txt")]
            public void ImplicitlyScopedResource(string text)
            {
                TestLog.WriteLine("({0})", text);
            }

            [Test]
            [TextData(ResourceScope = typeof(TextDataAttributeTest), ResourcePath = "SampleTextResource.txt")]
            public void ExplicitlyScopedResource(string text)
            {
                TestLog.WriteLine("({0})", text);
            }
        }
    }
}
