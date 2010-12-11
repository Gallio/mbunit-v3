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
    [TestFixture]
    public class CommandLineArgumentAttributeTests
    {
        [Test]
        public void UnInternationalizedArgumentAttributeDoesNotNeedResource()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.LongName = "no-logo";
            Assert.AreEqual("no-logo", attribute.LongName);
        }

        [Test]
        public void UnInternationalizedNullArgumentAttributeReturnsNull()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Description = null;
            Assert.AreEqual(null, attribute.Description);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Internationalized attribute strings must supply a resource")]
        public void InternationalizedArgumentAttributeMustSupplyResourceType()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Description = "#Description";
            string desc = attribute.Description;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Internationalized attribute string not found in resource named")]
        public void InternationalizedArgumentAttributeMustFindResourceNamed()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            attribute.Description = "#DescriptionWhichWillNotBeFoundInResource";
            string desc = attribute.Description;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Internationalized attribute string does not point to a string resource")]
        public void InternationalizedArgumentAttributeMustFindStringResourceNamed()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            attribute.Description = "#CommandLineArgumentAttributeTests_NotAStringResource";
            string desc = attribute.Description;                  
        }

        [Test]
        public void InternationalizedArgumentAttributeShortName()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.ShortName = "#CommandLineArgumentAttributeTests_ShortName";
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("Short name", attribute.ShortName);
        }

        [Test]
        public void InternationalizedArgumentAttributeLongName()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.LongName = "#CommandLineArgumentAttributeTests_LongName";
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("Long name", attribute.LongName);
        }

        [Test]
        public void InternationalizedArgumentAttributeDescription()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Description = "#CommandLineArgumentAttributeTests_Description";
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("Description", attribute.Description);
        }

        [Test]
        public void InternationalizedArgumentAttributeValueLabel()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.ValueLabel = "#CommandLineArgumentAttributeTests_ValueLabel";
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("Value label", attribute.ValueLabel);
        }
        
        [Test]
        public void InternationalizedArgumentAttributeOneSynonym()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Synonyms = new [] {"#CommandLineArgumentAttributeTests_Synonym1"};
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("Synonym1", attribute.Synonyms[0]);
        }

        [Test]
        public void InternationalizedArgumentAttributeMultipleSynonyms()
        {
            CommandLineArgumentAttribute attribute = new CommandLineArgumentAttribute(CommandLineArgumentFlags.AtMostOnce);
            attribute.Synonyms = new[] { "#CommandLineArgumentAttributeTests_Synonym1", "#CommandLineArgumentAttributeTests_Synonym2" };
            attribute.ResourceType = typeof(Gallio.Tests.Properties.Resources);
            Assert.AreEqual("Synonym1", attribute.Synonyms[0]);
            Assert.AreEqual("Synonym2", attribute.Synonyms[1]);
        }
        
    }
}
