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
using System;
using MbUnit2::MbUnit.Framework;
using System.Collections;
using Gallio.TestResources.MbUnit;
using Gallio.TestResources.MbUnit.Fixtures;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Tests;
using Rhino.Mocks;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(FilterParser<ITest>))]
    [Author("Julian Hidalgo")]
    public class FilterParserTest : BaseUnitTest
    {
        ITest fixture1 = null;
        ITest fixture2 = null;
        ITest fixture3 = null;

        [SetUp]
        public override void SetUp()
        {
            fixture1 = Mocks.CreateMock<ITest>();
            fixture2 = Mocks.CreateMock<ITest>();
            fixture3 = Mocks.CreateMock<ITest>();

            CodeReference codeReference1 = CodeReference.CreateFromType(typeof(SimpleTest));
            SetupResult.For(fixture1.CodeReference).Return(codeReference1);

            CodeReference codeReference2 = CodeReference.CreateFromType(typeof(ParameterizedTest));
            SetupResult.For(fixture2.CodeReference).Return(codeReference2);

            CodeReference codeReference3 = CodeReference.CreateFromType(typeof(FixtureInheritanceSample));
            SetupResult.For(fixture3.CodeReference).Return(codeReference3);

            Mocks.ReplayAll();
        }

        [TearDown]
        public override void TearDown()
        {
            Mocks.VerifyAll();
        }

        [Test]
        public void AnyFilterIsReturnedForEmptyFilterExpressions()
        {
            string filter = "";
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "Any()");
            Assert.AreSame(parsedFilter.GetType(), typeof(AnyFilter<ITest>));
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsTrue(parsedFilter.IsMatch(fixture2));
            Assert.IsTrue(parsedFilter.IsMatch(fixture3));
        }

        [RowTest]
        [Row("SimpleTest")]
        [Row("Gallio.TestResources.MbUnit.SimpleTest")]
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
        
        [Factory(typeof(string))]
        public IEnumerable Types()
        {
            yield return "'SimpleTest'";
            yield return "'Gallio.TestResources.MbUnit.SimpleTest'";
            yield return "\"SimpleTest\"";
            yield return "\"Gallio.TestResources.MbUnit.SimpleTest\"";
        }

        [Factory(typeof(string))]
        public IEnumerable MatchTypes()
        {
            yield return "~";
            yield return string.Empty;
        }

        [CombinatorialTest]
        public void FilterWithRegexValue2(
            [UsingFactories("Types")] string type,
            [UsingFactories("MatchTypes")] string matchType)
        {
            string filter = "Type:" + matchType + type;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            string filterType = GetFilterTypeForMatchType(matchType);
            Assert.AreEqual(parsedFilter.ToString(), "Type("+ filterType + "('" + type.Substring(1, type.Length - 2) + "'), True)");
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsFalse(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [RowTest]
        [Row("SimpleTest", "ParameterizedTest")]
        [Row("Gallio.TestResources.MbUnit.SimpleTest", "ParameterizedTest")]
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

        [RowTest]
        [Row("SimpleTest", "ParameterizedTest")]
        [Row("Gallio.TestResources.MbUnit.SimpleTest", "ParameterizedTest")]
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

        [RowTest]
        [Row("SimpleTest", "ParameterizedTest")]
        [Row("Gallio.TestResources.MbUnit.SimpleTest", "ParameterizedTest")]
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

        [RowTest]
        [Row("SimpleTest", "ParameterizedTest")]
        [Row("Gallio.TestResources.MbUnit.SimpleTest", "ParameterizedTest")]
        public void NotFilter(string type1, string type2)
        {
            string filter = "Type:" + type1 + " and !Type:" + type2;
            Filter<ITest> parsedFilter = FilterUtils.ParseTestFilter(filter);
            Assert.IsNotNull(parsedFilter);
            Assert.AreEqual(parsedFilter.ToString(), "And({ Type(Equality('" + type1 + "'), True), Not(Type(Equality('" + type2 + "'), True)) })");
            Assert.IsTrue(parsedFilter.IsMatch(fixture1));
            Assert.IsFalse(parsedFilter.IsMatch(fixture2));
            Assert.IsFalse(parsedFilter.IsMatch(fixture3));
        }

        [RowTest]
        [Row("Type:\"Fixture1\"")]
        [Row("Type:\"Fixtur\\e1\"")]
        [Row("Type:'Fixture1'")]
        [Row("'Type':Fixture1")]
        [Row("Type:\"Fixture1\",Fixture2")]
        [Row("Type:\"Fixture1\",\"Fixture2\"")]
        [Row("Type:\"Fixture1\",'Fixture2'")]
        [Row("Type:'Fixture1','Fixture2'")]
        [Row("\"Type\":Fixture1")]
        [Row("Type:~\"Fixture1\"")]
        [Row("Type:~'Fixture1'")]
        [Row("(Type:Fixture1 | Type:Fixture2)")]
        public void ValidFiltersTests(string filter)
        {
            // Just making sure they are parsed
            Assert.IsNotNull(FilterUtils.ParseTestFilter(filter));
        }

        private static string GetFilterTypeForMatchType(string matchType)
        {
            string filterType = "Regex";
            if (String.IsNullOrEmpty(matchType))
                filterType = "Equality";

            return filterType;
        }
    }
}
