// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model;
using Gallio.Tests;
using MbUnit2::MbUnit.Framework;

using Gallio.Model.Filters;
using Rhino.Mocks;
using ITestComponent=Gallio.Model.ITestComponent;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(MetadataFilter<ITestComponent>))]
    public class MetadataFilterTest : BaseUnitTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullArgument()
        {
            new MetadataFilter<ITestComponent>(null, new EqualityFilter<string>("expectedValue"));
        }

        [RowTest]
        [Row(true, new string[] { "expectedValue" })]
        [Row(true, new string[] { "this", "that", "expectedValue" })]
        [Row(false, new string[] { "otherValue" })]
        [Row(false, new string[] { })]
        public void IsMatchCombinations(bool expectedMatch, string[] values)
        {
            MetadataMap metadata = new MetadataMap();
            foreach (string value in values)
                metadata.Add("key", value);

            ITestComponent component = Mocks.CreateMock<ITestComponent>();
            SetupResult.For(component.Metadata).Return(metadata);
            Mocks.ReplayAll();

            Assert.AreEqual(expectedMatch, new MetadataFilter<ITestComponent>("key",
                new EqualityFilter<string>("expectedValue")).IsMatch(component));
        }

        [RowTest]
        [Row("Key1", "Member1")]
        [Row("Key1", "Member2")]
        public void ToStringTest(string key, string value)
        {
            MetadataFilter<ITestComponent> filter = new MetadataFilter<ITestComponent>(key,
                new EqualityFilter<string>(value));
            Assert.AreEqual("Metadata('" + key + "', Equality('" + value + "'))", filter.ToString());
        }
    }
}