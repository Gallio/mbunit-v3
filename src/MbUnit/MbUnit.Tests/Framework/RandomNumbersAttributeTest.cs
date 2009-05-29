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
using System.Transactions;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using System.Linq;
using Gallio.Common.Markup;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(RandomNumbersAttribute))]
    [RunSample(typeof(RandomNumbersSample))]
    public class RandomNumbersAttributeTest : BaseTestWithSampleRunner
    {
        private IEnumerable<decimal> GetActualValues(string testMethod)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(RandomNumbersSample).GetMethod(testMethod)));
            string[] lines = run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()).ToArray();

            foreach (string line in lines)
            {
                var match = Regex.Match(line, @"\[(?<value>-?(\d*\.)?\d+)\]");
                Assert.IsTrue(match.Success);
                yield return Decimal.Parse(match.Groups["value"].Value);
            }
        }

        [Test]
        [Row("Single", -10, 10, 100)]
        public void GenerateRandomValues(string testMethod, decimal expectedMinimum, decimal expectedMaximum, int expectedCount)
        {
            var values = GetActualValues(testMethod);
            Assert.AreEqual(expectedCount, values.Count());
            Assert.Multiple(() =>
            {
                foreach (decimal value in values)
                {
                    Assert.Between(value, expectedMinimum, expectedMaximum);
                }
            });
        }

        [Test]
        public void GenerateRandomFilteredValues()
        {
            var values = GetActualValues("RandomOddNumbers");
            Assert.AreEqual(500, values.Count());
            Assert.Multiple(() =>
            {
                foreach (decimal value in values)
                {
                    Assert.AreEqual(0, ((int)value) % 2);
                }
            });
        }

        [TestFixture, Explicit("Sample")]
        public class RandomNumbersSample
        {
            [Test]
            public void Single([RandomNumbers(Minimum = -10, Maximum = 10, Count = 100)] decimal value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void RandomOddNumbers([RandomNumbers(Minimum = 0, Maximum = 100, Count = 500, Filter = "IsOdd")] int value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            public static bool IsOdd(decimal value)
            {
                int n = (int)Convert.ChangeType(value, typeof(int));
                return 0 == n % 2;
            }
        }
    }
}
