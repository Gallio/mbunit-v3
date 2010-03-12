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
using System.Collections.Generic;
using System.Windows.Forms;
using SortOrder = Gallio.Icarus.Models.SortOrder;

namespace Gallio.Icarus.TestResults
{
    public class TestStepComparer : IComparer<ListViewItem>
    {
        public const int NO_SORT = -1;
        private const int DURATION_COLUMN = 2;
        private const int ASSERT_COUNT_COLUMN = 3;

        public int SortColumn { get; set; }
        public SortOrder SortOrder { get; set; }

        public TestStepComparer()
        {
            SortColumn = NO_SORT;
        }

        public int Compare(ListViewItem x, ListViewItem y)
        {
            var leftText = x.SubItems[SortColumn].Text;
            var rightText = y.SubItems[SortColumn].Text;

            var result = CompareItem(leftText, rightText);

            if (SortOrder == SortOrder.Descending)
                return -result;
                
            return result;
        }

        private int CompareItem(string leftText, string rightText)
        {
            int result;
            switch (SortColumn)
            {
                case DURATION_COLUMN:
                    result = CompareAsDouble(leftText, rightText);
                    break;

                case ASSERT_COUNT_COLUMN:
                    result = CompareAsInt(leftText, rightText);
                    break;

                default:
                    result = leftText.CompareTo(rightText);
                    break;
            }
            return result;
        }

        private static int CompareAsInt(string leftText, string rightText)
        {
            var left = Convert.ToInt32(leftText);
            var right = Convert.ToInt32(rightText);
            return left.CompareTo(right);
        }

        private static int CompareAsDouble(string leftText, string rightText)
        {
            var left = Convert.ToDouble(leftText);
            var right = Convert.ToDouble(rightText);
            return left.CompareTo(right);
        }
    }
}


