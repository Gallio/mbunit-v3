using System;
using System.Windows.Forms;

namespace Gallio.UI.ErrorReporting
{
    /// <summary>
    /// Presents an error dialog consisting of a title, a message and a detailed message.
    /// </summary>
    public partial class ErrorDialog : Form
    {
        /// <summary>
        /// Creates the dialog.
        /// </summary>
        public ErrorDialog()
        {
            InitializeComponent();

            ErrorTitle = "Error";
            ErrorMessage = "";
            ErrorDetails = "";
            ErrorDetailsVisible = false;
        }

        /// <summary>
        /// Shows the error dialog.
        /// </summary>
        /// <param name="owner">The owner window, or null if none.</param>
        /// <param name="errorTitle">The error title.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorDetails">The error details.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="errorTitle"/>,
        /// <paramref name="errorMessage"/> or <paramref name="errorDetails"/> is null.</exception>
        public static void Show(IWin32Window owner, string errorTitle, string errorMessage, string errorDetails)
        {
            if (errorTitle == null)
                throw new ArgumentNullException("errorTitle");
            if (errorMessage == null)
                throw new ArgumentNullException("errorMessage");
            if (errorDetails == null)
                throw new ArgumentNullException("errorDetails");

            using (var dialog = new ErrorDialog()
            {
                ErrorTitle = errorTitle,
                ErrorMessage = errorMessage,
                ErrorDetails = errorDetails,
                ShowInTaskbar = owner == null
            })
            {
                dialog.ShowDialog(owner);
            }
        }

        /// <summary>
        /// Gets or sets the error title.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string ErrorTitle
        {
            get { return Text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string ErrorMessage
        {
            get { return errorMessageLabel.Text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                errorMessageLabel.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the error details.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string ErrorDetails
        {
            get { return errorDetailsTextBox.Text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                errorDetailsTextBox.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the error details panel is visible.
        /// </summary>
        public bool ErrorDetailsVisible
        {
            get { return errorDetailsTextBox.Visible; }
            set
            {
                errorDetailsTextBox.Visible = value;
                if (value)
                    showOrHideDetailsButton.Text = "<< Details";
                else
                    showOrHideDetailsButton.Text = "Details >>";
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void showOrHideDetailsButton_Click(object sender, EventArgs e)
        {
            ErrorDetailsVisible = !ErrorDetailsVisible;
        }
    }
}