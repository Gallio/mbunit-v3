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

using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Common.Xml;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(Declaration))]
    public class DeclarationTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_attributes_should_throw_exception()
        {
            new Declaration(null);
        }

        [Test]
        public void Constructs_with_empty_attributes()
        {
            var declaration = new Declaration(AttributeCollection.Empty);
            Assert.IsEmpty(declaration.Attributes);
        }

        [Test]
        public void Constructs_ok()
        {
            var attribute1 = new Attribute(123, "name1", "value1", 999);
            var attribute2 = new Attribute(456, "name2", "value2", 999);
            var declaration = new Declaration(new[] { attribute1, attribute2 });
            Assert.AreElementsSame(new[] { attribute1, attribute2 }, declaration.Attributes);
        }
    }
}
