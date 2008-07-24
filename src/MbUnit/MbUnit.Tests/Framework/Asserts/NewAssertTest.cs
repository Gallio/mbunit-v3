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

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(NewAssert))]
    public class NewAssertTest
    {
        [Test]
        public void GreaterThan_int_test()
        {
            NewAssert.GreaterThan(5, 4);
        }

        [Test]
        public void GreaterThan_double_test()
        {
            NewAssert.GreaterThan(0.001, 0.0001);
        }

        [Test]
        [Row("abc", "ab")]
        [Row("abc", null)]
        public void GreaterThan_string_test(string left, string right)
        {
            NewAssert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThan_with_delegate_test()
        {
            NewAssert.GreaterThan(4, 3, (left, right) => left.CompareTo(0) + right.CompareTo(0));
        }
    }
}