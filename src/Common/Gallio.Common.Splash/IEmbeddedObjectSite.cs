using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the environment of an embedded object.
    /// </summary>
    public interface IEmbeddedObjectSite
    {
        /// <summary>
        /// Gets the control of the containing view.
        /// </summary>
        Control ParentControl { get; }

        /// <summary>
        /// Gets the style of the paragraph that contains the embedded object.
        /// </summary>
        Style ParagraphStyle { get; }

        /// <summary>
        /// Gets the style of the inline run that contains the embedded object.
        /// </summary>
        Style InlineStyle { get; }

        /// <summary>
        /// Gets whether the document reading order is right-to-left.
        /// </summary>
        bool RightToLeft { get; }

        /// <summary>
        /// Gets the index of the character that represents the embedded object
        /// for selection and hit testing in the document.
        /// </summary>
        int CharIndex { get; }
    }
}
