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
using System.Text;
using GenericParsing;

namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// Builder class parses comma-separated and tab-delimited files and 
    /// returns ADO.NET DataTable objects
    /// 
    /// NOTE: this is wholly dependent upon the extremely useful GenericParsing library 
    /// version Version 1.1.1.20400, which was created by Andrew Rissing.
    /// 
    /// http://www.codeproject.com/KB/database/GenericParser.aspx
    /// </summary>
    public class FlatFileToDataTableBuilder
    {
        /// <summary>
        /// Parses CSV file and returns DataTable object with the entire file
        /// </summary>
        public static DataTable BuildFromCsvFile(string FileName)
        {
            using (GenericParsing.GenericParserAdapter parser = CsvParserFactory(FileName))
            {
                return parser.GetDataTable();
            }
        }

        /// <summary>
        /// Parses tab-delimited file and returns DataTable object with the entire file
        /// </summary>
        public static DataTable BuildFromTabFile(string FileName)
        {
            using (GenericParsing.GenericParserAdapter parser = TabDelimitedParserFactory(FileName))
            {
                return parser.GetDataTable();
            }
        }

        /// <summary>
        /// Centralizes creation of parsers with all the properties for CSV files
        /// </summary>
        private static GenericParserAdapter CsvParserFactory(string FileName)
        {
            GenericParserAdapter parser = new GenericParserAdapter();
            parser.SetDataSource(FileName);
            parser.FirstRowHasHeader = true;
            parser.FirstRowSetsExpectedColumnCount = true;
            return parser;
        }

        /// <summary>
        /// Centralizes creation of parsers with all the properties for Tab-delimited fiels
        /// </summary>
        private static GenericParserAdapter TabDelimitedParserFactory(string FileName)
        {
            GenericParserAdapter parser = new GenericParserAdapter();
            parser.SetDataSource(FileName);
            parser.ColumnDelimiter = '\t';
            parser.FirstRowHasHeader = true;
            parser.FirstRowSetsExpectedColumnCount = true;
            return parser;
        }
    }
}
