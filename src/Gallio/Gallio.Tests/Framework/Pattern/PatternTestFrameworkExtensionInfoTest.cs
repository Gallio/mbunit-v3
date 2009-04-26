// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio.Framework.Pattern;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Pattern
{
    [TestFixture]
    [TestsOn(typeof(PatternTestFrameworkExtensionInfo))]
    public class PatternTestFrameworkExtensionInfoTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenIdIsNull()
        {
            new PatternTestFrameworkExtensionInfo(null, "name");
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenNameIsNull()
        {
            new PatternTestFrameworkExtensionInfo("id", null);
        }

        [Test]
        public void TheValuesPassedToTheConstructorAreExposedInTheProperties()
        {
            PatternTestFrameworkExtensionInfo info = new PatternTestFrameworkExtensionInfo("id", "name");
            Assert.AreEqual("id", info.Id);
            Assert.AreEqual("name", info.Name);
        }
    }
}
