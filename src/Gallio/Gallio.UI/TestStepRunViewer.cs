using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.UI
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
        private volatile MemoryStream stream;
        private bool initialized;

        /// <summary>
        /// Creates a test step run viewer.
        /// </summary>
        public TestStepRunViewer()
        {
            InitializeComponent();

            Disposed += HandleDisposed;
            Paint += HandleWebBrowserInit;
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

            stream = null;
        }

        /// <summary>
        /// Displays information about a set of test step run.
        /// </summary>
        /// <param name="testStepRuns">The test step runs</param>
        public void Show(ICollection<TestStepRun> testStepRuns)
        {
            Show(testStepRuns, null);
        }

        /// <summary>
        /// Displays information about a set of test step runs, using additional information
        /// from the test model.
        /// </summary>
        /// <param name="testStepRuns">The test step runs</param>
        /// <param name="testModelData">The test model data, or null if not available</param>
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
                stream = formatter.Format(testStepRuns, testModelData);
            }

            UpdateAsync();
        }

        private void HandleWebBrowserInit(object sender, EventArgs e)
        {
            if (!initialized)
            {
                initialized = true;
                UpdateAsync();
            }
        }

        private void HandleDisposed(object sender, EventArgs e)
        {
            ClearNoUpdate();
            initialized = false;
        }

        private void UpdateAsync()
        {
            if (!initialized)
                return;

            DoAsync(() =>
            {
                webBrowser.DocumentStream = stream;
                webBrowser.Show();
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
