// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace MbUnit.TestResources
{
    [TestFixture]
    public class SkippedTests
    {
        [Test]
        [Ignore("Won't run")]
        public void IgnoredTest()
        {
            Assert.Fail("Should never get here");   
        }

        [Test]
        [Pending("Won't run")]
        public void PendingTest()
        {
            Assert.Fail("Should never get here");
        }

        [Test]
        [Explicit("Should only run if explicitly selected.")]
        public void ExplicitTest()
        {
        }

        [TestFixture]
        [Explicit("Should only run if explicitly selected or if one of its test cases is explicitly selected.")]
        public class ExplicitFixture
        {
            [Test]
            public void Test()
            {
            }
        }
    }
}
