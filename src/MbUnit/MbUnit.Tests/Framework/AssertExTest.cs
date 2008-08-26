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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(AssertEx))]
    public class AssertExTest
    {
        [Test, Pending]
        public void Test()
        {
            Assert.Multiple(() =>
            {
                AssertEx.That(() => false);
                int x = 4;
                AssertEx.That(() => x == 5);
                AssertEx.That(() => ! (x == 4));
                AssertEx.That(() => !! (x == 5));
                AssertEx.That(() => new StringBuilder("abc").ToString() == "abcd");

                int[] vals = new int[] { 0, 1, 1, 2, 3, 5, 8, 12, 20 };
                for (int i = 2; i < vals.Length; i++)
                    AssertEx.That(() => vals[i] == vals[i - 1] + vals[i - 2]);

                for (int i = 0; i < 15; i++)
                    AssertEx.That(() => vals[i] > 0);

                vals = null;
                AssertEx.That(() => vals[0] > 0);
            });
        }
    }
}
