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
using MbUnit.Framework.Kernel.Model;
using MbUnit2::MbUnit.Framework;

using MbUnit.Framework.Kernel.Filters;
using Rhino.Mocks;

namespace MbUnit._Framework.Tests.Kernel.Filters
{
    [TestFixture]
    [TestsOn(typeof(MetadataFilter<IModelComponent>))]
    public class MetadataFilterTest : BaseUnitTest
    {
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

            IModelComponent component = Mocks.CreateMock<IModelComponent>();
            SetupResult.For(component.Metadata).Return(metadata);
            Mocks.ReplayAll();

            Assert.AreEqual(expectedMatch, new MetadataFilter<IModelComponent>("key", "expectedValue").IsMatch(component));
        }
    }
}