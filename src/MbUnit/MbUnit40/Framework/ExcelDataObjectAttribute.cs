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
using System.Data;
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
    /// Parses Excel files into DynamicObject's for data-driven tests.  Column headers in
    /// the first row are used to name the properties in the DynamicObject.
    /// This attribute can be added multiple times to the same test method.
    /// WARNING: Excel worksheets that are loaded must have column headings that are valid
    /// C# variable names.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Here's how to apply this attribute to a Gallio/MbUnit test:
    /// 
    /// [Test]
    /// [ExcelDataObject("./TestData/XLS/CaliforniaExcel.xlsx", "ExamsReport")]
    /// [ExcelDataObject("./TestData/XLS/CaliforniaExcelOldSchool.xls", "ExamsReport")]
    /// public void TestMethod2(dynamic RowOfTestData)
    /// {
    ///     TestLog.WriteLine(RowOfTestData.ToStringWithNewLines());                
    ///     TestLog.WriteLine("UserName through the DLR ==> " + RowOfTestData.UserName);
    ///     TestLog.WriteLine("Password through the DLR ==> " + RowOfTestData.Password);
    ///     TestLog.WriteLine("Password through the DLR ==> " + RowOfTestData.TestURL);
    /// }
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class ExcelDataObjectAttribute : TabularDataAttributeBase
    {
        // Location of the file
        private string filePath;

        // Name of the Excel worksheet
        private string worksheetName;

        /// <summary>
        /// Specify the file path and the worksheet name to use as a data source
        /// </summary>
        public ExcelDataObjectAttribute(string filePath, string worksheetName)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");
            if (worksheetName == null)
                throw new ArgumentNullException("worksheetName");

            this.filePath = filePath;
            this.worksheetName = worksheetName;
        }

        /// <summary>
        /// Implementation of the BuildDataTable method for this class alone.  Will be invoked by base class
        /// PopulateDataSource method.
        /// </summary>
        protected override DataTable BuildDataTable()
        {
            return ExcelToDataTableBuilder.Build(this.filePath, this.worksheetName);
        }

    }
}

