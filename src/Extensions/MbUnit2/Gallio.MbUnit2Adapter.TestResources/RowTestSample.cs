// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using MbUnit.Framework;

namespace Gallio.MbUnit2Adapter.TestResources
{
    [TestFixture]
    public class RowTestSample
    {
        [RowTest]
        [Row(3, 4, 5)]
        [Row(6, 8, 10)]
        [Row(1, 1, 1, Description="This one should fail.")]
        public void Pythagoras(int a, int b, int c)
        {
            Assert.AreEqual(c * c, a * a + b * b);
        }
    }
}
