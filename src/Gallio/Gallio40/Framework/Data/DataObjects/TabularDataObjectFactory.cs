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

namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// Factory method that cycles through DataTable and emits DynamicObject which contains rows.
    /// </summary>
    public class TabularDataObjectFactory
    {
        /// <summary>
        /// Enumerates through rows of DataTable and returns dynamically-generated object for each row.
        /// </summary>
        public static IEnumerable<DynamicObject> EmitDynamicObjectFromDataTable(
                DataTable dataTable, int[] RowIndexIncludeFilter, int[] RowIndexExcludeFilter, 
                string[] CellValueIncludeFilterArray, string[] CellValueExcludeFilterArray)
        {
            // Keeps track of which row is being parsed for the sake of filtering
            // The first row always contains the column names, so we start with Index == 2
            int RowIndex = 2;

            // Cycle through each row of of table data
            foreach (System.Data.DataRow dataRow in dataTable.Rows)
            {
                bool FilterReject = false;

                // Row Exclusion Filtering: exclude anything in the array argument
                if (RowIndexExcludeFilter != null)
                {
                    if (RowIndexExcludeFilter.Contains(RowIndex))
                        FilterReject = true;
                }

                // Row Inclusion Filtering: when used, only include things in the array argument
                if (RowIndexIncludeFilter != null)
                {
                    if (!RowIndexIncludeFilter.Contains(RowIndex))
                        FilterReject = true;
                }

                // Cell Value Exclusion Filtering: exclude Row which contains a Column value that
                // matches any of the array
                if (CellValueExcludeFilterArray != null)
                {
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        if (CellValueExcludeFilterArray.Contains(dataRow[dataColumn.ColumnName].ToString()))
                            FilterReject = true;
                    }
                }

                // Cell Value Inclusion Filtering: will *only* include Rows which contain at least one
                // of the values in the array argument
                if (CellValueIncludeFilterArray != null)
                {
                    bool CellValueFound = false;
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        if (CellValueIncludeFilterArray.Contains(dataRow[dataColumn.ColumnName].ToString()))
                            CellValueFound = true;
                    }
                    if (CellValueFound != true)
                        FilterReject = true;
                }

                // Was this Row rejected by any of the filters?
                if (FilterReject == false)
                {
                    // Passed all the filters... now, create a Dynamic object for each row...
                    DynamicObject OutputDataRow = new DynamicObject();

                    // ... and cycle through each column and set the appropriate members
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        OutputDataRow.TrySetMember(dataColumn.ColumnName, dataRow[dataColumn.ColumnName]);
                    }
                    yield return OutputDataRow;
                }

                ++RowIndex;
            }
        }
    }
}
