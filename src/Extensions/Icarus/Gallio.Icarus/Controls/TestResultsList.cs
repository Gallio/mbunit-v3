// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Windows.Forms;

using Gallio.Model;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Controls
{
    public class TestResultsList : ListView
    {
        protected readonly TestResultsListColumnSorter columnSorter;

        public TestResultsList()
        {
            columnSorter = new TestResultsListColumnSorter();
            ListViewItemSorter = columnSorter;

            ColumnHeader StepName = new ColumnHeader();
            ColumnHeader Duration = new ColumnHeader();
            ColumnHeader CodeReference = new ColumnHeader();
            ColumnHeader Assembly = new ColumnHeader();
            ColumnHeader TestKind = new ColumnHeader();
            ColumnHeader Asserts = new ColumnHeader();
            StepName.Text = "Step name";
            StepName.Width = 200;
            TestKind.Text = "Test kind";
            TestKind.Width = 65;
            Duration.Text = "Duration (s)";
            Duration.Width = 70;
            Asserts.Text = "Asserts";
            Asserts.Width = 50;
            CodeReference.Text = "Code reference";
            CodeReference.Width = 200;
            Assembly.Text = "Assembly";
            Assembly.Width = 200;
            Columns.AddRange(new[] { StepName, TestKind, Duration, Asserts, CodeReference, Assembly});
            FullRowSelect = true;
            View = View.Details;
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            base.OnColumnClick(e);

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == columnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                columnSorter.Order = columnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                columnSorter.SortColumn = e.Column;
                columnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            Sort();
        }

        public void AddTestStepRun(string testKind, TestStepRun testStepRun, int indentCount)
        {
            // get the appropriate icon based on outcome
            int imgIndex = -1;
            switch (testStepRun.Result.Outcome.Status)
            {
                case TestStatus.Failed:
                    imgIndex = 1;
                    break;
                case TestStatus.Inconclusive:
                    imgIndex = 2;
                    break;
                case TestStatus.Passed:
                    imgIndex = 0;
                    break;
            }
            // convert the test step run information to a format for display as a list view item
            string duration = testStepRun.Result.Duration.ToString("0.000");
            string assertCount = testStepRun.Result.AssertCount.ToString();
            string codeReference = testStepRun.Step.CodeReference.TypeName ?? string.Empty;
            string assemblyName = testStepRun.Step.CodeReference.AssemblyName ?? string.Empty;
            ListViewItem listViewItem = CreateListViewItem(testStepRun.Step.Name, imgIndex, testKind, duration, assertCount, 
                codeReference, assemblyName, indentCount);
            Items.Add(listViewItem);
        }

        private ListViewItem CreateListViewItem(string name, int imgIndex, string testKind, string duration, string assertCount, 
            string codeReference, string assemblyName, int indentCount)
        {
            ListViewItem lvi = new ListViewItem(name, imgIndex);
            lvi.SubItems.AddRange(new[] { testKind, duration, assertCount, codeReference, assemblyName });
            if (columnSorter.SortColumn == 0)
                lvi.IndentCount = indentCount;
            return lvi;
        }

        public new void Clear()
        {
            Items.Clear();
        }
    }

    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    public class TestResultsListColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private readonly CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public TestResultsListColumnSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;

            // Cast the objects to be compared to ListViewItem objects
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;

            // Compare the two items
            switch (ColumnToSort)
            {
                case 0: // outcome
                    compareResult = listviewX.ImageIndex.CompareTo(listviewY.ImageIndex);
                    break;
                case 3: // assert count
                case 2: // duration
                {
                    decimal left = Convert.ToDecimal(listviewX.SubItems[ColumnToSort].Text);
                    decimal right = Convert.ToDecimal(listviewY.SubItems[ColumnToSort].Text);
                    if (left < right)
                        compareResult = 1;
                    else
                    {
                        if (left > right)
                            compareResult = -1;
                        else
                            compareResult = 0;
                    }
                }
                    break;
                default: // text
                    compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
                    break;
            }

            // Calculate correct return value based on object comparison
            if (OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            if (OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            // Return '0' to indicate they are equal
            return 0;
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }
    }
}
