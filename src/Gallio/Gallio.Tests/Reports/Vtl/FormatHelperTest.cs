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
using System.Collections.Specialized;
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

namespace Gallio.Tests.Reports.Vtl
{
    [TestFixture]
    [TestsOn(typeof(FormatHelper))]
    public class FormatHelperTest
    {
        [Test]
        public void FormatHelper_NormalizeEndOfLines()
        {
            var helper = new FormatHelper();
            string actual = helper.NormalizeEndOfLines("line 1\nline 2\n");
            string expected = String.Format("line 1{0}line 2{0}", Environment.NewLine);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Row("The antbirds are a large family of passerine birds", "The <wbr/>antbirds <wbr/>are <wbr/>a <wbr/>large <wbr/>family <wbr/>of <wbr/>passerine <wbr/>birds")]
        [Row(@"D:\Root\Folder\File.ext", @"D:\<wbr/>Root\<wbr/>Folder\<wbr/>File.ext")]
        public void FormatHelper_BreakWord(string text, string expected)
        {
            var helper = new FormatHelper();
            string actual = helper.BreakWord(text);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Row("Blue Red.Green. Magenta  .", "BlueRedGreenMagenta")]
        [Row(" .. .", "")]
        [Row("", "")]
        public void FormatHelper_RemoveChars(string input, string expectedOutput)
        {
            var helper = new FormatHelper();
            string output = helper.RemoveChars(input, " .");
            Assert.AreEqual(expectedOutput, output);
        }
    }
}