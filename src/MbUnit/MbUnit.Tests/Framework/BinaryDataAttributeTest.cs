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
using System.IO;
using System.Linq;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(BinaryDataAttribute))]
    [RunSample(typeof(BinaryDataSample))]
    public class BinaryDataAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("Inline", "65 66 67")]
        [Row("ImplicitlyScopedResource", "1 2 3 4")]
        [Row("ExplicitlyScopedResource", "1 2 3 4")]
        public void VerifySampleOutput(string sampleName, string expectedOutput)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(BinaryDataSample).GetMethod(sampleName)));
            Assert.Count(1, run.Children, "Different number of runs than expected.");
            AssertLogContains(run.Children[0], expectedOutput);
        }

        [TestFixture, Explicit("Sample")]
        internal class BinaryDataSample
        {
            private static string FormatBinary(IEnumerable<byte> bytes)
            {
                return String.Join(" ", bytes.Select(x => x.ToString()).ToArray());
            }

            [Test]
            [BinaryData(Contents = "ABC")]
            public void Inline(byte[] data)
            {
                TestLog.WriteLine("({0})", FormatBinary(data));
            }

            [Test]
            [BinaryData(ResourcePath = "SampleBinaryResource.bin")]
            public void ImplicitlyScopedResource(byte[] data)
            {
                TestLog.WriteLine("({0})", FormatBinary(data));
            }

            [Test]
            [BinaryData(ResourceScope = typeof(BinaryDataAttributeTest), ResourcePath = "SampleBinaryResource.bin")]
            public void ExplicitlyScopedResource(byte[] data)
            {
                TestLog.WriteLine("({0})", FormatBinary(data));
            }
        }
    }
}
