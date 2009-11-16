// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Linq;
using System.Collections.Generic;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(ConverterAttribute))]
    [RunSample(typeof(NonConvertibleStubSample))]
    public class ConverterAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void Run()
        {
            TestStepRun testRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(NonConvertibleStubSample).GetMethod("Test")));
            Assert.AreEqual(TestOutcome.Passed, testRun.Result.Outcome);
            AssertLogContains(testRun, "CustomConverter: source = 123");
            AssertLogContains(testRun, "CustomConverter: source = 456");
        }

        internal class NonConvertibleStub
        {
            private readonly int value;

            public int Value
            {
                get
                {
                    return value;
                }
            }

            public NonConvertibleStub(int value)
            {
                this.value = value;
            }
        }

        [Explicit("Sample")]
        internal class NonConvertibleStubSample
        {
            [Converter]
            public static NonConvertibleStub Convert(string source)
            {
                TestLog.WriteLine("CustomConverter: source = {0}", source);
                return new NonConvertibleStub(Int32.Parse(source));
            }

            [Test]
            [Row("123", 123)]
            [Row("456", 456)]
            public void Test(NonConvertibleStub stub, int expectedStubValue)
            {
                Assert.AreEqual(expectedStubValue, stub.Value);
            }
        }
    }
}
