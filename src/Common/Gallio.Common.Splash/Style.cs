using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the style of a text run.
    /// </summary>
    public class Style
    {
        private readonly static Font dummyFont = new Font(FontFamily.GenericSerif, 12);

        /// <summary>
        /// Gets the font.
        /// </summary>
        public Font Font
        {
            get { return dummyFont; }
        }

        /// <summary>
        /// Gets the tab stop ruler.
        /// </summary>
        public TabStopRuler TabStopRuler
        {
            get { return TabStopRuler.CreatePixelRuler(60); }
        }

        /// <summary>
        /// Gets whether to perform word-wrapping.
        /// </summary>
        public bool WordWrap
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the left margin width in pixels.
        /// </summary>
        public int LeftMargin
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the right margin width in pixels.
        /// </summary>
        public int RightMargin
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the first line indent in pixels.
        /// </summary>
        public int FirstLineIndent
        {
            get { return 0; }
        }
    }
}
