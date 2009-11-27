using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the kind of snap.
    /// </summary>
    public enum SnapKind
    {
        /// <summary>
        /// The character snap did not succeed.  The position was outside the bounds of the document.
        /// </summary>
        None,

        /// <summary>
        /// The character snap was exact.
        /// </summary>
        Exact,

        /// <summary>
        /// The character snap was before the actual character or at a position above
        /// the start of the document.  (eg. To the left of the character if the reading order is left-to-right.)
        /// </summary>
        Leading,

        /// <summary>
        /// The character snap was after the actual character or at a position above
        /// the start of the document.  (eg. To the right of the character if the reading order is left-to-right.)
        /// </summary>
        Trailing
    }
}
