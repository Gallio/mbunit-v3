using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Gallio.MSTestAdapter.Model;
using Gallio.Runner.Caching;

namespace Gallio.MSTestAdapter.Wrapper
{
    internal class MSTestRunner2008 : MSTestRunner
    {
        public MSTestRunner2008(IDiskCache diskCache) : base(diskCache)
        {
        }

        protected override string GetVisualStudioVersion()
        {
            return "9.0";
        }

        protected override void WriteTestList(XmlWriter writer, IEnumerable<MSTest> tests, string assemblyFilePath)
        {
            writer.WriteStartDocument();

            writer.WriteStartElement("TestLists", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2006");

            writer.WriteStartElement("TestList");
            writer.WriteAttributeString("name", "Lists of Tests");
            writer.WriteAttributeString("id", RootTestListGuid.ToString());

            writer.WriteEndElement();

            writer.WriteStartElement("TestList");
            writer.WriteAttributeString("id", SelectedTestListGuid.ToString());
            writer.WriteAttributeString("name", SelectedTestListName);
            writer.WriteAttributeString("parentListId", RootTestListGuid.ToString());
            writer.WriteStartElement("TestLinks");

            foreach (MSTest test in tests)
            {
                if (test.IsTestCase)
                {
                    writer.WriteStartElement("TestLink");
                    writer.WriteAttributeString("id", test.Guid);
                    writer.WriteAttributeString("name", test.TestName);
                    writer.WriteAttributeString("storage", assemblyFilePath);
                    writer.WriteAttributeString("type",
                        "Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestElement, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.ObjectModel, PublicKeyToken=b03f5f7f11d50a3a");
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
