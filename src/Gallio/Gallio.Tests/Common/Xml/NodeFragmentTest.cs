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
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Common.Xml;
using Rhino.Mocks;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(NodeFragment))]
    public class NodeFragmentTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_root_should_throw_exception()
        {
            new NodeFragment(new NodeDeclaration(NodeAttributeCollection.Empty), null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_declaration_should_throw_exception()
        {
            var mockRoot = MockRepository.GenerateStub<INode>();
            new NodeFragment(null, mockRoot);
        }

        [Test]
        public void Constructs_empty()
        {
            var mockRoot = MockRepository.GenerateStub<INode>();
            var declaration = new NodeDeclaration(NodeAttributeCollection.Empty);
            var document = new NodeFragment(declaration, mockRoot);
            Assert.AreSame(declaration, document.Declaration);
        }
    }
}
