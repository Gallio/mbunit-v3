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
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Gallio.Framework;
using Gallio.Framework.Data;
using Gallio.Framework.Data.DataObjects;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;


namespace MbUnit.Framework
{
    /// <summary>
    /// Parses Xml files into the dynamic XmlDataObjects for data-driven tests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// TODO: add more thorough explanation of usage here.  For now, refer to the Unit Tests.
    /// TODO: make the GetElementList method return IEnumerable&lt;XmlDataObject&gt;
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class XmlDataObjectAttribute : MbUnit.Framework.ContentAttribute
    {
        private readonly FactoryKind kind = FactoryKind.Auto;
        private int columnCount = 0;

        // By default, our attribute only uses the XmlDataObjectFactory.Method(List<XElement>)
        private readonly Type factoryType = typeof(XmlDataObjectFactory);
        private readonly string factoryMethodName = "SingleDocument";

        // XPath where the one-to-many Elements are located.
        private string xPath;


        /// <summary>
        /// Specifies the declaring type and name of a static method, property or field
        /// that will provide values for a data-driven test.
        /// </summary>
        /// <param name="xPath"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xPath"/> is null.</exception>
        public XmlDataObjectAttribute(string xPath)
        {
            if (xPath == null)
                throw new ArgumentNullException("xPath");

            this.xPath = xPath;
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            var invoker = new FixtureMemberInvoker<IEnumerable>(factoryType, scope, factoryMethodName);
            XDocument xdocument = OpenXDocument(codeElement);
            var parameters = new object[] { GetElementList(xdocument, xPath) };
            var dataSet = new FactoryDataSet(() => invoker.Invoke(parameters), kind, columnCount);
            dataSource.AddDataSet(dataSet);
        }

        /// <summary>
        /// Uses the existing Gallio plumbing to locate files
        /// </summary>
        protected XDocument OpenXDocument(ICodeElementInfo codeElement)
        {
            using (TextReader reader = OpenTextReader(codeElement))
            {
                return XDocument.Load(reader);
            }
        }

        /// <summary>
        /// Returns one-to-many elements located within xdocument by the XPath.
        /// </summary>
        protected List<XElement> GetElementList(XDocument xdocument, string XPath)
        {
            // Get one-to-many XElements based on the XPath
            List<XNode> SelectedNodes = xdocument.XPathSelectElements(XPath).ToList<XNode>();

            // Are any of the child nodes not XElements..?
            if (SelectedNodes.Any<XNode>(x => x.GetType() != typeof(XElement)))
            {
                throw new ArgumentException("only Element nodes are allowed in the XPath");
            }

            return SelectedNodes.Cast<XElement>().ToList<XElement>();
        }
    }
}

