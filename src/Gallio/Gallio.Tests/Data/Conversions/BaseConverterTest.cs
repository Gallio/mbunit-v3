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

using Gallio.Data.Conversions;
using MbUnit.Framework;

namespace Gallio.Tests.Data.Conversions
{
    [TestFixture]
    [TestsOn(typeof(BaseConverter))]
    public class BaseConverterTest
    {
        [Test, ExpectedArgumentNullException]
        public void CanConvertThrowsIfSourceTypeIsNull()
        {
            new NullConverter().CanConvert(null, typeof(int));
        }

        [Test, ExpectedArgumentNullException]
        public void CanConvertThrowsIfTargetTypeIsNull()
        {
            new NullConverter().CanConvert(typeof(int), null);
        }

        [Test, ExpectedArgumentNullException]
        public void GetConversionCostThrowsIfSourceTypeIsNull()
        {
            new NullConverter().GetConversionCost(null, typeof(int));
        }

        [Test, ExpectedArgumentNullException]
        public void GetConversionCostThrowsIfTargetTypeIsNull()
        {
            new NullConverter().GetConversionCost(typeof(int), null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConvertThrowsIfTargetTypeIsNull()
        {
            new NullConverter().Convert(42, null);
        }
    }
}
