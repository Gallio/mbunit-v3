using System.Windows.Forms;

namespace Gallio.UI.Controls
{
    ///<summary>
    /// Parses a string representation of a <see cref="Keys"/> enum.
    ///</summary>
    public interface IKeysParser
    {
        ///<summary>
        /// Parses a string representation of a <see cref="Keys"/> enum.
        ///</summary>
        ///<param name="shortcut">The shortcut string to parse.</param>
        ///<returns>A <see cref="Keys"/> enum.</returns>
        /// <example>
        /// For example, "Ctrl + S" would parse as Keys.Control | Keys.S
        /// </example>
        Keys Parse(string shortcut);
    }
}