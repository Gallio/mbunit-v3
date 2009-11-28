using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Provides style parameters for painting.
    /// </summary>
    public class PaintOptions
    {
        /// <summary>
        /// Initializes paint options to system defaults.
        /// </summary>
        public PaintOptions()
        {
            BackgroundColor = SystemColors.Window;
            SelectedTextColor = SystemColors.HighlightText;
            SelectedBackgroundColor = SystemColors.Highlight;
        }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the selected text color.
        /// </summary>
        public Color SelectedTextColor { get; set; }

        /// <summary>
        /// Gets or sets the selected background color.
        /// </summary>
        public Color SelectedBackgroundColor { get; set; }
    }
}
