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
using Ado = System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;


namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// Static methods for Building DataTable from an OpenXml spreadsheet file.
    /// Operates under the assumption that the data is in a tabular format.
    /// </summary>
    public class OpenXmlExcelToDataTableBuilder
    {
        /// <param name="XlsxFilePath">Location of Excel 2007 or newer file</param>
        /// <param name="WorksheetName">Worksheet to be used as Data Source</param>
        /// <returns>DataTable loaded with Excel data</returns>
        public static Ado::DataTable Build(string XlsxFilePath, string WorksheetName)
        {
            // Unzip the files
            string UnzipDirectory = TargetUnzipDirectory(XlsxFilePath);
            CleanDirectory(UnzipDirectory);
            UnzipFile(XlsxFilePath, UnzipDirectory);

            // Create the file paths
            string SharedStringFilePath = UnzipDirectory + @"\xl\sharedStrings.xml";
            string WorkbookFilePath = UnzipDirectory + @"\xl\workbook.xml";
            string WorkSheetFilePath;

            // Load the Workbook and get the sheet name
            dynamic Workbook = XmlDataObjectBuilder.EmitDataObject(WorkbookFilePath);
            dynamic sheet = XmlDataObjectUtility.FindFirstElementByAttribute(Workbook.sheets.sheet, "name", WorksheetName);

            if (sheet == null)
                throw new ArgumentException("Unable to locate Worksheet: " + WorksheetName);
            else
                WorkSheetFilePath = UnzipDirectory + @"\xl\worksheets\sheet" + sheet.id.Replace("rId", "") + ".xml";

            // Load the Worksheet into an XmlDataObject
            dynamic Worksheet = XmlDataObjectBuilder.EmitDataObject(WorkSheetFilePath);

            // Build the Shared String Table
            dynamic StringTable = XmlDataObjectBuilder.EmitDataObject(SharedStringFilePath);
            List<string> StringTableList = new List<string>();
            foreach (dynamic si in StringTable.si)
                StringTableList.Add(si.t.Value);

            // Create Data Table and load Columns from first row
            Ado::DataTable OutputDataTable = new Ado::DataTable();

            dynamic WorksheetRows = XmlDataObject.AsList(Worksheet.sheetData.row);

            foreach (dynamic ColumnName in ExtractWorksheetRow(StringTableList, WorksheetRows[0]))
                OutputDataTable.Columns.Add(ColumnName);

            // Load the Rows
            for (int WorksheetRowIndex = 1; WorksheetRowIndex < WorksheetRows.Count; WorksheetRowIndex++)
            {
                Ado::DataRow NewRow = OutputDataTable.NewRow();
                List<string> WorksheetRow = ExtractWorksheetRow(StringTableList, WorksheetRows[WorksheetRowIndex]);

                for (int Column = 0; Column < WorksheetRow.Count; Column++)
                {
                    NewRow[Column] = WorksheetRow[Column];
                }

                OutputDataTable.Rows.Add(NewRow);
            }

            // Clean up
            CleanDirectory(UnzipDirectory);

            return OutputDataTable;
        }

        /// <summary>
        /// Helper method uses OpenXml String, parses dynamic representation of Xml Data row into a 
        /// List of strings, based on OpenXml file format rules
        /// </summary>
        private static List<string> ExtractWorksheetRow(List<string> StringTable, dynamic row)
        {
            List<string> output = new List<string>();

            foreach (dynamic cell in row.c)
            {
                if (cell.t == "s")
                {
                    int StringTableIndex = Int32.Parse(cell.v.Value);
                    output.Add(StringTable[StringTableIndex]);
                }
                if (cell.t == null)
                {
                    output.Add(cell.v.Value);
                }
            }

            return output;
        }

        /// <summary>
        /// Deterministically generates target directory into which to unzip OpenXml spreadsheet files.
        /// </summary>
        private static string TargetUnzipDirectory(string FileName)
        {
            return Environment.CurrentDirectory + @"\" + Path.GetFileName(FileName) + "_unzip";
        }

        /// <summary>
        /// Extracts zip file into specified directory
        /// </summary>
        protected static void UnzipFile(string zipFileName, string targetDirectory)
        {
            (new FastZip()).ExtractZip(zipFileName, targetDirectory, null);
        }

        /// <summary>
        /// Clears out contents of directory
        /// </summary>
        protected static void CleanDirectory(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            else
            {
                foreach (FileInfo currFile in di.GetFiles())
                    currFile.Delete();

                foreach (DirectoryInfo currDir in di.GetDirectories())
                    currDir.Delete(true);
            }
        }
    }
}
