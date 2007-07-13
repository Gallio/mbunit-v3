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
using MbUnit.Framework.Utilities;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Tests.Core.Utilities
{
    [TestFixture]
    [TestsOn(typeof(ListUtils))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ListUtilsTest
    {
        [RowTest]
        [Row(new int[] { 1, 2, 3 }, 3, new string[] { "1", "2", "3" })]
        public void ConvertAndCopyAll(int[] input, int outputLength, string[] expectedOutput)
        {
            string[] output = new string[outputLength];

            ListUtils.ConvertAndCopyAll<int, string>(input, output, delegate(int value)
            {
                return value.ToString();
            });

            ArrayAssert.AreEqual(expectedOutput, output);
        }

        [RowTest]
        [Row(new int[] { 1, 2, 3 }, new string[] { "1", "2", "3" })]
        public void ConvertAllToArray(int[] input, string[] expectedOutput)
        {
            string[] output = ListUtils.ConvertAllToArray<int, string>(input, delegate(int value)
            {
                return value.ToString();
            });

            ArrayAssert.AreEqual(expectedOutput, output);
        }
    }
}
