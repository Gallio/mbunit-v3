// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.Model.Filters;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(EqualityFilter<string>))]
    [Author("Julian Hidalgo")]
    public class EqualityFilterTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullArgument()
        {
            new EqualityFilter<string>(null);
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual(CreateFilter().ToString(), "Equality('MbUnit')");
        }

        [Test]
        public void Match()
        {
            Assert.IsTrue(CreateFilter().IsMatch("MbUnit"));
        }

        [Test]
        [Row(null)]
        [Row("")]
        [Row("MbUnit2")]
        [Row("mbUnit")]
        [Row("MBUNIT")]
        [Row("MBUsNIT")]
        public void NoMatch(string expressionToMatch)
        {
            Assert.IsFalse(CreateFilter().IsMatch(expressionToMatch));
        }

        private EqualityFilter<string> CreateFilter()
        {
            return new EqualityFilter<string>("MbUnit");
        }
    }
}
