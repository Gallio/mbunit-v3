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

using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Common.Xml;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(Path))]
    public class PathTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Extends_with_null_element_name_should_throw_exception()
        {
            Path.Empty.Extend(null);
        }

        [Test]
        public void Empty_path()
        {
            var path = Path.Empty;
            Assert.IsEmpty(path.ToString());
            Assert.IsEmpty(path.ToString("attribute"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Resolve_with_null_attribute_name_should_throw_exception()
        {
            Path.Empty.ToString(null);
        }

        [Test]
        public void Resolve_simple_path()
        {
            var path = Path.Empty.Extend("Root");
            var output = path.ToString();
            Assert.AreEqual("<Root>", output);
        }

        [Test]
        public void Resolve_declaration_path()
        {
            var path = Path.Empty.Extend("xml", true);
            var output = path.ToString();
            Assert.AreEqual("<?xml?>", output);
        }

        [Test]
        public void Resolve_simple_path_with_attribute()
        {
            var path = Path.Empty.Extend("Root");
            var output = path.ToString("name");
            Assert.AreEqual("<Root name='...'>", output);
        }

        [Test]
        public void Resolve_declaration_path_with_attribute()
        {
            var path = Path.Empty.Extend("xml", true);
            var output = path.ToString("name");
            Assert.AreEqual("<?xml name='...' ?>", output);
        }

        [Test]
        public void Resolve_complex_path()
        {
            var path = Path.Empty.Extend("Root").Extend("Parent").Extend("Child");
            var output = path.ToString();
            Assert.AreEqual("<Root><Parent><Child>", output);
        }

        [Test]
        public void Resolve_complex_path_with_attribute()
        {
            var path = Path.Empty.Extend("Root").Extend("Parent").Extend("Child");
            var output = path.ToString("name");
            Assert.AreEqual("<Root><Parent><Child name='...'>", output);
        }
    }
}
