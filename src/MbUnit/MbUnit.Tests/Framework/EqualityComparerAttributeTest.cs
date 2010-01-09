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
    [TestsOn(typeof(EqualityComparerAttribute))]
    [RunSample(typeof(NonEquatableStubSample))]
    public class EqualityComparerAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void Run()
        {
            TestStepRun testRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(NonEquatableStubSample).GetMethod("Test")));
            Assert.AreEqual(TestOutcome.Passed, testRun.Result.Outcome);
            IList<TestStepRun> steps = testRun.Children;
            Assert.AreElementsEqualIgnoringOrder(new[]
            {
                "CustomEqualityComparer: x = 123, y = 456",
                "CustomEqualityComparer: x = 123, y = 123",
                "CustomEqualityComparer: x = 456, y = 123"
            }, steps.Select(x => x.TestLog.ToString()),
            (x, y) => y.Contains(x));
        }

        internal class NonEquatableStub
        {
            private readonly int value;

            public int Value
            {
                get
                {
                    return value;
                }
            }

            public NonEquatableStub(int value)
            {
                this.value = value;
            }
        }

        [Explicit("Sample")]
        internal class NonEquatableStubSample
        {
            [EqualityComparer]
            public static bool Equals(NonEquatableStub x, NonEquatableStub y)
            {
                TestLog.WriteLine("CustomEqualityComparer: x = {0}, y = {1}", x.Value, y.Value);
                return x.Value == y.Value;
            }

            [Test]
            [Row(123, 456, false)]
            [Row(123, 123, true)]
            [Row(456, 123, false)]
            public void Test(int value1, int value2, bool expectedEqual)
            {
                var stub1 = new NonEquatableStub(value1);
                var stub2 = new NonEquatableStub(value2);

                if (expectedEqual)
                    Assert.AreEqual(stub1, stub2);
                else
                    Assert.AreNotEqual(stub1, stub2);
            }
        }
    }
}
