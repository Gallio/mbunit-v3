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

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest_Xml : BaseAssertTest
    {
        private const string declaration = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>";

        [Test]
        [Row("<Empty/>", "<Empty/>")]
        [Row("<Empty></Empty>", "<Empty></Empty>")]
        [Row("<Empty></Empty>", "<Empty/>")]
        [Row("<Node>Hello</Node>", "<Node>Hello</Node>")]
        [Row("<Node x=\"1\"/>", "<Node x=\"1\"/>")]
        [Row("<Node x=\"1\"/>", "<Node  x=\"1\" />")]
        [Row("<Node x=\"1\" y=\"2\"/>", "<Node x=\"1\" y=\"2\"/>")]
        [Row("<Node x=\"1\" y=\"2\"/>", "<Node y=\"2\" x=\"1\"/>")]
        [Row("<Node x=\"1\" y=\"2\"/>", "<Node y=\"2\" x=\"1\" ></Node>")]
        [Row("<Node x=\"1\" y=\"2\">Value</Node>", "<Node x=\"1\" y=\"2\">Value</Node>")]
        [Row("<Node x=\"1\" y=\"2\">Value</Node>", "<Node y=\"2\" x=\"1\">Value</Node>")]
        [Row("<Parent><Child/></Parent>", "<Parent><Child/></Parent>")]
        [Row("<Parent><Child>Value</Child></Parent>", "<Parent><Child>Value</Child></Parent>")]
        [Row("<Parent><Child/><Child/><Child/></Parent>", "<Parent><Child/><Child/><Child/></Parent>")]
        [Row("<Root><Item x=\"1\"/><Item x=\"2\"/><Item x=\"3\"/></Root>", "<Root><Item x=\"1\"></Item><Item x=\"2\"/><Item  x=\"3\" /></Root>")]
        [Row("<Root><Item x=\"1\"/><Item x=\"2\"/><Item x=\"3\"/></Root>", "<Root><Item x=\"2\"/><Item x=\"1\"></Item><Item  x=\"3\" /></Root>")]
        [Row(solarSystemXml, solarSystemXml)]
        public void AreEqual_passes(string expected, string actual, [Column(true, false)] bool withDeclaration)
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
        public void AreEqual_fails(string expectedXml, string actualXml, ExpectedFailureData[] expectedErrors)
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Xml.AreEqual(expectedXml, actualXml));
            Assert.AreEqual(1, failures.Length);
            Assert.AreElementsEqualIgnoringOrder(
                expectedErrors.Select(x => x.ToString()),
                failures[0].InnerFailures.Select(x => x.ToString()),
                (x, y) => y.Replace(" ", String.Empty).StartsWith(x.Replace(" ", String.Empty)));
        }

        public IEnumerable<object[]> ProvideXmlData
        {
            get
            {
                yield return new object[] 
                { 
                    "<Node>Hello</Node>", 
                    "<Node>Salut</Node>",
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
                    solarSystemXml,

                    "<SolarSystem>" +
                    "  <Planets>" +
                    "    <Planet name='Mercury' distanceToSun='0.4 AU'>" +
                    "      <Satellites/>" +
                    "    </Planet>" +
                    "    <Planet name='Venus' distanceToSun='0.7 AU'>" +
                    "      <Satellites/>" +
                    "    </Planet>" +
                    "    <Planet name='Earth' distanceToSun='1 AU'>" +
                    "      <Satellites>" +
                    "        <Satellite name='Moon'/>" +
                    "      </Satellites>" +
                    "    </Planet>" +
                    "    <Planet name='Mars' distanceToSun='1.666 AU'>" + // <-- Wrong distance!
                    "      <Satellites>" +
                    "        <Satellite name='Deimos'/>" +
                    "        <Satellite name='Phobos'/>" +
                    "      </Satellites>" +
                    "    </Planet>" +
                    "    <Planet name='Jupiter' distanceToSun='5.2 AU'>" +
                    "      <Satellites>" +
                    "        <Satellite name='Io'/>" +
                    "        <Satellite name='Europa'/>" +
                    "        <Satellite name='Ganymede'/>" +
                    "        <Satellite name='Callisto'/>" +
                    "      </Satellites>" +
                    "    </Planet>" +
                    "    <Planet name='Saturn' distanceToSun='9.5 AU'>" +
                    "      <Satellites>" +
                    "        <Satellite name='Mimas'/>" +
                    "        <Satellite name='Enceladus'/>" +
                    "        <Satellite name='Tethys'/>" +
                    "        <Satellite name='Dione'/>" +
                    "        <Satellite name='Rhea'/>" +
                    "        <Satellite name='Titan'/>" +
                    "        <Satellite name='Iapetus'/>" +
                    "      </Satellites>" +
                    "    </Planet>" +
                    "    <Planet name='Uranus' distanceToSun='19.6 AU'>" +
                    "      <Satellites>" +
                    "        <Satellite name='Miranda'/>" +
                    "        <Satellite name='Ariel'/>" +
                    "        <SXtellite name='Umbriel'/>" + // <-- Typo!
                    "        <Satellite name='Titania'/>" +
                    "        <Satellite name='Oberon'/>" +
                    "      </Satellites>" +
                    "    </Planet>" +
                    "    <Planet name='Pipo' distanceToSun='666 AU'>" + // <-- Imaginary planet!
                    "      <Satellites/>" +
                    "    </Planet>" +
                    "    <Planet name='Neptune' distanceToSun='30 AU'>" +
                    "      <Satellites>" +
                    "        <Satellite name='Triton'/>" +
                    "      </Satellites>" +
                    "    </Planet>" +
                    "  </Planets>" +
                    "</SolarSystem>",

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
                    }
                };
            }
        }

        private const string solarSystemXml =
            "<SolarSystem>" +
            "  <Planets>" +
            "    <Planet name='Mercury' distanceToSun='0.4 AU'>" +
            "      <Satellites/>" +
            "    </Planet>" +
            "    <Planet name='Venus' distanceToSun='0.7 AU'>" +
            "      <Satellites/>" +
            "    </Planet>" +
            "    <Planet name='Earth' distanceToSun='1 AU'>" +
            "      <Satellites>" +
            "        <Satellite name='Moon'/>" +
            "      </Satellites>" +
            "    </Planet>" +
            "     <Planet name='Mars' distanceToSun='1.5 AU'>" +
            "      <Satellites>" +
            "        <Satellite name='Deimos'/>" +
            "        <Satellite name='Phobos'/>" +
            "      </Satellites>" +
            "    </Planet>" +
            "     <Planet name='Jupiter' distanceToSun='5.2 AU'>" +
            "      <Satellites>" +
            "        <Satellite name='Io'/>" +
            "        <Satellite name='Europa'/>" +
            "        <Satellite name='Ganymede'/>" +
            "        <Satellite name='Callisto'/>" +
            "      </Satellites>" +
            "    </Planet>" +
            "     <Planet name='Saturn' distanceToSun='9.5 AU'>" +
            "      <Satellites>" +
            "        <Satellite name='Mimas'/>" +
            "        <Satellite name='Enceladus'/>" +
            "        <Satellite name='Tethys'/>" +
            "        <Satellite name='Dione'/>" +
            "        <Satellite name='Rhea'/>" +
            "        <Satellite name='Titan'/>" +
            "        <Satellite name='Iapetus'/>" +
            "      </Satellites>" +
            "    </Planet>" +
            "     <Planet name='Uranus' distanceToSun='19.6 AU'>" +
            "      <Satellites>" +
            "        <Satellite name='Miranda'/>" +
            "        <Satellite name='Ariel'/>" +
            "        <Satellite name='Umbriel'/>" +
            "        <Satellite name='Titania'/>" +
            "        <Satellite name='Oberon'/>" +
            "      </Satellites>" +
            "    </Planet>" +
            "     <Planet name='Neptune' distanceToSun='30 AU'>" +
            "      <Satellites>" +
            "        <Satellite name='Triton'/>" +
            "      </Satellites>" +
            "    </Planet>" +
            "  </Planets>" +
            "</SolarSystem>";
    }
}