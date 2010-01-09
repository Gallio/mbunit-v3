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
using System.Collections.Generic;
using System.Xml;
using Gallio.MSTestAdapter.Model;

namespace Gallio.MSTestAdapter.Wrapper
{
    internal class MSTestRunner2005 : MSTestRunner
    {
        protected override string GetVisualStudioVersion()
        {
            return "8.0";
        }

        protected override void WriteTestMetadata(XmlWriter writer, IEnumerable<MSTest> tests, string assemblyFilePath)
        {
            writer.WriteStartDocument();

            writer.WriteStartElement("Tests");

            writer.WriteStartElement("edtdocversion");
            writer.WriteAttributeString("branch", "retail");
            writer.WriteAttributeString("build", "50727");
            writer.WriteAttributeString("revision", "1433");
            writer.WriteEndElement();

            writer.WriteStartElement("TestCategory");
            writer.WriteAttributeString("type", "Microsoft.VisualStudio.TestTools.Common.TestCategory");

            writer.WriteStartElement("id");
            writer.WriteAttributeString("type", "Microsoft.VisualStudio.TestTools.Common.TestCategoryId");
            writer.WriteStartElement("id");
            writer.WriteAttributeString("type", "System.Guid");
            writer.WriteValue(RootTestListGuid.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("name");
            writer.WriteAttributeString("type", "System.String");
            writer.WriteValue("List of Tests");
            writer.WriteEndElement();

            writer.WriteStartElement("description");
            writer.WriteAttributeString("type", "System.String");
            writer.WriteEndElement();

            writer.WriteStartElement("enabled");
            writer.WriteAttributeString("type", "System.Boolean");
            writer.WriteValue("True");
            writer.WriteEndElement();

            writer.WriteStartElement("parentCategoryId");
            writer.WriteAttributeString("type", "Microsoft.VisualStudio.TestTools.Common.TestCategoryId");
            writer.WriteStartElement("id");
            writer.WriteAttributeString("type", "System.Guid");
            writer.WriteValue("00000000-0000-0000-0000-000000000000");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteEndElement(); // TestCategory

            writer.WriteStartElement("TestCategory");
            writer.WriteAttributeString("type", "Microsoft.VisualStudio.TestTools.Common.TestCategory");

            writer.WriteStartElement("id");
            writer.WriteAttributeString("type", "Microsoft.VisualStudio.TestTools.Common.TestCategoryId");
            writer.WriteStartElement("id");
            writer.WriteAttributeString("type", "System.Guid");
            writer.WriteValue(SelectedTestListGuid.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("name");
            writer.WriteAttributeString("type", "System.String");
            writer.WriteValue(SelectedTestListName);
            writer.WriteEndElement();

            writer.WriteStartElement("description");
            writer.WriteAttributeString("type", "System.String");
            writer.WriteEndElement();

            writer.WriteStartElement("enabled");
            writer.WriteAttributeString("type", "System.Boolean");
            writer.WriteValue("True");
            writer.WriteEndElement();

            writer.WriteStartElement("parentCategoryId");
            writer.WriteAttributeString("type", "Microsoft.VisualStudio.TestTools.Common.TestCategoryId");
            writer.WriteStartElement("id");
            writer.WriteAttributeString("type", "System.Guid");
            writer.WriteValue(RootTestListGuid.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("testLinks");
            writer.WriteAttributeString("type", "System.Collections.Hashtable");

            foreach (MSTest test in tests)
            {
                if (test.IsTestCase)
                {
                    writer.WriteStartElement("key");
                    writer.WriteAttributeString("type", "System.Guid");
                    writer.WriteValue(test.Guid.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("value");
                    writer.WriteAttributeString("type", "Microsoft.VisualStudio.TestTools.Common.Link");

                    writer.WriteStartElement("id");
                    writer.WriteAttributeString("type", "System.Guid");
                    writer.WriteValue(test.Guid.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("name");
                    writer.WriteAttributeString("type", "System.String");
                    writer.WriteValue(test.TestName);
                    writer.WriteEndElement();

                    writer.WriteStartElement("storage");
                    writer.WriteAttributeString("type", "System.String");
                    writer.WriteValue(assemblyFilePath);
                    writer.WriteEndElement();

                    writer.WriteStartElement("type");
                    writer.WriteAttributeString("type", "System.Type, mscorlib");
                    writer.WriteValue("Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestElement, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.ObjectModel, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                    writer.WriteEndElement();

                    writer.WriteStartElement("enabled");
                    writer.WriteAttributeString("type", "System.Boolean");
                    writer.WriteValue("True");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement(); // testLinks

            writer.WriteEndElement(); // TestCategory

            writer.WriteEndElement(); // Tests

            writer.WriteEndDocument();
        }

        protected override void WriteRunConfig(XmlWriter writer)
        {
            /* http://msdn.microsoft.com/en-us/library/ms404663(VS.80).aspx
             * 
             * <?xml version="1.0" encoding="UTF-8"?>
             * <Tests>  
             *   <TestRunConfiguration type="Microsoft.VisualStudio.TestTools.Common.TestRunConfiguration">
             *     <id type="Microsoft.VisualStudio.TestTools.Common.TestRunConfigurationId">
             *       <id type="System.Guid">5d9344ed-bbde-4850-b05e-a7058096e956</id>
             *     </id>
             *     <name type="System.String">Gallio Test Run</name>
             *     <description type="System.String">This is a test run configuration used by Gallio to launch MSTest tests locally.</description>
             *   </TestRunConfiguration>
             * </Tests> 
             */

            writer.WriteStartDocument();

            writer.WriteStartElement("Tests");

            writer.WriteStartElement("TestRunConfiguration");
            writer.WriteAttributeString("type", "Microsoft.VisualStudio.TestTools.Common.TestRunConfiguration");

            writer.WriteStartElement("id");
            writer.WriteAttributeString("type", "Microsoft.VisualStudio.TestTools.Common.TestRunConfigurationId");
            writer.WriteStartElement("id");
            writer.WriteAttributeString("type", "System.Guid");
            writer.WriteValue("5d9344ed-bbde-4850-b05e-a7058096e956");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("name");
            writer.WriteAttributeString("type", "System.String");
            writer.WriteValue("Gallio Test Run");
            writer.WriteEndElement();

            writer.WriteStartElement("description");
            writer.WriteAttributeString("type", "System.String");
            writer.WriteValue("This is a test run configuration used by Gallio to launch MSTest tests locally.");
            writer.WriteEndElement();

            writer.WriteStartElement("runTimeout");
            writer.WriteAttributeString("type", "System.Int32");
            writer.WriteValue("0");
            writer.WriteEndElement();

            writer.WriteStartElement("testTimeout");
            writer.WriteAttributeString("type", "System.Int32");
            writer.WriteValue("300000");
            writer.WriteEndElement();

            writer.WriteStartElement("agentNotRespondingTimeout");
            writer.WriteAttributeString("type", "System.Int32");
            writer.WriteValue("300000");
            writer.WriteEndElement();

            writer.WriteStartElement("deploymentTimeout");
            writer.WriteAttributeString("type", "System.Int32");
            writer.WriteValue("300000");
            writer.WriteEndElement();

            writer.WriteStartElement("bucketSize");
            writer.WriteAttributeString("type", "System.Int32");
            writer.WriteValue("200");
            writer.WriteEndElement();

            writer.WriteStartElement("bucketThreshold");
            writer.WriteAttributeString("type", "System.Int32");
            writer.WriteValue("1000");
            writer.WriteEndElement();

            writer.WriteEndElement(); // TestRunConfiguration

            writer.WriteEndElement(); // Tests

            writer.WriteEndDocument();
        }
    }
}
