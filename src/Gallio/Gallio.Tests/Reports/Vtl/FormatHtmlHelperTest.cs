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
using System.IO;
using System.Text;
using System.Xml;
using Gallio.Common.Markup;
using Gallio.Common.Policies;
using Gallio.Reports;
using Gallio.Reports.Vtl;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Gallio.Common.Collections;
using Gallio.Runner.Reports.Schema;
using NVelocity.App;
using NVelocity.Runtime;
using System.Reflection;
using Gallio.Common.Markup.Tags;
using Gallio.Model.Schema;

namespace Gallio.Tests.Reports.Vtl
{
    [TestFixture]
    [TestsOn(typeof(FormatHtmlHelper))]
    public class FormatHtmlHelperTest
    {
        [Test]
        public void NormalizeEndOfLines()
        {
            var helper = new FormatHtmlHelper();
            string actual = helper.NormalizeEndOfLines("line 1\nline 2\r\nline 3\n");
            Assert.AreEqual("line 1<br>line 2<br>line 3<br>", actual);
        }

        [Test]
        [Row("The antbirds are a large family of passerine birds.",
             "The&nbsp;<wbr/>antbirds&nbsp;<wbr/>are&nbsp;<wbr/>a&nbsp;<wbr/>large&nbsp;<wbr/>family&nbsp;<wbr/>of&nbsp;<wbr/>passerine&nbsp;<wbr/>birds<wbr/>.")]
        [Row(@"D:\Root\Folder\File.ext",
             @"D<wbr/>:<wbr/>\Root<wbr/>\Folder<wbr/>\File<wbr/>.ext")]
        [Row(null, "")]
        public void BreakWord(string text, string expected)
        {
            var helper = new FormatHtmlHelper();
            string actual = helper.BreakWord(text);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateUniqueIds()
        {
            var helper = new FormatHtmlHelper();
            var list = new List<string>();

            for (int i = 0; i < 1000; i++)
            {
                string id = helper.GenerateId();
                Assert.DoesNotContain(list, id);
                list.Add(id);
            }
        }

        [Test]
        public void Flatten_null_string()
        {
            string flat = FormatHtmlHelper.Flatten(null);
            Assert.IsEmpty(flat);
        }

        [Test]
        [Row(@"root\parent\a child folder\item%.txt", "root/parent/a%20child%20folder/item%25.txt")]
        [Row(null, "")]
        public void PathToUri(string path, string expected)
        {
            var helper = new FormatHtmlHelper();
            string actual = helper.PathToUri(path);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Flatten()
        {
            string xml =
                "<root>" + "\r\n" +
                "  <parent a='123'>blah " + "\r\n" +
                "       blah</parent>" + "\n" + // simulate inconsistent new lines.
                "       <parent " + "\r\n" +
                "     b='456'><child>" + "\r\n" +
                "   abcefg" + "\r\n" +
                "       </child></parent>" + "\r\n" +
                " </root>";

            string flat = FormatHtmlHelper.Flatten(xml);
            Assert.AreEqual("<root><parent a='123'>blah blah</parent><parent b='456'><child>abcefg</child></parent></root>", flat); // Semantics should be unchanged.
        }
    }
}