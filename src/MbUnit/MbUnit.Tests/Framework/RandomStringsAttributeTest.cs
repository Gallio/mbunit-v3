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
    [TestsOn(typeof(RandomStringsAttribute))]
    [RunSample(typeof(RandomStringsSample))]
    public class RandomStringsAttributeTest : BaseTestWithSampleRunner
    {
        private IEnumerable<string> GetActualValues(string testMethod)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(RandomStringsSample).GetMethod(testMethod)));
            string[] lines = run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()).ToArray();

            foreach (string line in lines)
            {
                var match = Regex.Match(line, @"\[(?<value>.*)\]");
                Assert.IsTrue(match.Success);
                yield return match.Groups["value"].Value;
            }
        }

        [Test]
        [Row("SingleRegex", @"^[A-D]{3}[0-9]{3}$", 100)]
        public void GenerateRandomValuesFromRegularExpression(string testMethod, string expectedMatch, int expectedCount)
        {
            var values = GetActualValues(testMethod);
            Assert.Count(100, values);
            Assert.Multiple(() =>
            {
                foreach (string value in values)
                {
                    Assert.FullMatch(value, expectedMatch);
                }
            });
        }

        [Test]
        public void GenerateFilteredRandomValuesFromRegularExpression()
        {
            var values = GetActualValues("FilteredRegexSequence");
            Assert.Count(100, values);
            Assert.Multiple(() =>
            {
                foreach (string value in values)
                {
                    Assert.FullMatch(value, @"^[A-D]{3}[0-9]{3}$");
                    Assert.IsFalse(value.StartsWith("AAA"));
                    Assert.IsFalse(value.EndsWith("999"));
                }
            });
        }

        [TestFixture, Explicit("Sample")]
        public class RandomStringsSample
        {
            [Test]
            public void SingleRegex([RandomStrings(Pattern = @"[A-D]{3}[0-9]{3}", Count = 100)] string text)
            {
                TestLog.WriteLine("[{0}]", text);
            }

            [Test]
            public void FilteredRegexSequence([RandomStrings(Pattern = @"[A-D]{3}[0-9]{3}", Count = 100, Filter = "MyFilter")] string text)
            {
                TestLog.WriteLine("[{0}]", text);
            }

            [Test]
            public void SinglePreset([RandomStrings(Stock = RandomStringStock.EnCountries, Count = 50)] string country)
            {
                TestLog.WriteLine("[{0}]", country);
            }

            public static bool MyFilter(string text)
            {
                return !text.StartsWith("AAA") && !text.EndsWith("999");
            }
        }
    }
}
