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
using System.Data;
using System.Linq;
using System.Text;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Common.Markup;
using Gallio.Framework;
using Gallio.Framework.Data.DataObjects;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;


namespace MbUnit.Framework.Tests
{
    [TestFixture]
    [TestsOn(typeof(FlatFileToDataTableBuilder))]
    public class TabularFactoryDataTests : BaseTestWithSampleRunner
    {
        #region Plain vanilla, filter-free tests
        private readonly string[] TestDataSet1 = new string[]
        {
            "Name = 'John Smith', Address = '123  Test  Street', City = 'New York'",
            "Name = 'Patricia Doe', Address = '72 North Avenue', City = 'Chicago'",
            "Name = 'Allen Watts', Address = '444 Unit Boulevard', City = 'Oakland'"
        };

        private List<string> EmitDynamicToDataTableToListOfStringsVanillaTest(DataTable dataTable)
        {
            List<string> dataoutput = new List<string>();
            foreach (dynamic output in
                TabularDataObjectFactory.EmitDynamicObjectFromDataTable(dataTable, null, null, null, null))
            {
                dataoutput.Add(String.Format("Name = '{0}', Address = '{1}', City = '{2}'",
                                            output.Name, output.Address, output.City));
            }
            return dataoutput;
        }

        [Test]
        public void LoadCsvFile()
        {
            List<string> dataoutput = EmitDynamicToDataTableToListOfStringsVanillaTest(
                FlatFileToDataTableBuilder.BuildFromCsvFile(@"..\Framework\Data\DataObjects\TestXlsData.csv"));

            Assert.AreElementsEqual(TestDataSet1, dataoutput);
        }

        [Test]
        public void LoadTabDelimitedFile()
        {
            List<string> dataoutput = EmitDynamicToDataTableToListOfStringsVanillaTest(
                FlatFileToDataTableBuilder.BuildFromTabFile(@"..\Framework\Data\DataObjects\TestXlsData.txt"));

            Assert.AreElementsEqual(TestDataSet1, dataoutput);
        }
        #endregion

        #region Quotes, Commas and funky characters
        [Test]
        public void LoadCsvFileWithQuotes()
        {
            DataTable dataTable = FlatFileToDataTableBuilder.BuildFromCsvFile(@"..\Framework\Data\DataObjects\TestFileWithQuotes.csv");

            List<string> dataoutput = new List<string>();
            foreach (dynamic output in
                TabularDataObjectFactory.EmitDynamicObjectFromDataTable(dataTable, null, null, null, null))
            {
                dataoutput.Add(String.Format("UserName = '{0}', Column_With_Quotes_1 = '{1}'",
                            output.UserName, output.Column_With_Quotes_1));
            }

            Assert.AreElementsEqual(new [] {
                "UserName = 'admin', Column_With_Quotes_1 = '\"This is some test data's stuff\"'",
                "UserName = 'admin', Column_With_Quotes_1 = '\"The stuff\"'",
                "UserName = 'admin', Column_With_Quotes_1 = 'More test data \" \" \" '  '  '  \"'"}, 
                dataoutput); 
        }

        [Test]
        public void LoadCsvFileWithCommas()
        {
            DataTable dataTable = FlatFileToDataTableBuilder.BuildFromCsvFile(@"..\Framework\Data\DataObjects\TestFileWithCommas.csv");

            List<string> dataoutput = new List<string>();
            foreach (dynamic output in
                TabularDataObjectFactory.EmitDynamicObjectFromDataTable(dataTable, null, null, null, null))
            {
                dataoutput.Add(String.Format("Plenty_of_Commas_Just_For_You = '{0}', UserName = '{1}'",
                            output.Plenty_of_Commas_Just_For_You, output.UserName));
            }

            Assert.AreElementsEqual(new[] {
                "Plenty_of_Commas_Just_For_You = 'This,is,too,many,commas,man!', UserName = 'admin1'",
                "Plenty_of_Commas_Just_For_You = 'This,is,too,many,commas,man!', UserName = 'admin2'",
                "Plenty_of_Commas_Just_For_You = 'More commas,,,,,,,,,,,,,,,,,,,,,,,', UserName = 'admin3'" },
                dataoutput);
        }
        #endregion

        #region Tabular Data with Filtering
        [Test]
        public void LoadFilesWithFilteringCsv()
        {
            TestLog.WriteLine("\n\nCSV file\n========");
            DataTable dataTable2 = FlatFileToDataTableBuilder.BuildFromCsvFile(@"..\Framework\Data\DataObjects\TestXlsData.csv");
            TestingHelper(dataTable2);
        }

        [Test]
        public void LoadFilesWithFilteringTxt()
        {
            TestLog.WriteLine("\n\nTXT file\n========");
            DataTable dataTable3 = FlatFileToDataTableBuilder.BuildFromTabFile(@"..\Framework\Data\DataObjects\TestXlsData.txt");
            TestingHelper(dataTable3);
        }

        public void TestingHelper(DataTable dataTable)
        {
            // Tests the Row Index Inclusion filtering
            List<string> TestOutputForInclusionByIndex = new List<string>();
            TestLog.WriteLine("Including rows with index 2 and 3:\n");
            foreach (dynamic output in 
                    TabularDataObjectFactory.EmitDynamicObjectFromDataTable(dataTable, new int[] { 2, 3 }, null, null, null))
            {
                TestOutputForInclusionByIndex.Add(output.Name);
            }
            Assert.AreElementsEqual(new[] { "John Smith",  "Patricia Doe" }, TestOutputForInclusionByIndex);

            // Tests the Row Index Exclusion filtering
            List<string> TestOutputForExclusionByIndex = new List<string>();
            foreach (dynamic output in
                    TabularDataObjectFactory.EmitDynamicObjectFromDataTable(dataTable, null, new int[] { 2, 3 }, null, null))
            {
                TestOutputForExclusionByIndex.Add(output.Name);
            }
            Assert.AreElementsEqual(new[] { "Allen Watts" }, TestOutputForExclusionByIndex);

            // Tests the Inclusion By Value filtering
            List<string> TestOutputForInclusionByValue = new List<string>();
            foreach (dynamic output in 
                        TabularDataObjectFactory.EmitDynamicObjectFromDataTable(
                            dataTable, null, null, new string[] { "John Smith", "Allen Watts" }, null))
            {
                TestOutputForInclusionByValue.Add(output.Name);
            }
            Assert.AreElementsEqual(new[] { "John Smith", "Patricia Doe" }, TestOutputForInclusionByIndex);
            
            // Tests the Exclusion By Value filtering
            List<string> TestOutputForExclusionByValue = new List<string>();
            foreach (dynamic output in
                        TabularDataObjectFactory.EmitDynamicObjectFromDataTable(
                            dataTable, null, null, null, new string[] { "John Smith", "Allen Watts" }))
            {
                TestOutputForExclusionByValue.Add(output.Name);
            }
            Assert.AreElementsEqual(new[] { "Patricia Doe" }, TestOutputForExclusionByValue);
        }
        #endregion
    }
}

