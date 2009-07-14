// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System.Windows.Forms;

namespace Gallio.Icarus.Controls
{
    public class TestResultsList : ListView
    {
        public TestResultsList()
        {
            ColumnHeader stepName = new ColumnHeader();
            ColumnHeader duration = new ColumnHeader();
            ColumnHeader codeReference = new ColumnHeader();
            ColumnHeader fileName = new ColumnHeader();
            ColumnHeader testKind = new ColumnHeader();
            ColumnHeader asserts = new ColumnHeader();

            stepName.Text = "Step name";
            stepName.Width = 200;
            testKind.Text = "Test kind";
            testKind.Width = 65;
            duration.Text = "Duration (s)";
            duration.Width = 70;
            asserts.Text = "Asserts";
            asserts.Width = 50;
            codeReference.Text = "Code reference";
            codeReference.Width = 200;
            fileName.Text = "File";
            fileName.Width = 200;
            
            Columns.AddRange(new[] { stepName, testKind, duration, asserts, codeReference, fileName });
            
            FullRowSelect = true;
            View = View.Details;
            VirtualMode = true;

            // Activate double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            SetStyle(ControlStyles.EnableNotifyMessage, true);
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
                // If the new size is smaller than the Index of TopItem, we need to make
                // sure the new TopItem is set to something smaller.
                // http://blogs.msdn.com/cumgranosalis/archive/2006/03/18/ListViewVirtualModeBugs.aspx
                if (VirtualMode && View == View.Details && TopItem != null && value > 0 && TopItem.Index > value - 1)
                    TopItem = Items[value - 1];
                base.VirtualListSize = value;
            }
        }
    }
}
