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
using System.Linq;
using System.Text.RegularExpressions;
using Gallio.Framework.Assertions;
using MbUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Collections;
using System.Reflection;
using System.IO;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest_Xml_AreEqual : BaseAssertTest
    {
        private const string declaration = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>";

        [Test]
        [Row("<Empty/>", "<Empty/>")]
        [Row("<Empty></Empty>", "<Empty></Empty>")]
        [Row("<Empty></Empty>", "<Empty/>")]
        [Row("<Node>Hello</Node>", "<Node>Hello</Node>")]
        [Row("<Node x='1'/>", "<Node x='1'/>")]
        [Row("<Node x='1'/>", "<Node  x='1' />")]
        [Row("<Node x='1' y='2'/>", "<Node x='1' y='2'/>")]
        [Row("<Node x='1' y='2'/>", "<Node y='2' x='1'/>")]
        [Row("<Node x='1' y='2'/>", "<Node y='2' x='1' ></Node>")]
        [Row("<Node x='1' y='2'>Value</Node>", "<Node x='1' y='2'>Value</Node>")]
        [Row("<Node x='1' y='2'>Value</Node>", "<Node y='2' x='1'>Value</Node>")]
        [Row("<Parent><Child/></Parent>", "<Parent><Child/></Parent>")]
        [Row("<Parent><Child>Value</Child></Parent>", "<Parent><Child>Value</Child></Parent>")]
        [Row("<Parent><Child/><Child/><Child/></Parent>", "<Parent><Child/><Child/><Child/></Parent>")]
        [Row("<Root><Item x='1'/><Item x='2'/><Item x='3'/></Root>", "<Root><Item x='1'></Item><Item x='2'/><Item  x='3' /></Root>")]
        [Row("<Root><Item x='1'/><Item x='2'/><Item x='3'/></Root>", "<Root><Item x='2'/><Item x='1'></Item><Item  x='3' /></Root>")]
        public void AreEqual_passes(string expected, string actual,  [Column(true, false)] bool withDeclaration)
        {
            if (withDeclaration)
            {
                expected = declaration + expected;
                actual = declaration + actual;
            }

            Assert.Xml.AreEqual(expected, actual);
        }

        public struct ExpectedFailureData
        {
            public string Path;
            public string Message;
            public string Actual;
            public string Expected;

            public override string ToString()
            {
                var builder = new StringBuilder(Message);
                builder.Append("\n\nPath : " + Path);

                if (!String.IsNullOrEmpty(Expected))
                {
                    builder.AppendFormat("\nExpected Value : \"{0}\"", Expected);
                }

                if (!String.IsNullOrEmpty(Actual))
                {
                    builder.AppendFormat("\nActual Value : \"{0}\"", Actual);
                }

                return builder.ToString();
            }
        }

        [Test]
        [Factory("ProvideXmlData")]
        public void AreEqual_fails(string expectedXml, string actualXml, XmlOptions options, int expectedErrors)
        {
            AssertionFailure[] failures = Capture(() => Assert.Xml.AreEqual(expectedXml, actualXml, options));

            if (expectedErrors > 0)
            {
                Assert.Count(1, failures);
                Assert.Count(expectedErrors, failures[0].InnerFailures);
            }
            else
            {
                Assert.IsEmpty(failures);
            }
        }

        public IEnumerable<object[]> ProvideXmlData
        {
            get
            {
                yield return new object[] 
                { 
                    "<Node>Hello</Node>", 
                    "<Node>Salut</Node>",
                    XmlOptions.Default, 1
                };

                yield return new object[] 
                { 
                    "<Root><Parent><Child>Hello</Child></Parent></Root>", 
                    "<Root><Parent><Child>Salut</Child></Parent></Root>",
                    XmlOptions.Default, 1
                };

                yield return new object[] 
                { 
                    "<Root><Parent><Child>Hello</Child></Parent></Root>", 
                    "<Root><Parent><Môme>Salut</Môme></Parent></Root>",
                    XmlOptions.Default, 2
                };

                yield return new object[] 
                { 
                    "<?xml version='1.0' encoding='ISO-8859-1'?><Root/>", 
                    "<?xml version='1.0' encoding='UTF-8'?><Root/>", 
                    XmlOptions.Default, 1
                };

                yield return new object[] 
                { 
                    "<Root><Node1/><!-- Expected text. --><Node2/></Root>", 
                    "<Root><Node1/><!-- Actual text. --><Node2/></Root>", 
                    XmlOptions.Strict, 1
                };

                yield return new object[] 
                { 
                    "<Root><Child value='123X'/><Child/><Child value='456Z'/></Root>",
                    "<ROOT><Child></Child><CHILD value='123x'/><!-- Yipee! --><child vaLUE = '456z'/></ROOT>",
                    XmlOptions.Loose, 0
                };

                yield return new object[] 
                { 
                    GetTextResource("MbUnit.Tests.Framework.SolarSystem.xml"),
                    GetTextResource("MbUnit.Tests.Framework.SolarSystemWithErrors.xml"),
                    XmlOptions.Custom.IgnoreElementsOrder, 5
                };

                yield return new object[] // issue 841
                { 
                    "<root><a/><b x='123'/></root>",
                    "<root><b/></root>",
                    XmlOptions.Default, 2
                };
            }
        }

        [Test, Explicit]
        public void AreEqual_fails_explicit()
        {
            string expected = GetTextResource("MbUnit.Tests.Framework.SolarSystem.xml");
            string actual = GetTextResource("MbUnit.Tests.Framework.SolarSystemWithErrors.xml");
            Assert.Xml.AreEqual(expected, actual);
        }
    }
}