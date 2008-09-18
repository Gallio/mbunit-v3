using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Gallio.Navigator
{
    /// <summary>
    /// Provides access to Gallio services.
    /// </summary>
    [ComVisible(true)]
    [Guid("B7F075D8-56EC-49f7-8692-89BEECBD7A0F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IGallioNavigator
    {
        /// <summary>
        /// Opens up an editor for the specified path and line number.
        /// </summary>
        /// <remarks>
        /// Lines and columns are numbered starting from 1.
        /// Zero indicates an unspecified value.
        /// </remarks>
        /// <param name="path">The path</param>
        /// <param name="lineNumber">The line number, or 0 if unspecified</param>
        /// <param name="columnNumber">The column number, or 0 if unspecified</param>
        /// <returns>True if the navigation succeeded</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="path"/> is invalid, or if
        /// <paramref name="lineNumber"/> or <paramref name="columnNumber"/> is less than 0</exception>
        bool NavigateTo(string path, int lineNumber, int columnNumber);
    }
}
