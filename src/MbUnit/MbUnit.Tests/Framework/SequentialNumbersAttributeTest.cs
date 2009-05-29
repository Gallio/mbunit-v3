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
using System.Collections;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(SequentialNumbersAttribute))]
    [RunSample(typeof(SequentiaNumbersSample))]
    public class SequentialNumbersAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("SingleStartStepCountSequence", new[] { 1, 1.25, 1.5, 1.75, 2 })]
        [Row("SingleStartEndCountSequence", new[] { 1, 1.25, 1.5, 1.75, 2 })]
        [Row("SingleStartEndStepSequence", new[] { 1, 1.25, 1.5, 1.75, 2 })]
        [Row("PrimeSequence", new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 })]
        public void GenerateSequence(string testMethod, decimal[] expectedOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(SequentiaNumbersSample).GetMethod(testMethod)));
            string[] lines = run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()).ToArray();
            Assert.AreElementsEqual(expectedOutput, lines.Select(x => Decimal.Parse(Regex.Match(x, @"\[(?<value>-?(\d*\.)?\d+)\]").Groups["value"].Value)));
        }

        [TestFixture, Explicit("Sample")]
        public class SequentiaNumbersSample
        {
            [Test]
            public void SingleStartStepCountSequence([SequentialNumbers(Start = 1, Step = 0.25, Count = 5)] decimal value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void SingleStartEndCountSequence([SequentialNumbers(Start = 1, End = 2, Count = 5)] decimal value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void SingleStartEndStepSequence([SequentialNumbers(Start = 1, End = 2, Step = 0.25)] decimal value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            [Test]
            public void PrimeSequence([SequentialNumbers(Start = 1, End = 100, Step = 1, Filter = "IsPrime")] int value)
            {
                TestLog.WriteLine("[{0}]", value);
            }

            public static bool IsPrime(decimal number)
            {
                // Simple implementation of the "Sieve of Eratosthenes" algorithm, 
                // that checks for the primality of a number 
                // (http://en.wikipedia.org/wiki/Sieve_of_Eratosthenes)
                int n = (int)Convert.ChangeType(number, typeof(int));
                var array = new BitArray(n + 1, true);

                for (int i = 2; i < n + 1; i++)
                {
                    if (array[i])
                    {
                        for (int j = i * 2; j < n + 1; j += i)
                        {
                            array[j] = false;
                        }

                        if (array[i] && (n == i))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
