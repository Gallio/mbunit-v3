using System;
using System.Windows.Forms;

namespace Gallio.VisualStudio.Shell.UI
{
    /// <summary>
    /// Abstract base class for tool windows.
    /// </summary>
    public class ShellToolWindow : UserControl, IShellComponent
    {
        private IShell shell;
        private ShellToolWindowPane toolWindowPane;
        private ShellToolWindowContainer toolWindowContainer;
        private string storedCaption = string.Empty;

        /// <summary>
        /// Default constructor.  (For designer.)
        /// </summary>
        public ShellToolWindow()
        {
        }

        /// <summary>
        /// Gets the shell, or null if not associated yet.
        /// </summary>
        public IShell Shell
        {
            get { return shell; }
        }

        /// <summary>
        /// Gets the container, or null if not associated yet.
        /// </summary>
        public ShellToolWindowContainer ToolWindowContainer
        {
            get { return toolWindowContainer; }
        }

        /// <summary>
        /// Gets the associated tool window pane, or null if not associated yet.
        /// </summary>
        public ShellToolWindowPane ToolWindowPane
        {
            get { return toolWindowPane; }
        }

        /// <summary>
        /// Gets or sets the window caption.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string Caption
        {
            get
            {
                if (toolWindowPane != null)
                    storedCaption = toolWindowPane.Caption;
                return storedCaption;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                storedCaption = value;
                if (toolWindowPane != null)
                    toolWindowPane.Caption = value;
            }
        }

        /// <summary>
        /// Closes the window frame.
        /// </summary>
        public void Close()
        {
            ThrowIfNoPane();
            toolWindowPane.Close();
        }

        /// <inheritdoc />
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            toolWindowContainer = Parent as ShellToolWindowContainer;
            if (toolWindowContainer != null)
            {
                toolWindowPane = toolWindowContainer.ToolWindowPane;
                if (toolWindowPane == null)
                    throw new ShellException("The tool window should not be added to its container until the container has been associated with a tool window pane.");

                shell = toolWindowPane.Shell;
            }
            else
            {
                toolWindowPane = null;
                shell = null;
            }
        }
        /// <inheritdoc />
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);

            if (toolWindowPane != null)
            {
                if (value)
                    toolWindowPane.Show();
                else
                    toolWindowPane.Hide();
            }
        }

        private void ThrowIfNoPane()
        {
            if (toolWindowPane == null)
                throw new InvalidOperationException("The window does not have an associated tool window pane.");
        }
    }
}
