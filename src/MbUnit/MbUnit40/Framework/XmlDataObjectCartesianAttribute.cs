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
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Gallio.Common;
using Gallio.Common.IO;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Data;
using Gallio.Framework.Data.DataObjects;
using Gallio.Framework.Pattern;


namespace MbUnit.Framework
{
    /// <summary>
    /// Parses one-to-many Xml files into dynamic XmlDataObjects for data-driven tests.
    /// Returns the Cartesian Product of all of sets (lists) of Elements specified by the XPaths.
    /// </summary>
    /// <remarks>
    /// <para>
    /// 
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = false, Inherited = true)]
    public class XmlDataObjectCartesianAttribute : DataPatternAttribute
    {
        private readonly FactoryKind kind = FactoryKind.Auto;
        private int columnCount;

        // By default, our attribute is only uses the XmlDataObjectFactory.Method(List<List<XElement>>)
        private readonly Type factoryType = typeof(XmlDataObjectFactory);
        private readonly string factoryMethodName = "MultipleDocuments";

        // This mandatory properties are passed to the factory method
        private object[] FileAndXPathCouplets;


        /// <summary>
        /// Specifies the declaring type and name of a static method, property or field
        /// that will provide values for a data-driven test.
        /// </summary>
        /// <param name="FileAndXPathCouplets">Array contain pairs of Xml File Paths and XPaths</param>
        public XmlDataObjectCartesianAttribute(params object[] FileAndXPathCouplets)
        {
            // First things first: we need pairs of Xml files and XPaths
            if (FileAndXPathCouplets.Length % 2 != 0)
            {
                throw new ArgumentException("Even number of arguments is required");
            }

            this.FileAndXPathCouplets = FileAndXPathCouplets;
            this.columnCount = FileAndXPathCouplets.Length / 2;
        }

        /// <summary>
        /// CLS-compliant constructor for single file
        /// </summary>
        public XmlDataObjectCartesianAttribute(string FilePath1, string XPath1)
        {
            this.FileAndXPathCouplets = new object[] { FilePath1, XPath1 };
            this.columnCount = 1;
        }

        /// <summary>
        /// CLS-compliant constructor for two-file combination
        /// </summary>
        public XmlDataObjectCartesianAttribute(string FilePath1, string XPath1, string FilePath2, string XPath2)
        {
            this.FileAndXPathCouplets = new object[] { FilePath1, XPath1, FilePath2, XPath2 };
            this.columnCount = 2;
        }

        /// <summary>
        /// CLS-compliant constructor for three-file combination
        /// </summary>
        public XmlDataObjectCartesianAttribute(
                string FilePath1, string XPath1, string FilePath2, string XPath2, string FilePath3, string XPath3)
        {
            this.FileAndXPathCouplets = new object[] { FilePath1, XPath1, FilePath2, XPath2, FilePath3, XPath3 };
            this.columnCount = 3;
        }

        /// <summary>
        /// CLS-compliant constructor for four-file combination
        /// </summary>
        public XmlDataObjectCartesianAttribute(
                string FilePath1, string XPath1, string FilePath2, string XPath2, string FilePath3, string XPath3, 
                string FilePath4, string XPath4)
        {
            this.FileAndXPathCouplets = new object[] { 
                    FilePath1, XPath1, FilePath2, XPath2, FilePath3, XPath3, FilePath4, XPath4 };
            this.columnCount = 4;
        }

        /// <summary>
        /// CLS-compliant constructor for four-file combination
        /// </summary>
        public XmlDataObjectCartesianAttribute(
                string FilePath1, string XPath1, string FilePath2, string XPath2, string FilePath3, string XPath3, 
                string FilePath4, string XPath4, string FilePath5, string XPath5)
        {
            this.FileAndXPathCouplets = new object[] { 
                    FilePath1, XPath1, FilePath2, XPath2, FilePath3, XPath3, FilePath4, XPath4, FilePath5, XPath5 };
            this.columnCount = 5;
        }


        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            var invoker = new FixtureMemberInvoker<IEnumerable>(factoryType, scope, factoryMethodName);
            // Get List-of-Lists of Eleemnts
            var listOfListsOfElements = BuildListOfNodeLists(codeElement);

            // Use Gallio's invoker object to execute factory method
            var parameters = new object[] { listOfListsOfElements };
            var dataSet = new FactoryDataSet(() => invoker.Invoke(parameters), kind, columnCount);
            dataSource.AddDataSet(dataSet);
        }


        #region functionality which cycles through the list of passed parameters and builds Lists-of-Lists of Elements
        /// <summary>
        /// Use the Gallio's plumbing to locate Files
        /// </summary>
        private XDocument OpenXDocument(string FilePath, ICodeElementInfo codeElement)
        {
            using (TextReader reader = new ContentFile(FilePath).OpenTextReader())
            {
                return XDocument.Load(reader);
            }
        }

        /// <param name="codeElement">Gallio parameter passed by PopulateDataSource for identifying assembly manifest</param>
        /// <summary>List of XElements in the path</summary>
        private List<List<XElement>> BuildListOfNodeLists(ICodeElementInfo codeElement)
        {
            // Build an array of all the Xml Documents in memory
            int NumberOfCouplets = FileAndXPathCouplets.Length / 2;
            List<List<XElement>> ListOfNodeLists = new List<List<XElement>>();

            for (int Counter = 0; Counter < NumberOfCouplets; Counter++)
            {
                // Retreive the next couplet of File and XPath
                string FilePath = FileAndXPathCouplets[Counter * 2].ToString();
                string XPath = FileAndXPathCouplets[Counter * 2 + 1].ToString();

                ListOfNodeLists.Add(LoadXPathNodeList(codeElement, FilePath, XPath));
            }

            return ListOfNodeLists;
        }

        /// <param name="codeElement">Gallio Code Element for identifying assembly manifest</param>
        /// <param name="FilePath"></param>
        /// <param name="XPath">XPath of the one-to-many nodes</param>
        /// <returns>List of XElements in the path</returns>
        private List<XElement> LoadXPathNodeList(ICodeElementInfo codeElement, string FilePath, string XPath)
        {
            XDocument xdocument = OpenXDocument(FilePath, codeElement);
            List<XNode> XPathNodeList = xdocument.XPathSelectElements(XPath).ToList<XNode>();

            // Are any of the child nodes not XElements..?
            if (XPathNodeList.Any<XNode>(x => x.GetType() != typeof(XElement)))
            {
                throw new ArgumentException("only Element nodes are allowed in the XPath");
            }

            return XPathNodeList.Cast<XElement>().ToList<XElement>();
        }
        #endregion

    }
}

