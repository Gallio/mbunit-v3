using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Gallio.Common;
using Gallio.Framework;
using Gallio.Framework.Data.DataObjects;
using MbUnit.Framework;

namespace MbUnit.Framework.Tests
{
    [TestFixture]
    public class TabularDataAttributeWithFilteringTests
    {
        ///<summary>
        /// This demonstrates the application of the Row Inclusion filters.
        /// Simply pass a new integer array to the named-parameter RowIndexIncludeFilterArray.
        /// It will only run tests for Rows numer 2 and 3.
        /// The filters apply to the FlatFileDataObject and ExcelDataObject.
        /// 
        /// IMPORTANT NOTE: the first Row of actual data starts at Index == "2".  This is because
        /// the Column Headers are stored at "1".
        /// 
        /// In this test, we're only including rows number 2 and 3 -- the first and second
        /// rows of data respectively.
        /// </summary>
        [Test]
        [FlatFileDataObject(@"..\Framework\TestXlsData.csv", TabularDataFileType.CsvFile,
                            RowIndexIncludeFilterArray = new int[] { 2, 3 })]
        public void RowInclusionFilteredCsvData(dynamic RowOfTestData)
        {
            TestLog.WriteLine("UserName through the DLR ==> " + RowOfTestData.UserName);
        }
        
        /// <summary>
        /// Demonstrates usage with a Tab-delimited file.
        /// 
        /// In this test, we're only including rows number 2 and 3 -- the first and second
        /// rows of data respectively.
        /// </summary>
        [Test]
        [FlatFileDataObject(@"..\Framework\TestXlsData.txt", TabularDataFileType.TabFile,
                            RowIndexIncludeFilterArray = new int[] { 2, 3 })]
        public void RowInclusionFilteredTabFileData(dynamic RowOfTestData)
        {
            TestLog.WriteLine("UserName through the DLR ==> " + RowOfTestData.UserName);
        }

        /// <summary>
        /// Demonstrates usage with Excel file.
        /// 
        /// In this test, we're only including row numbers 2 and 3 -- the first and second
        /// rows of data respectively.
        /// </summary>
        [Test]
        [ExcelDataObject(@"..\Framework\TestXlsData.xlsx", "Worksheet1",
                            RowIndexIncludeFilterArray = new int[] { 2, 3 })]
        public void RowInclusionFilteredExcelData(dynamic RowOfTestData)
        {
            TestLog.WriteLine(RowOfTestData.ToStringWithNewLines());
            TestLog.WriteLine("RowOfTestData.Name ==> " + RowOfTestData.Name);
            TestLog.WriteLine("RowOfTestData.Address ==> " + RowOfTestData.Address);
            TestLog.WriteLine("RowOfTestData.City ==> " + RowOfTestData.City);
            TestLog.WriteLine("RowOfTestData.State ==> " + RowOfTestData.State);
            TestLog.WriteLine("RowOfTestData.Url ==> " + RowOfTestData.Url);
            TestLog.WriteLine("RowOfTestData.Degree ==> " + RowOfTestData.Degree);
            TestLog.WriteLine("RowOfTestData.Martial_Art ==> " + RowOfTestData.Martial_Art);
        }

        /// <summary>
        /// Row Exclusion filters - they do the inverse of Row Inclusion.
        /// 
        /// In this test, we're include ALL rows EXCEPT for row numbers 2 and 3.
        /// </summary>
        [Test]
        [FlatFileDataObject(@"..\Framework\TestXlsData.csv", TabularDataFileType.CsvFile,
                            RowIndexExcludeFilterArray = new int[] { 2, 3 })]
        public void RowExclusionFilteredData(dynamic RowOfTestData)
        {
            TestLog.WriteLine(RowOfTestData.ToStringWithNewLines());
            TestLog.WriteLine("RowOfTestData.Name ==> " + RowOfTestData.Name);
            TestLog.WriteLine("RowOfTestData.Address ==> " + RowOfTestData.Address);
            TestLog.WriteLine("RowOfTestData.City ==> " + RowOfTestData.City);
            TestLog.WriteLine("RowOfTestData.State ==> " + RowOfTestData.State);
            TestLog.WriteLine("RowOfTestData.Url ==> " + RowOfTestData.Url);
            TestLog.WriteLine("RowOfTestData.Degree ==> " + RowOfTestData.Degree);
            TestLog.WriteLine("RowOfTestData.Martial_Art ==> " + RowOfTestData.Martial_Art);
        }

        /// <summary>
        /// Cell Value Inclusion Filtering - will only include rows if they have any of the
        /// string values found in the Filter Array.
        /// 
        /// In this test, we're only including every row that which contain at least one cell 
        /// with the values "John Smith" or "Allen Watts".
        /// </summary>
        [Test]
        [FlatFileDataObject(@"..\Framework\TestXlsData.csv", TabularDataFileType.CsvFile,
                            CellValueIncludeFilterArray = new string[] { "John Smith", "Allen Watts" })]
        public void CellValueInclusionFilteredData(dynamic RowOfTestData)
        {
            TestLog.WriteLine(RowOfTestData.ToStringWithNewLines());
            TestLog.WriteLine("RowOfTestData.Name ==> " + RowOfTestData.Name);
            TestLog.WriteLine("RowOfTestData.Address ==> " + RowOfTestData.Address);
            TestLog.WriteLine("RowOfTestData.City ==> " + RowOfTestData.City);
            TestLog.WriteLine("RowOfTestData.State ==> " + RowOfTestData.State);
            TestLog.WriteLine("RowOfTestData.Url ==> " + RowOfTestData.Url);
            TestLog.WriteLine("RowOfTestData.Degree ==> " + RowOfTestData.Degree);
            TestLog.WriteLine("RowOfTestData.Martial_Art ==> " + RowOfTestData.Martial_Art);
        }

        /// <summary>
        /// Cell Value Exclusion Filtering - will exclude rows if they have any of the
        /// string values found in the Filter Array.
        /// 
        /// In this test, we're including ALL rows EXCEPT for the ones that contain at least one
        /// cell with the values "John Smith" or "Allen Watts".
        /// </summary>
        [Test]
        [FlatFileDataObject(@"..\Framework\TestXlsData.csv", TabularDataFileType.CsvFile,
                            CellValueExcludeFilterArray = new string[] { "John Smith", "Allen Watts" })]
        public void CellValueExclusionFilteredData(dynamic RowOfTestData)
        {
            TestLog.WriteLine(RowOfTestData.ToStringWithNewLines());
            TestLog.WriteLine("RowOfTestData.Name ==> " + RowOfTestData.Name);
            TestLog.WriteLine("RowOfTestData.Address ==> " + RowOfTestData.Address);
            TestLog.WriteLine("RowOfTestData.City ==> " + RowOfTestData.City);
            TestLog.WriteLine("RowOfTestData.State ==> " + RowOfTestData.State);
            TestLog.WriteLine("RowOfTestData.Url ==> " + RowOfTestData.Url);
            TestLog.WriteLine("RowOfTestData.Degree ==> " + RowOfTestData.Degree);
            TestLog.WriteLine("RowOfTestData.Martial_Art ==> " + RowOfTestData.Martial_Art);
        }
    }
}
