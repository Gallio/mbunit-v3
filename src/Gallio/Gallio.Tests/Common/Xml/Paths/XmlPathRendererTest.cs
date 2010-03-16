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
using Gallio.Common.Xml;
using Gallio.Framework;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Common.Xml.Paths;
using System.Collections.Generic;

namespace Gallio.Tests.Common.Xml.Paths
{
    [TestFixture]
    [TestsOn(typeof(XmlPathRenderer))]
    public class XmlPathRendererTest
    {
        [Test]
        [Row("/", "")]
        [Row("/0", "<SolarSystem …>")]
        [Row("/0:0", "<SolarSystem xmlns='SolarSystem'>")]
        [Row("/0/0/0:0", "<SolarSystem …><Planets><Planet name='Mercury' …>")]
        [Row("/0/0/6:0", "<SolarSystem …><Planets>…<Planet name='Saturn' …>")]
        [Row("/0/0/0:1", "<SolarSystem …><Planets><Planet … distanceToSun='0.4 AU'>")]
        [Row("/0/0/0/0", "<SolarSystem …><Planets><Planet name='Mercury' distanceToSun='0.4 AU'><Satellites/>")]
        [Row("/0/0/2", "<SolarSystem …><Planets><!-- Hey! This one is mine! -->")]
        [Row("/0/0/5/0/2:0", "<SolarSystem …><Planets>…<Planet name='Jupiter' distanceToSun='5.2 AU'><Satellites>…<Satellite name='Ganymede'/>")]
        [Row("/0/0/0/0/0/0/0", "<SolarSystem …><Planets><Planet name='Mercury' distanceToSun='0.4 AU'><Satellites/>", Description = "Should truncate path and ignore non-existing nodes")]
        [Row("/0/0/666", "<SolarSystem …><Planets>", Description = "Should truncate path when index out of range")]
        public void Format_path(string input, string expected, [TextData(ResourcePath = "SolarSystem.xml")] string xml)
        {
            NodeFragment fragment = Parser.Run(xml, Options.None);
            var path = XmlPathRoot.Strict.Parse(input);
            var actual = XmlPathRenderer.Run(path, fragment, XmlPathRenderingOptions.None);
            Assert.AreEqual(expected, actual);
        }
    }
}
