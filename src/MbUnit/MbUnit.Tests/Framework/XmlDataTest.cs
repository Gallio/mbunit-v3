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
using System.IO;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(XmlDataAttribute))]
    [RunSample(typeof(XmlDataSample))]
    public class XmlDataTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("Inline", new string[] { "(abc, 123)", "(def, 456)" })]
        [Row("ImplicitlyScopedResource", new string[] { "Apples: 1.00", "Bananas: 1.50", "Cookies: 2.00" })]
        [Row("ExplicitlyScopedResource", new string[] { "Apples: 1.00", "Bananas: 1.50", "Cookies: 2.00" })]
        [Row("AbsolutelyScopedResource", new string[] { "Apples: 1.00", "Bananas: 1.50", "Cookies: 2.00" })]
        [Row("File", new string[] { "Apples: 1.00", "Bananas: 1.50", "Cookies: 2.00" })]
        public void VerifySampleOutput(string sampleName, string[] output)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(XmlDataSample).GetMethod(sampleName)));

            Assert.AreEqual(output.Length, run.Children.Count, "Different number of runs than expected.");

            for (int i = 0; i < output.Length; i++)
            {
                AssertLogContains(run.Children[i], output[i]);
                Assert.IsNotNull(run.Children[i].Step.Metadata.GetValue(MetadataKeys.DataLocation));
            }
        }

        [Test]
        public void Metadata()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(XmlDataSample).GetMethod("File")));

            Assert.AreEqual(@"..\Framework\XmlDataTest.xml", run.Children[0].Step.Metadata.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual(@"Worm", run.Children[0].Step.Metadata.GetValue("ConsumedBy"));

            Assert.AreEqual(@"..\Framework\XmlDataTest.xml", run.Children[1].Step.Metadata.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual(@"Monkey", run.Children[1].Step.Metadata.GetValue("ConsumedBy"));

            Assert.AreEqual(@"..\Framework\XmlDataTest.xml", run.Children[2].Step.Metadata.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual(@"Cookie Monster", run.Children[2].Step.Metadata.GetValue("ConsumedBy"));
        }

        [TestFixture, Explicit("Sample")]
        internal class XmlDataSample
        {
            [Test]
            [XmlData("//item", Contents = @"<data><item name=""abc"" value=""123""/><item name=""def"" value=""456""/></data>")]
            public void Inline([Bind("@name")] string x, [Bind("@value")] int y)
            {
                TestLog.WriteLine("({0}, {1})", x, y);
            }

            [Test]
            [XmlData("//item", ResourcePath = "XmlDataTest.xml")]
            public void ImplicitlyScopedResource([Bind("price")] decimal price, [Bind("@name")] string item)
            {
                TestLog.WriteLine("{0}: {1}", item, price);
            }

            [Test]
            [XmlData("//item", ResourceScope = typeof(XmlDataTest), ResourcePath = "XmlDataTest.xml")]
            public void ExplicitlyScopedResource([Bind("price")] decimal price, [Bind("@name")] string item)
            {
                TestLog.WriteLine("{0}: {1}", item, price);
            }

            [Test]
            [XmlData("//item", ResourcePath = @"MbUnit.Tests\Framework\XmlDataTest.xml")]
            public void AbsolutelyScopedResource([Bind("price")] decimal price, [Bind("@name")] string item)
            {
                TestLog.WriteLine("{0}: {1}", item, price);
            }

            [Test]
            [XmlData("//item", FilePath = @"..\Framework\XmlDataTest.xml")]
            public void File([Bind("price")] decimal price, [Bind("@name")] string item)
            {
                TestLog.WriteLine("{0}: {1}", item, price);
            }
        }
    }
}
