// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace Gallio.Icarus
{
    public partial class LogWindow : DockWindow
    {
        public string LogBody
        {
            get { return logBody.Text; }
        }

        public LogWindow()
        {
            InitializeComponent();
        }

        public LogWindow(string text) : this()
        {
            Text = text;
        }

        public void AppendText(string text)
        {
            logBody.AppendText(text);
        }

        public void AppendText(string text, Color color)
        {
            int start = logBody.TextLength;
            logBody.AppendText(text);
            int end = logBody.TextLength;
           
            // Textbox may transform chars, so (end-start) != text.Length
            logBody.Select(start, end - start);
            {
                logBody.SelectionColor = color;
                // could set box.SelectionBackColor, box.SelectionFont too.
            }
            // clear selection
            logBody.SelectionLength = 0;
        }

        public void Clear()
        {
            logBody.Clear();
        }

        private void clearAllToolStripButton_Click(object sender, EventArgs e)
        {
            logBody.Clear();
        }

        protected override string GetPersistString()
        {
            return GetType().ToString() + "," + Text;
        }
    }
}