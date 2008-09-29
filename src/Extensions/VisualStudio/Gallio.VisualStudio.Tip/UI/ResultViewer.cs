using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gallio.VisualStudio.Shell.ToolWindows;

namespace Gallio.VisualStudio.Tip.UI
{
    /// <summary>
    /// User control which displays detailed about a Gallio test result.
    /// </summary>
    public partial class ResultViewer : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// For the designer only.
        /// </summary>
        public ResultViewer()
        {
            InitializeComponent();
        }

        private GallioTestResult testResult;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="testResult">The Gallio test result.</param>
        public ResultViewer(GallioTestResult testResult)
        {
            if (testResult == null)
            {
                throw new ArgumentNullException("testResult");
            }

            InitializeComponent();
            this.testResult = testResult;
        }

        private void ResultViewer_Load(object sender, EventArgs e)
        {
            InitializeContent();
        }

        private void InitializeContent()
        {
            labelTitle.Text = testResult.TestName;
        }
    }
}
