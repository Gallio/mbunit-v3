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
using System.IO;
using System.Windows.Forms;
using Gallio.Common;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;

namespace Gallio.UI.Reports
{
    /// <summary>
    /// <para>
    /// Displays a summary of a set of test step runs.
    /// </para>
    /// <para>
    /// This control is optimized to display individual test run results to the user on
    /// demand more quickly than could be done if we had to show the whole report at once.
    /// </para>
    /// </summary>
    public partial class TestStepRunViewer : UserControl
    {
        private HtmlTestStepRunFormatter formatter;
        private volatile FileInfo htmlFile;

        /// <summary>
        /// Creates a test step run viewer.
        /// </summary>
        public TestStepRunViewer()
        {
            InitializeComponent();

            Disposed += HandleDisposed;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateAsync();
        }

        /// <summary>
        /// Clears the contents of the report viewer and discards all cached content.
        /// </summary>
        public void Clear()
        {
            ClearNoUpdate();
            UpdateAsync();
        }

        private void ClearNoUpdate()
        {
            if (formatter != null)
                formatter.Clear();

            htmlFile = null;
        }

        /// <summary>
        /// Displays information about a set of test step run.
        /// </summary>
        /// <param name="testStepRuns">The test step runs.</param>
        public void Show(ICollection<TestStepRun> testStepRuns)
        {
            Show(testStepRuns, null);
        }

        /// <summary>
        /// Displays information about a set of test step runs, using additional
        /// information from the test model when available.
        /// </summary>
        /// <param name="testStepRuns">The test step runs.</param>
        /// <param name="testModelData">The test model data, or null if not available.</param>
        public void Show(ICollection<TestStepRun> testStepRuns, TestModelData testModelData)
        {
            if (testStepRuns == null || testStepRuns.Contains(null))
                throw new ArgumentNullException("testStepRuns");

            if (testStepRuns.Count == 0)
            {
                ClearNoUpdate();
            }
            else
            {
                EnsureFormatter();
                htmlFile = formatter.Format(testStepRuns, testModelData);
            }

            UpdateAsync();
        }

        private void HandleDisposed(object sender, EventArgs e)
        {
            ClearNoUpdate();
        }

        private void UpdateAsync()
        {
            DoAsync(() =>
            {
                if (webBrowser.IsBusy)
                    webBrowser.Stop();

                var cachedHtmlFile = htmlFile; // in case it changes concurrently

                if (cachedHtmlFile != null)
                {
                    webBrowser.Url = new Uri(cachedHtmlFile.FullName);
                    webBrowser.Show();
                }
                else
                {
                    webBrowser.Url = null;
                }
            });
        }

        private void DoAsync(Action action)
        {
            if (InvokeRequired)
            {
                BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        private void EnsureFormatter()
        {
            lock (this)
            {
                if (formatter == null)
                    formatter = new HtmlTestStepRunFormatter();
            }
        }
    }
}