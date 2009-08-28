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
using System.Threading;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using MbUnit.Framework.Xml;

namespace MbUnit.Tests.Framework.Xml
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_xml_should_throw_exception()
        {
            new Parser(null);
        }

        [Test]
        [Row("<Node/>")]
        [Row("<Node>Value</Node>")]
        [Row("<Root><Node1/><Node2/></Root>")]
        [Row("<?xml version=\"1.0\"?><Node/>")]
        [Row("<Node value=\"hello\"/>")]
        [Row("<Node value1=\"123\" value2=\"456\"/>")]
        [Row("<Root><!-- Some comments here... --></Root>")]
        [Row("<Root><Child1/><!-- Some comments here... --><Child2/></Root>")]
        public void Parse_xml_tree(string input)
        {
            var parser = new Parser(input);
            Document document = parser.Run();
            string actualOutput = document.ToXml();
            Assert.AreEqual(input, actualOutput);
        }
    }
}
