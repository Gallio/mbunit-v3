using System;
using System.IO;
using System.Windows.Forms;
using EnvDTE;

namespace Gallio.Navigator
{
    /// <summary>
    /// Gallio navigator implementation.
    /// </summary>
    public class GallioNavigatorImpl : IGallioNavigator
    {
        /// <inheritdoc />
        public bool NavigateTo(string path, int lineNumber, int columnNumber)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException("Path must not be empty.", "path");
            if (!Path.IsPathRooted(path))
                throw new ArgumentException("Path must be rooted.", "path");
            if (lineNumber < 0)
                throw new ArgumentOutOfRangeException("lineNumber");
            if (columnNumber < 0)
                throw new ArgumentOutOfRangeException("columnNumber");

            try
            {
                return NavigateToImpl(path, lineNumber, columnNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Gallio could not navigate to: {0} ({1},{2}).\n\n{3}.", path, lineNumber, columnNumber, ex));
                return false;
            }
        }

        private static bool NavigateToImpl(string path, int lineNumber, int columnNumber)
        {
            path = Path.GetFullPath(path);

            VisualStudioSupport.WithDTE(dte =>
            {
                dte.MainWindow.Activate();
                dte.MainWindow.Visible = true;

                Window window = dte.OpenFile(Constants.vsViewKindCode, path);
                TextSelection selection = window.Selection as TextSelection;
                if (lineNumber != 0)
                {
                    if (selection != null)
                        selection.MoveToLineAndOffset(lineNumber, Math.Max(1, columnNumber), false);
                }

                window.Activate();
                window.Visible = true;
            });

            return true;
        }
    }
}
