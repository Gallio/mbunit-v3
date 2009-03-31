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
using Gallio.Reflection;
using MbUnit.Framework;
using MbUnit.TestResources;
using MbUnit.TestResources.Fixtures;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Tests;
using Rhino.Mocks;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(FilterParser<ITest>))]
    [Author("Julian Hidalgo")]
    public class FilterParserTest : BaseTestWithMocks
    {
        private ITest fixture1 = null;
        private ITest fixture2 = null;
        private ITest fixture3 = null;
        private ITest fixture4 = null;
        private string fixture1TypeName = null;
        private string fixture2TypeName = null;
        private string fixture3TypeName = null;

        [SetUp]
        public override void SetUp()
        {
            fixture1 = Mocks.StrictMock<ITest>();
            fixture2 = Mocks.StrictMock<ITest>();
            fixture3 = Mocks.StrictMock<ITest>();
            fixture4 = Mocks.StrictMock<ITest>();

            ICodeElementInfo codeElement1 = Reflector.Wrap(typeof(SimpleTest));
            SetupResult.For(fixture1.CodeElement).Return(codeElement1);
            fixture1TypeName = codeElement1.Name;

            ICodeElementInfo codeElement2 = Reflector.Wrap(typeof(ParameterizedTest));
            SetupResult.For(fixture2.CodeElement).Return(codeElement2);
            fixture2TypeName = codeElement2.Name;

            ICodeElementInfo codeElement3 = Reflector.Wrap(typeof(FixtureInheritanceSample));
            SetupResult.For(fixture3.CodeElement).Return(codeElement3);
            fixture3TypeName = codeElement3.Name;

            ICodeElementInfo codeElement4 = Reflector.Wrap(typeof(DerivedFixture));
            SetupResult.For(fixture4.CodeElement).Return(codeElement4);

            Mocks.ReplayAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InstantiateWithNullFactory()
        {
            new FilterParser<ITest>(null);
        }

        [Test]
        [Row("Exact", false)]
        [Row("", true)]
        public void ExactType(string filterType, bool shouldMatch)
        {
            string filter = filterType + "Type:" + fixture3TypeName;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsTrue(parsedFilter.IsMatch(fixture3));
            Assert.AreEqual(parsedFilter.IsMatch(fixture4), shouldMatch);
            Assert.AreEqual(parsedFilter.ToString(), "Type(Equality('" + fixture3TypeName + "'), "
                + (filterType == "Exact" ? "False" : "True") + ")");
        }

        [TearDown]
        public override void TearDown()
        {
            Mocks.VerifyAll();
        }

        [Test]
        [Row(null, ExpectedException = typeof(FilterParseException))]
        [Row("", ExpectedException = typeof(FilterParseException))]
        [Row(" ", ExpectedException = typeof(FilterParseException))]
        [Row("\t", ExpectedException = typeof(FilterParseException))]
        [Row("\n \n", ExpectedException = typeof(FilterParseException))]
        public void AnyFilterIsReturnedForEmptyFilterExpressions(string filter)
        {
            FilterUtils.ParseTestFilter(filter);
        }

        [Test]
        public void AnyFilterIsReturnedForStar()
        {
            string filter = "*";
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "Any()");
            Assert.AreSame(parsedFilter.GetType(), typeof(AnyFilter<ITest>));
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsTrue(parsedFilter.IsMatch(fixture2));
            Assert.IsTrue(parsedFilter.IsMatch(fixture3));
        }

        [Test]
        [Row("* and * or * and *", "(* and *) or (* and *)", true)]
        [Row("* or * and * or *", "* or (* and *) or *", true)]
        [Row("not * or not * and *", "(not *) or ((not *) and *)", false)]
        public void FilterWithStars(string filter1, string filter2, bool matches)
        {
            Filter<ITest> parsedFilter1 = FilterUtils.ParseTestFilter(filter1);
            Assert.IsNotNull(parsedFilter1);

            Filter<ITest> parsedFilter2 = FilterUtils.ParseTestFilter(filter2);
            Assert.IsNotNull(parsedFilter2);

            Assert.AreEqual(parsedFilter1.ToString(), parsedFilter2.ToString());

            Assert.AreEqual(parsedFilter1.IsMatch(fixture1), matches);
            Assert.AreEqual(parsedFilter1.IsMatch(fixture2), matches);
            Assert.AreEqual(parsedFilter1.IsMatch(fixture3), matches);
            Assert.AreEqual(parsedFilter2.IsMatch(fixture1), matches);
            Assert.AreEqual(parsedFilter2.IsMatch(fixture2), matches);
            Assert.AreEqual(parsedFilter2.IsMatch(fixture3), matches);
        }

        [Test]
        [Row("SimpleTest")]
        [Row("MbUnit.TestResources.SimpleTest")]
        public void FilterWithOneValue(string type)
        {
            string filter = "Type:" + type;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "Type(Equality('" + type + "'), True)");
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsFalse(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [Test]
        [Row("'SimpleTest'")]
        [Row("'MbUnit.TestResources.SimpleTest'")]
        [Row("\"SimpleTest\"")]
        [Row("\"MbUnit.TestResources.SimpleTest\"")]
        public void FilterWithQuotedValue(string type)
        {
            string filter = "Type:" + type;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "Type(Equality('"
                + type.Substring(1, type.Length - 2)
                + "'), True)");
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsFalse(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [Test]
        [Row("/SimpleTest/", false)]
        [Row("/MbUnit.TestResources.SimpleTest/", false)]
        [Row("/simpletest/", true)]
        [Row("/MBUNIT.TESTRESOURCES.SIMPLETEST/", true)]
        public void FilterWithRegexValue(string type, bool caseInsensitive)
        {
            string filter = "Type:" + type + (caseInsensitive ? "i" : "");
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "Type(Regex('"
                + type.Substring(1, type.Length - 2)
                + "', "
                + (caseInsensitive ? "IgnoreCase, " : "")
                + "Compiled), True)");
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsFalse(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [Test]
        [Row("SimpleTest", "ParameterizedTest")]
        [Row("MbUnit.TestResources.SimpleTest", "ParameterizedTest")]
        public void FilterWithTwoValues(string type1, string type2)
        {
            string filter = "Type:" + type1 + "," + type2;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "Type(Or({ Equality('" + type1 + "'), Equality('" + type2 + "') }), True)");
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsTrue(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [Test]
        [Row("SimpleTest", "ParameterizedTest")]
        [Row("MbUnit.TestResources.SimpleTest", "ParameterizedTest")]
        public void OrFilter(string type1, string type2)
        {
            string filter = "Type:" + type1 + " or Type:" + type2;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "Or({ Type(Equality('" + type1 + "'), True), Type(Equality('" + type2 + "'), True) })");
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsTrue(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [Test]
        [Row("SimpleTest", "ParameterizedTest")]
        [Row("MbUnit.TestResources.SimpleTest", "ParameterizedTest")]
        public void AndFilter(string type1, string type2)
        {
            string filter = "Type:" + type1 + " and Type:" + type2;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "And({ Type(Equality('" + type1 + "'), True), Type(Equality('" + type2 + "'), True) })");
            Assert.IsFalse(parsedFilter.IsMatch(fixture1));
            Assert.IsFalse(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [Test]
        [Row("SimpleTest", "ParameterizedTest")]
        [Row("MbUnit.TestResources.SimpleTest", "ParameterizedTest")]
        public void NotFilter(string type1, string type2)
        {
            string filter = "Type:" + type1 + " and not Type:" + type2;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "And({ Type(Equality('" + type1 + "'), True), Not(Type(Equality('" + type2 + "'), True)) })");
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsFalse(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [Test]
        [Row("Type:\"Fixture1\"")]
        [Row("Type:\"Fixtur\\\\e1\"")]
        [Row("Type:'Fixture1'")]
        [Row("'Type':Fixture1")]
        [Row("Type:\"Fixture1\",Fixture2")]
        [Row("Type:\"Fixture1\",\"Fixture2\"")]
        [Row("Type:\"Fixture1\",'Fixture2'")]
        [Row("Type:'Fixture1','Fixture2'")]
        [Row("\"Type\":Fixture1")]
        [Row("Type:~\"Fixture1\"")]
        [Row("Type:~'Fixture1'")]
        [Row("(Type:Fixture1 or Type:Fixture2)")]
        [Row("Type:foo''")]
        [Row(@"Type:foo\\")]
        [Row("Type:foo\'blah")]
        [Row("(not not Author: Julian)")]
        public void ValidFiltersTests(string filter)
        {
            // Just making sure they are parsed
            Assert.IsNotNull(FilterUtils.ParseTestFilter(filter));
        }

        [Test]
        [Row("Type:/RegExp/", "Type(Regex('RegExp', Compiled), True)")]
        [Row("Type:/RegExp/i", "Type(Regex('RegExp', IgnoreCase, Compiled), True)")]
        [Row("Type://", "Type(Regex('', Compiled), True)")]
        [Row("Type://i", "Type(Regex('', IgnoreCase, Compiled), True)")]
        [Row(@"Abc: /123 456 \/ 789/", @"Metadata('Abc', Regex('123 456 / 789', Compiled))")]
        [Row(@"Abc: /123 456 \/ 789/i", @"Metadata('Abc', Regex('123 456 / 789', IgnoreCase, Compiled))")]
        public void RegularExpressions(string filter, string parsedFilterString)
        {
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), parsedFilterString);
        }

        [Test]
        [Row("*", "*")]
        [Row("include *", "*")]
        [Row("exclude *", "exclude *")]
        [Row("include * exclude *", "* exclude *")]
        [Row("include * exclude Type: foo and Type: bar", "* exclude (Type: foo and Type: bar)")]
        [Row("Type: foo and Type: bar", "(Type: foo and Type: bar)")]
        public void FilterSets(string filterSetExpr, string parsedFilterSetString)
        {
            FilterSet<ITest> parsedFilterSet = FilterUtils.ParseTestFilterSet(filterSetExpr);
            Assert.IsNotNull(parsedFilterSet);
            Assert.AreEqual(parsedFilterSetString, parsedFilterSet.ToFilterSetExpr());
        }

        [Test]
        [Row("foo''", ExpectedException = typeof(FilterParseException))]
        [Row(@"foo\", ExpectedException = typeof(FilterParseException))]
        [Row(@"foo\\", ExpectedException = typeof(FilterParseException))]
        [Row("foo\'blah", ExpectedException = typeof(FilterParseException))]
        [Row(@"\", ExpectedException = typeof(FilterParseException))]
        [Row(@"'", ExpectedException = typeof(FilterParseException))]
        [Row("\"", ExpectedException = typeof(FilterParseException))]
        [Row("/", ExpectedException = typeof(FilterParseException))]
        [Row("~'Fixture1'", ExpectedException = typeof(FilterParseException))]
        [Row("Type:\"", ExpectedException = typeof(FilterParseException))]
        [Row(@"Type:'", ExpectedException = typeof(FilterParseException))]
        [Row(@"Type:/", ExpectedException = typeof(FilterParseException))]
        [Row(@"Type:foo\", ExpectedException = typeof(FilterParseException))]
        [Row(@"Type:\", ExpectedException = typeof(FilterParseException))]
        [Row(@"Type:'\", ExpectedException = typeof(FilterParseException))]
        [Row("Type:\"\\", ExpectedException = typeof(FilterParseException))]
        [Row("(Author:me", ExpectedException = typeof(FilterParseException))]
        [Row("(Author:", ExpectedException = typeof(FilterParseException))]
        [Row("(Author::", ExpectedException = typeof(FilterParseException))]
        [Row("include", ExpectedException = typeof(FilterParseException))]
        [Row("include exclude", ExpectedException = typeof(FilterParseException))]
        [Row("include * exclude", ExpectedException = typeof(FilterParseException))]
        public void InvalidFilter(string filter)
        {
            FilterUtils.ParseTestFilter(filter);
        }

        [Test]
        public void ComplexFilter1()
        {
            string filter = "((Type: " + fixture1TypeName + ") or (Type: " + fixture2TypeName +
                ")) and not Type:" + fixture3TypeName;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "And({ Or({ Type(Equality('" + fixture1TypeName
                + "'), True), Type(Equality('" + fixture2TypeName
                + "'), True) }), Not(Type(Equality('" + fixture3TypeName
                + "'), True)) })");
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsTrue(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [Test]
        public void ComplexFilter2()
        {
            string filter = "not ((Type: " + fixture1TypeName + ") or (Type: " + fixture2TypeName + ")) and Type:" + fixture3TypeName + "";
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "And({ Not(Or({ Type(Equality('" + fixture1TypeName + "'), True), Type(Equality('" + fixture2TypeName + "'), True) })), Type(Equality('" + fixture3TypeName + "'), True) })");
            Assert.IsFalse(parsedFilter.IsMatch(fixture1));
            Assert.IsFalse(parsedFilter.IsMatch(fixture2));
            Assert.IsTrue(parsedFilter.IsMatch(fixture3));
        }
    }
}
