// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

extern alias MbUnit2;
using MbUnit2::MbUnit.Framework;
using System;
using System.Text.RegularExpressions;
using Gallio.Model.Filters;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(RegexFilter))]
    [Author("Julian Hidalgo")]
    public class RegexFilterTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullArgument()
        {
            new RegexFilter(null);
        }

        [Test]
        public void ToStringTest()
        {
            string someExpression = "Some Expression *";
            RegexFilter filter = new RegexFilter(new Regex(someExpression));
            Assert.AreEqual(filter.ToString(), "Regex('" + someExpression + "', None)");
        }

        [RowTest]
        [Row("Acb", RegexOptions.None)]
        [Row("Acbb", RegexOptions.None)]
        [Row("Accb", RegexOptions.None)]
        [Row("acb", RegexOptions.IgnoreCase)]
        public void Matches(string expressionToMatch, RegexOptions regexOptions)
        {
            string someExpression = "A*b+";
            RegexFilter filter = new RegexFilter(new Regex(someExpression, regexOptions));
            Assert.AreEqual(filter.ToString(), "Regex('" + someExpression + "', " + regexOptions + ")");
            Assert.IsTrue(filter.IsMatch(expressionToMatch));
        }

        [RowTest]
        [Row(null)]
        [Row("")]
        [Row("Acaede")]
        [Row("a*cB")]
        public void NoMatches(string expressionToMatch)
        {
            string someExpression = "A*b+";
            RegexFilter filter = new RegexFilter(new Regex(someExpression));
            Assert.AreEqual(filter.ToString(), "Regex('" + someExpression + "', None)");
            Assert.IsFalse(filter.IsMatch(expressionToMatch));
        }
    }
}
