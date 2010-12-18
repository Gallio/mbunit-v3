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
using Gallio.Runtime.ConsoleSupport;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.ConsoleSupport
{
    [TestFixture] [Author("JCL")]
    public class CommandLineArgumentAttributeTests
    {
        [Test]
        public void UnLocalizedArgumentAttributeDoesNotNeedResource()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.LongName = "no-logo";
            Assert.AreEqual("no-logo", attribute.LongName);
        }

        [Test]
        public void UnLocalizedNullArgumentAttributeReturnsNull()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Description = null;
            Assert.AreEqual(null, attribute.Description);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Localized attribute strings must supply a resource")]
        public void LocalizedArgumentAttributeMustSupplyResourceType()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Description = "#Description";
            var desc = attribute.LocalizedDescription;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Localized attribute string not found in resource named")]
        public void LocalizedArgumentAttributeMustFindResourceNamed()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            attribute.Description = "#DescriptionWhichWillNotBeFoundInResource";
            var desc = attribute.LocalizedDescription;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Localized attribute string does not point to a string resource")]
        public void LocalizedArgumentAttributeMustFindStringResourceNamed()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            attribute.Description = "#CommandLineArgumentAttributeTests_NotAStringResource";
            var desc = attribute.LocalizedDescription;                  
        }

        [Test]
        public void LocalizedArgumentAttributeShortName()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.ShortName = "#CommandLineArgumentAttributeTests_ShortName";
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("#CommandLineArgumentAttributeTests_ShortName", attribute.ShortName);
            Assert.AreEqual("Short name", attribute.LocalizedShortName);
        }

        [Test]
        public void LocalizedArgumentAttributeLongName()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.LongName = "#CommandLineArgumentAttributeTests_LongName";
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("#CommandLineArgumentAttributeTests_LongName", attribute.LongName);
            Assert.AreEqual("Long name", attribute.LocalizedLongName);
        }

        [Test]
        public void LocalizedArgumentAttributeDescription()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Description = "#CommandLineArgumentAttributeTests_Description";
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("#CommandLineArgumentAttributeTests_Description", attribute.Description);
            Assert.AreEqual("Description", attribute.LocalizedDescription);
        }

        [Test]
        public void LocalizedArgumentAttributeValueLabel()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.ValueLabel = "#CommandLineArgumentAttributeTests_ValueLabel";
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("#CommandLineArgumentAttributeTests_ValueLabel", attribute.ValueLabel);
            Assert.AreEqual("Value label", attribute.LocalizedValueLabel);
        }
        
        [Test]
        public void LocalizedArgumentAttributeOneSynonym()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Synonyms = new [] {"#CommandLineArgumentAttributeTests_Synonym1"};
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("#CommandLineArgumentAttributeTests_Synonym1", attribute.Synonyms[0]);
            Assert.AreEqual("Synonym1", attribute.LocalizedSynonyms[0]);
        }

        [Test]
        public void LocalizedArgumentAttributeMultipleSynonyms()
        {
            var attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Synonyms = new[] { "#CommandLineArgumentAttributeTests_Synonym1", "#CommandLineArgumentAttributeTests_Synonym2" };
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("#CommandLineArgumentAttributeTests_Synonym1", attribute.Synonyms[0]);
            Assert.AreEqual("Synonym1", attribute.LocalizedSynonyms[0]);
            Assert.AreEqual("#CommandLineArgumentAttributeTests_Synonym2", attribute.Synonyms[1]);
            Assert.AreEqual("Synonym2", attribute.LocalizedSynonyms[1]);
        }
        
    }
}
