// Copyright 2005-2011 Gallio Project - http://www.gallio.org/
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
using System.Linq;
using System.Text;
using Gallio.Common;
using Gallio.Framework;
using Gallio.Framework.Data.DataObjects;
using MbUnit.Framework;

namespace MbUnit.Framework.Tests
{
    [TestFixture]
    public class TabularDataHeaderNameTranslationTests
    {
        /// <summary>
        /// This demonstrates how file header names set Dynamic Object properties.
        /// View the CSV file to see Column Header names and understand how they
        /// are translated into dynamically accessed properties.
        /// </summary>
        [Test]
        [FlatFileDataObject(@"..\Framework\Data\DataObjects\HeaderNames.csv", TabularDataFileType.CsvFile)]
        public void SpreadsheetHeaderNamesInXLS(dynamic sample)
        {
            // Demonstrates access of properties using the DLR
            Assert.AreEqual<string>("123", sample.SimplePropertyName);
            Assert.AreEqual<string>("456", sample.Property_Name_With_Spaces);
            Assert.AreEqual<string>("789", sample.PropertyNameWithNumbers123);
            Assert.AreEqual<string>("111", sample.Property_Name_With_Numbers_And_Spaces_123);
            Assert.AreEqual<string>("222", sample.Property_Name_With_Dashes);
            Assert.AreEqual<string>("333", sample.PropertyNameWithLeadingTrailingSpaces);
            Assert.AreEqual<string>("444", sample.EscapeCharactersTurnInUnderscores___);
        }

        /// <summary>
        /// This tests demonstrates the same functionality as the above test, although in isolation
        /// from the Excel files.
        /// </summary>
        [Test]
        public void DynamicObjectPropertyNameAccessing()
        {
            dynamic sample = new DynamicObject();
            sample.TrySetMember("SimplePropertyName", "123");
            sample.TrySetMember("Property Name With Spaces", "456");
            sample.TrySetMember("PropertyNameWithNumbers123", "789");
            sample.TrySetMember("Property Name With Numbers And Spaces 123", "111");
            sample.TrySetMember("Property-Name-With-Dashes", "222");
            sample.TrySetMember("  PropertyNameWithLeadingTrailingSpaces  ", "333");
            sample.TrySetMember("EscapeCharactersTurnInUnderscores!@#", "444");

            // Demonstrates access of properties using the DLR
            Assert.AreEqual<string>("123", sample.SimplePropertyName);
            Assert.AreEqual<string>("456", sample.Property_Name_With_Spaces);
            Assert.AreEqual<string>("789", sample.PropertyNameWithNumbers123);
            Assert.AreEqual<string>("111", sample.Property_Name_With_Numbers_And_Spaces_123);
            Assert.AreEqual<string>("222", sample.Property_Name_With_Dashes);
            Assert.AreEqual<string>("333", sample.PropertyNameWithLeadingTrailingSpaces);
            Assert.AreEqual<string>("444", sample.EscapeCharactersTurnInUnderscores___);
        }


        /// <summary>
        /// These test isolates the Property Name translation piece - XmlNodeNameToCSharpSafe
        /// </summary>
        [Test]
        public void DemonstratePropertyNameToDataObjectTranslation()
        {
            TestLog.WriteLine("Here's what Property Names Translate to...\n");

            foreach (KeyValuePair<string, string> current in PropertyNameMap)
            {
                string TranslatedValue = DynamicObject.MemberNameToCSharpSafe(current.Key);

                TestLog.WriteLine("Input Property Name: '" + current.Key + "'");
                TestLog.WriteLine("Expected Translated Name: '" + current.Value + "'");
                TestLog.WriteLine("Actual TranslatedName: '" + TranslatedValue + "'");
                TestLog.WriteLine("");

                Assert.AreEqual<string>(current.Value, TranslatedValue);
            }
        }

        // These are the test Column Headers / Property Names
        Dictionary<string, string> PropertyNameMap = new Dictionary<string, string>()
        {
            { "SimplePropertyName", "SimplePropertyName" },
            { "Property Name With Spaces", "Property_Name_With_Spaces" },
            { "PropertyNameWithNumbers123", "PropertyNameWithNumbers123" },
            { "Property Name With Numbers And Spaces 123", "Property_Name_With_Numbers_And_Spaces_123" },
            { "Property-Name-With-Dashes", "Property_Name_With_Dashes" },
            { "  PropertyNameWithLeadingTrailingSpaces  ", "PropertyNameWithLeadingTrailingSpaces" },
            { "EscapeCharactersTurnInUnderscores!@#", "EscapeCharactersTurnInUnderscores___" },
        };
    }
}
