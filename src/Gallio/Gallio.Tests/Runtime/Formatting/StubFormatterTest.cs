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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Runtime.Formatting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Formatting
{
    [TestsOn(typeof(StubFormatter))]
    public class StubFormatterTest
    {
        [Test]
        public void StubFormatterCanUseBuiltInRules()
        {
            StubFormatter formatter = new StubFormatter();

            // Just try a couple of basic types.
            Assert.AreEqual("\"abc\"", formatter.Format("abc"));
            Assert.AreEqual("1.2m", formatter.Format(1.2m));
            Assert.AreEqual("'\\n'", formatter.Format('\n'));
        }
    }
}
