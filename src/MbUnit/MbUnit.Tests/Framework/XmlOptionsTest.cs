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
using Gallio.Common.Xml;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(XmlOptions))]
    public class XmlOptionsTest
    {
        [Test]
        public void Get_strict_settings()
        {
            var settings = XmlOptions.Strict;
            Assert.AreEqual(Options.None, settings.Value);
        }

        [Test]
        public void Get_default_settings()
        {
            var settings = XmlOptions.Default;
            Assert.AreEqual(Options.IgnoreAttributesOrder | Options.IgnoreElementsOrder |
                Options.IgnoreComments, settings.Value);
        }

        [Test]
        public void Get_loose_settings()
        {
            var settings = XmlOptions.Loose;
            Assert.AreEqual(Options.IgnoreAttributesOrder | Options.IgnoreElementsOrder |
                Options.IgnoreComments | Options.IgnoreAttributesNameCase |
                Options.IgnoreAttributesValueCase | Options.IgnoreElementsNameCase |
                Options.IgnoreElementsValueCase, settings.Value);
        }

        [Test]
        public void Get_custom_settings()
        {
            var settings = XmlOptions.Custom.IgnoreAttributesNameCase.IgnoreElementsValueCase.IgnoreComments;
            Assert.AreEqual(Options.IgnoreAttributesNameCase | Options.IgnoreElementsValueCase |
                Options.IgnoreComments, settings.Value);
        }


        [Test]
        public void Method()
        {
            var loose = XmlOptions.Loose;
            var def = XmlOptions.Default;
            var strict = XmlOptions.Strict;



        }
    }
}
