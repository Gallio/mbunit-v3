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
    /// Parses CSV and tab-delimited files into DynamicObject's for data-driven tests. Column 
    /// headers in the first row are used to name the properties in the DynamicObject.
    /// This attribute can be added multiple times to the same test method.
    /// WARNING: column headings must contain valid C# variable names to enable proper usage
    /// </summary>
    /// <remarks>
    /// <para>
    /// Here's how to apply this attribute to a Gallio/MbUnit test:
    /// 
    /// [FlatFileDataObject("./TestData/XLS/TestFileWithQuotes.csv", FileType.CsvFile)]
    /// public void TestMethod2(dynamic RowOfTestData)
    /// {
    ///     TestLog.WriteLine(RowOfTestData.ToStringWithNewLines());
    ///     TestLog.WriteLine("UserName through the DLR ==> " + RowOfTestData.UserName);
    ///     TestLog.WriteLine("Password through the DLR ==> " + RowOfTestData.Password);
    /// }
    /// 
    /// [FlatFileDataObject("./TestData/XLS/CaliforniaExcel.txt", FileType.TabFile)]
    /// public void TestMethod2(dynamic RowOfTestData)
    /// {
    ///     TestLog.WriteLine(RowOfTestData.ToStringWithNewLines());
    ///     TestLog.WriteLine("Password through the DLR ==> " + RowOfTestData.TestURL);
    /// }
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class FlatFileDataObjectAttribute : TabularDataAttributeBase
    {
        // Path of the file
        private string filePath;

        // The file type which will dictory which factory method to use
        private TabularDataFileType fileType;

        /// <summary>
        /// Specify the file path and the type of file to use as a data source
        /// </summary>
        public FlatFileDataObjectAttribute(string filePath, TabularDataFileType fileType)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            this.filePath = filePath;
            this.fileType = fileType;
        }

        /// <summary>
        /// Implementation of the BuildDataTable method for this class alone.  Will be invoked by base class
        /// PopulateDataSource method.
        /// </summary>
        protected override DataTable BuildDataTable()
        {
            if (this.fileType == TabularDataFileType.CsvFile)
                return FlatFileToDataTableBuilder.BuildFromCsvFile(this.filePath);
            if (this.fileType == TabularDataFileType.TabFile)
                return FlatFileToDataTableBuilder.BuildFromTabFile(this.filePath);

            throw new NotImplementedException("File Type: " + this.fileType + " is not yet supported");
        }
    }
}

