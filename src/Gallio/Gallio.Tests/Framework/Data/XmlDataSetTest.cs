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
using System.Xml.XPath;
using Gallio.Common.Collections;
using Gallio.Framework.Data;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(XmlDataSet))]
    [DependsOn(typeof(BaseDataSetTest))]
    public class XmlDataSetTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenDocumentIsNull()
        {
            new XmlDataSet(null, "", false);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenItemPathIsNull()
        {
            new XmlDataSet(delegate { return new XmlDocument(); }, null, false);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void IsDynamicPropertyIsSameAsWasSpecifiedInTheConstructor(bool isDynamic)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<root><rows><row a=\"42\"/></rows></root>");

            XmlDataSet dataSet = new XmlDataSet(delegate { return document; }, "//row", isDynamic);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, true));
            Assert.Count(1, items);
            Assert.AreEqual(isDynamic, items[0].IsDynamic);
        }

        [Test]
        public void GetItemsReturnsNothingIfIsDynamicAndNotIncludingDynamicRows()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<root><rows><row a=\"42\"/></rows></root>");

            XmlDataSet dataSet = new XmlDataSet(delegate { return document; }, "//row", true);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, false));
            Assert.Count(0, items);
        }

        [Test]
        public void ColumnCountIsZero()
        {
            XmlDataSet dataSet = new XmlDataSet(delegate { return new XmlDocument(); }, "", false);
            Assert.AreEqual(0, dataSet.ColumnCount);
        }

        [Test]
        public void CanBindReturnsTrueIfAndOnlyIfTheColumnPathCanBeResolvedInTheDocument()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<root><rows><row a=\"42\"/><row a=\"53\"/></rows></root>");

            XmlDataSet dataSet = new XmlDataSet(delegate { return document; }, "//row", false);

            Assert.IsFalse(dataSet.CanBind(new DataBinding(null, null)),
                "CanBind should return false if there is no binding path.");
            Assert.IsFalse(dataSet.CanBind(new DataBinding(null, "not valid xpath")),
                "CanBind should return false if the binding path is an invalid XPath expression.");
            Assert.IsFalse(dataSet.CanBind(new DataBinding(null, "@b")),
                "CanBind should return false if the binding path cannot be resolved in the rows.");
            Assert.IsTrue(dataSet.CanBind(new DataBinding(null, "@a")),
                "CanBind should return true if the binding path can be resolved in the rows.");
        }

        [Test]
        public void GetItemsReturnsXPathNavigatorsForAllSelectedValues()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<root><rows><row a=\"42\" b=\"x\"/><row a=\"53\" b=\"y\"/></rows></root>");

            XmlDataSet dataSet = new XmlDataSet(delegate { return document; }, "//row", false);

            DataBinding[] bindings = new DataBinding[]
            {
                new DataBinding(null, "@a"),
                new DataBinding(null, "@b")
            };

            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(bindings, true));

            Assert.AreEqual("42", ((XPathNavigator)items[0].GetValue(bindings[0])).Value);
            Assert.AreEqual("x", ((XPathNavigator)items[0].GetValue(bindings[1])).Value);
            Assert.AreEqual("53", ((XPathNavigator)items[1].GetValue(bindings[0])).Value);
            Assert.AreEqual("y", ((XPathNavigator)items[1].GetValue(bindings[1])).Value);

            Assert.Throws<DataBindingException>(delegate { items[0].GetValue(new DataBinding(0, null)); });
            Assert.Throws<DataBindingException>(delegate { items[0].GetValue(new DataBinding(null, "not valid xpath")); });
        }

        [Test]
        public void CanGetDescriptiveDataBindingsFromItem()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<root><rows><row a=\"42\" b=\"x\"/><row a=\"53\" b=\"y\"/></rows></root>");

            XmlDataSet dataSet = new XmlDataSet(delegate { return document; }, "//row", false);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, true));

            Assert.AreElementsEqual(new[]
            {
                new DataBinding(null, ".")
            }, items[0].GetBindingsForInformalDescription());
        }

        [Test]
        public void ProducesMetadata()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<root><rows><row a=\"42\" b=\"x\"><metadata xmlns=\"http://www.gallio.org/\"><entry key=\"Metadata\" value=\"abc\"/></metadata></row><row a=\"53\" b=\"y\"><metadata xmlns=\"http://www.gallio.org/\"><entry key=\"Metadata\" value=\"def\"/></metadata></row></rows></root>");

            XmlDataSet dataSet = new XmlDataSet(delegate { return document; }, "//row", false);
            dataSet.DataLocationName = "<inline>";
            Assert.AreEqual("<inline>", dataSet.DataLocationName);

            DataBinding binding = new DataBinding(null, "@a");
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(new DataBinding[] { binding }, true));

            Assert.AreEqual("42", ((XPathNavigator)items[0].GetValue(binding)).Value);
            PropertyBag map = DataItemUtils.GetMetadata(items[0]);
            Assert.AreEqual("<inline>", map.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual("abc", map.GetValue("Metadata"));

            Assert.AreEqual("53", ((XPathNavigator)items[1].GetValue(binding)).Value);
            map = DataItemUtils.GetMetadata(items[1]);
            Assert.AreEqual("<inline>", map.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual("def", map.GetValue("Metadata"));
        }
    }
}
