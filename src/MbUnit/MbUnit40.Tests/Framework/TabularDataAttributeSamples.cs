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
    public class TabularDataAttributeTests
    {
        /// <summary>
        /// Usage of the attribute is straightforward - specify an excel filename and a worksheet name.
        /// Should be compatible with both Excel 97-2003 and Excel 2007+ 
        /// </summary>
        [Test]
        [ExcelDataObject(@"..\Framework\TestXlsData.xlsx", "Worksheet1")]
        public void ExcelDataObjectTest(dynamic RowOfTestData)
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
        /// Excel is sensitive to numbers and text. By default, the Excel parser will return numbers 
        /// as doubles.  The following test demonstrates how to address these issues with dynamically 
        /// generated objects.
        /// 
        /// VERY IMPORTANT NOTE: if you have numbers cells in Excel, they will be 
        /// boxed and stored as doubles and NOT text.  Be certain to check the format.
        /// </summary>
        [Test]
        [ExcelDataObject(@"..\Framework\NumbersAndText.xlsx", "Sheet1")]
        public void NumbersAndTextTest(dynamic RowOfTestData)
        {
            TestLog.WriteLine(RowOfTestData.ToStringWithNewLines());

            double NumberValue = RowOfTestData.ThisIsANumber;
            string TextValue = RowOfTestData.ThisIsText;

            TestLog.WriteLine(RowOfTestData.ThisIsANumber.GetType());
            TestLog.WriteLine(RowOfTestData.ThisIsText.GetType());

            Assert.IsTrue(NumberValue == System.Double.Parse(TextValue));
        }

        /// <summary>
        /// CSV files and tab-delimited files work, as well.  You must specify file type.
        /// </summary>
        [Test]
        [FlatFileDataObject(@"..\Framework\TestXlsData.csv", TabularDataFileType.CsvFile)]
        public void TestMethod2(dynamic RowOfTestData)
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
        /// You can decorate Test Methods with multiple attributes to bring data from different
        /// data sources.
        /// </summary>
        [Test]
        [ExcelDataObject(@"..\Framework\TestXlsData.xlsx", "Worksheet1", RowIndexIncludeFilterArray = new int[] { 1, 2, 3 })]
        [ExcelDataObject(@"..\Framework\TestXlsData.xls", "Worksheet1")]
        public void TestMethod3(dynamic RowOfTestData)
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

