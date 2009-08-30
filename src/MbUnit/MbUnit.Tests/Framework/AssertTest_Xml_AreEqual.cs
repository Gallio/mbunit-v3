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
using System.Linq;
using System.Text.RegularExpressions;
using Gallio.Framework.Assertions;
using MbUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Collections;

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

            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Xml.AreEqual(expected, actual));
            Assert.IsEmpty(failures);
        }

        private void AssertFailures(AssertionFailure[] failures, params string[] expectedDescriptions)
        {
            Assert.AreEqual(1, failures.Length);
            Assert.AreElementsEqualIgnoringOrder(expectedDescriptions, failures[0].LabeledValues.Select(x => x.ToString()));
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
        public void AreEqual_fails(string expectedXml, string actualXml, XmlOptions options, ExpectedFailureData[] expectedErrors)
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Xml.AreEqual(expectedXml, actualXml, options));

            if (expectedErrors.Length > 0)
            {
                Assert.AreEqual(1, failures.Length);
                Assert.AreElementsEqualIgnoringOrder(
                    expectedErrors.Select(x => x.ToString()),
                    failures[0].InnerFailures.Select(x => x.ToString()),
                    (x, y) => y.Replace(" ", String.Empty).StartsWith(x.Replace(" ", String.Empty)));
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
                    XmlOptions.Default,
                    new[] 
                    { 
                        new ExpectedFailureData
                        {
                            Message = "Unexpected element value found.",
                            Path = "<Node>",
                            Expected = "Hello",
                            Actual = "Salut"
                        }
                    }
                };

                yield return new object[] 
                { 
                    "<Root><Parent><Child>Hello</Child></Parent></Root>", 
                    "<Root><Parent><Child>Salut</Child></Parent></Root>",
                    XmlOptions.Default,
                    new[] 
                    { 
                        new ExpectedFailureData
                        {
                            Message = "Unexpected element value found.",
                            Path = "<Root><Parent><Child>",
                            Expected = "Hello",
                            Actual = "Salut"
                        }
                    }
                };

                yield return new object[] 
                { 
                    "<Root><Parent><Child>Hello</Child></Parent></Root>", 
                    "<Root><Parent><Môme>Salut</Môme></Parent></Root>",
                    XmlOptions.Default,
                    new[] 
                    { 
                        new ExpectedFailureData
                        {
                            Message = "Unexpected element found.",
                            Path = "<Root><Parent>",
                            Actual = "Môme"
                        },
                        new ExpectedFailureData
                        {
                            Message = "Missing element.",
                            Path = "<Root><Parent>",
                            Expected = "Child",
                        }
                    }
                };

                yield return new object[] 
                { 
                    "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><Root/>", 
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Root/>", 
                    XmlOptions.Default,
                    new[] 
                    { 
                        new ExpectedFailureData
                        {
                            Message = "Unexpected attribute value found.",
                            Path = "<?xml encoding='...' ?>",
                            Actual = "UTF-8",
                            Expected = "ISO-8859-1"
                        },
                    }
                };

                yield return new object[] 
                { 
                    "<Root><Node1/><!-- Expected text. --><Node2/></Root>", 
                    "<Root><Node1/><!-- Actual text. --><Node2/></Root>", 
                    XmlOptions.Strict,
                    new[] 
                    { 
                        new ExpectedFailureData
                        {
                            Message = "Unexpected comment found.",
                            Path = "<Root>",
                            Actual = " Actual text. ",
                            Expected = " Expected text. "
                        },
                    }
                };

                yield return new object[] 
                { 
                    "<Root><Child value='123X'/><Child/><Child value='456Z'/></Root>",
                    "<ROOT><Child></Child><CHILD value='123x'/><!-- Yipee! --><child vaLUE = '456z'/></ROOT>",
                    XmlOptions.Loose,
                    EmptyArray<ExpectedFailureData>.Instance
                };

                yield return new object[] 
                { 
                    Properties.Resources.SolarSystem,
                    Properties.Resources.SolarSystemWithErrors,
                    XmlOptions.Custom.IgnoreElementsOrder,
                    new ExpectedFailureData[] 
                    { 
                        new ExpectedFailureData
                        {
                            Message = "Unexpected attribute value found.",
                            Path = "<SolarSystem><Planets><Planet distanceToSun='...'>",
                            Actual = "1.666 AU",
                            Expected = "1.5 AU"
                        },
                        new ExpectedFailureData
                        {
                            Message = "Unexpected element found.",
                            Path = "<SolarSystem><Planets><Planet><Satellites>",
                            Actual = "SXtellite",
                        },
                        new ExpectedFailureData
                        {
                            Message = "Missing element.",
                            Path = "<SolarSystem><Planets><Planet><Satellites>",
                            Expected = "Satellite",
                        },
                        new ExpectedFailureData
                        {
                            Message = "Unexpected element found.",
                            Path = "<SolarSystem><Planets>",
                            Actual = "Planet",
                        },
                        new ExpectedFailureData
                        {
                            Message = "Unexpected comment found.",
                            Path = "<SolarSystem><Planets>",
                            Actual = " Hey! This one is yours! ",
                            Expected = " Hey! This one is mine! "
                        },
                    }
                };
            }
        }
    }
}