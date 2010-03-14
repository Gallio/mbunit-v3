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
using System.Threading;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Common.Xml;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(Parser))]
    public class ParserTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_xml_should_throw_exception()
        {
            Parser.Run(null, Options.None);
        }

        private string sample =
            "<Root>" +
            "  <!-- Some comment -->" +
            "  <Parent>" +
            "    <Child id='123'/>" +       
            "    <Child id='456'>Data</Child>" +
            "  </Parent>" +            
            "</Root>";            

        [Test]
        public void Parse()
        {
            NodeFragment fragment = Parser.Run(sample, Options.None);

            // Level 0.
            var children = fragment.Children.ToList();
            Assert.AreEqual(1, children.Count);
            Assert.IsInstanceOfType<NodeElement>(children[0]);
            Assert.AreEqual("Root", ((NodeElement)children[0]).Name);
            Assert.IsEmpty(((NodeElement)children[0]).Attributes);

            // Level 1.
            children = children[0].Children.ToList();
            Assert.AreEqual(2, children.Count);
            Assert.IsInstanceOfType<NodeComment>(children[0]);
            Assert.AreEqual(" Some comment ", ((NodeComment)children[0]).Text);
            Assert.IsInstanceOfType<NodeElement>(children[1]);
            Assert.AreEqual("Parent", ((NodeElement)children[1]).Name);
            Assert.IsEmpty(((NodeElement)children[1]).Attributes);

            // Level 3 - Child 1.
            children = children[1].Children.ToList();
            Assert.AreEqual(2, children.Count);
            Assert.IsInstanceOfType<NodeElement>(children[0]);
            Assert.AreEqual("Child", ((NodeElement)children[0]).Name);
            Assert.AreEqual(1, ((NodeElement)children[0]).Attributes.Count);
            Assert.AreEqual("id", ((NodeElement)children[0]).Attributes[0].Name);
            Assert.AreEqual("123", ((NodeElement)children[0]).Attributes[0].Value);
            Assert.IsEmpty(((NodeElement)children[0]).Children);

            // Level 3 - Child 2.
            Assert.IsInstanceOfType<NodeElement>(children[1]);
            Assert.AreEqual("Child", ((NodeElement)children[1]).Name);
            Assert.AreEqual(1, ((NodeElement)children[1]).Attributes.Count);
            Assert.AreEqual("id", ((NodeElement)children[1]).Attributes[0].Name);
            Assert.AreEqual("456", ((NodeElement)children[1]).Attributes[0].Value);

            // Level 4.
            children = children[1].Children.ToList();
            Assert.IsInstanceOfType<NodeContent>(children[0]);
            Assert.AreEqual("Data", ((NodeContent)children[0]).Text);
        }
    }
}
