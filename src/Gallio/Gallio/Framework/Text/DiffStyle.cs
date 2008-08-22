using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Framework.Text
{
    /// <summary>
    /// Specifies the presentation style for a <see cref="DiffSet" />.
    /// </summary>
    public enum DiffStyle
    {
        /// <summary>
        /// Display the left and right document contents and diffs fully interleaved.
        /// </summary>
        Interleaved = 0,

        /// <summary>
        /// Display only the left document contents.
        /// </summary>
        LeftOnly,

        /// <summary>
        /// Display only the right document contents.
        /// </summary>
        RightOnly
    }
}
