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
using System.Drawing;
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.Logging;

namespace Gallio.Icarus
{
    internal partial class RuntimeLogWindow : DockWindow
    {
        public RuntimeLogWindow(IRuntimeLogController runtimeLogController)
        {
            InitializeComponent();

            if (severityComboBox.ComboBox != null)
            {
                severityComboBox.ComboBox.DataSource = Enum.GetValues(typeof(LogSeverity));
                severityComboBox.ComboBox.DataBindings.Add("SelectedItem", runtimeLogController, "MinLogSeverity", 
                    false, DataSourceUpdateMode.OnPropertyChanged);
            }

            runtimeLogController.LogMessage += runtimeLogController_LogMessage;
        }

        void runtimeLogController_LogMessage(object sender, Controllers.EventArgs.RuntimeLogEventArgs e)
        {
            BeginInvoke((MethodInvoker)(() => AppendTextLine(e.Message, e.Color)));
        }

        private void AppendTextLine(string text, Color color)
        {
            AppendText(text, color);
            AppendText("\n", color);
        }

        private void AppendText(string text, Color color)
        {
            var start = logBody.TextLength;
            logBody.AppendText(text);
            var end = logBody.TextLength;
           
            // Textbox may transform chars, so (end-start) != text.Length
            logBody.Select(start, end - start);
            {
                logBody.SelectionColor = color;
                // could set box.SelectionBackColor, box.SelectionFont too.
            }
            // clear selection
            logBody.SelectionLength = 0;
        }

        private void clearAllToolStripButton_Click(object sender, EventArgs e)
        {
            logBody.Clear();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logBody.SelectAll();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(logBody.SelectedText);
        }
    }
}
