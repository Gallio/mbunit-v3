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
using System.Windows.Forms;
using Gallio.Icarus.Properties;

namespace Gallio.Icarus.Controls
{
    public class TestResultsList : ListView
    {
        public TestResultsList()
        {             
            AddColumns();

            FullRowSelect = true;
            View = View.Details;
            VirtualMode = true;

            // Activate double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        private void AddColumns()
        {
            var stepName = new ColumnHeader
            {
                Text = Resources.TestResultsList_TestResultsList_Step_name,
                Width = 200
            };
            var testKind = new ColumnHeader
            {
                Text = Resources.TestResultsList_TestResultsList_Test_kind,
                Width = 65
            };
            var duration = new ColumnHeader
            {
                Text = Resources.TestResultsList_TestResultsList_Duration__s_,
                Width = 70
            };
            var asserts = new ColumnHeader
            {
                Text = Resources.TestResultsList_TestResultsList_Asserts,
                Width = 50
            };
            var codeReference = new ColumnHeader
            {
                Text = Resources.TestResultsList_TestResultsList_Code_reference,
                Width = 200
            };
            var fileName = new ColumnHeader
            {
                Text = Resources.TestResultsList_TestResultsList_File,
                Width = 200
            };

            Columns.AddRange(new[]
                {
                    stepName, testKind, duration, asserts, codeReference, fileName
                });
        }

        protected override void OnNotifyMessage(Message m)
        {
            // Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
                base.OnNotifyMessage(m);
        }

        /// <summary>
        /// The size of the VirtualListSize
        /// </summary>
        public new int VirtualListSize
        {
            get
            {
                return base.VirtualListSize;
            }
            set
            {
                try
                {
                    // If the new size is smaller than the Index of TopItem, we need to make
                    // sure the new TopItem is set to something smaller.
                    // http://blogs.msdn.com/cumgranosalis/archive/2006/03/18/ListViewVirtualModeBugs.aspx
                    if (VirtualMode && View == View.Details && value > 0 && TopItem != null && TopItem.Index > value - 1)
                    {
                        TopItem = Items[value - 1];
                    }
                    base.VirtualListSize = value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
