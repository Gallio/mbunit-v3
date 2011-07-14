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
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// Builder class enables consumers to easily get DataTables from Excel worksheets
    /// </summary>
    public class ExcelToDataTableBuilder
    {
        private const string ConnectionStringTemplate = 
                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;";

        // OleDb excel worksheet query text
        private const string SelectWorksheetQueryTemplate = "SELECT * FROM [{0}$]";


        /// <summary>
        /// Opens an Excel file and imports worksheet into a DataTable.  Worksheet Name is required.
        /// </summary>
        public static DataTable Build(string FilePath, string WorksheetName)
        {
            if (Path.GetExtension(FilePath) == ".xls")
            {
                // If anything goes wrong, "using" will force disposal of the connection to the file
                using (OleDbConnection conn = BuildExcelConnection(FilePath))
                {
                    // "Connect" to the XLS file
                    conn.Open();

                    // Get a DataAdapter to the query
                    string ExcelQuery = String.Format(SelectWorksheetQueryTemplate, WorksheetName);
                    OleDbDataAdapter Adapter = new OleDbDataAdapter(ExcelQuery, conn);

                    // Populate DataTable using DataAdapter
                    DataTable dataTable = new DataTable();
                    Adapter.Fill(dataTable);

                    // Close the connection
                    conn.Close();

                    // Finished
                    return dataTable;
                }
            }
            if (Path.GetExtension(FilePath) == ".xlsx")
            {
                return OpenXmlExcelToDataTableBuilder.Build(FilePath, WorksheetName);
            }

            throw new ArgumentException("Invalid file extension specified on Excel data file:" + FilePath);
        }

        /// <summary>
        /// Returns an OleDb connection to an Excel file
        /// </summary>
        private static OleDbConnection BuildExcelConnection(string FilePath)
        {
            string ConnectionString = String.Format(ConnectionStringTemplate, FilePath);
            OleDbConnection conn = new OleDbConnection(ConnectionString);
            return conn;
        }
    }
}
