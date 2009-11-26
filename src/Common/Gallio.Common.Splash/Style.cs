using System;
using System.Drawing;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the style of a text run.
    /// </summary>
    public class Style : IEquatable<Style>
    {
        private readonly Font font;
        private readonly Color color;
        private readonly TabStopRuler tabStopRuler;
        private readonly bool wordWrap;
        private readonly int leftMargin;
        private readonly int rightMargin;
        private readonly int firstLineIndent;
        private int hashCode;

        internal Style(Font font, Color color, TabStopRuler tabStopRuler, bool wordWrap,
            int leftMargin, int rightMargin, int firstLineIndent)
        {
            this.font = font;
            this.color = color;
            this.tabStopRuler = tabStopRuler;
            this.wordWrap = wordWrap;
            this.leftMargin = leftMargin;
            this.rightMargin = rightMargin;
            this.firstLineIndent = firstLineIndent;
        }

        /// <summary>
        /// Creates a default style based on current system font and color settings.
        /// </summary>
        /// <returns>The style.</returns>
        public static Style CreateDefaultStyle()
        {
            return new Style(SystemFonts.DefaultFont, SystemColors.WindowText,
                TabStopRuler.CreatePixelRuler(60), true, 0, 0, 0);
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <remarks>
        /// This is an inline style property.
        /// </remarks>
        public Font Font
        {
            get { return font; }
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <remarks>
        /// This is an inline style property.
        /// </remarks>
        public Color Color
        {
            get { return color; }
        }

        /// <summary>
        /// Gets the tab stop ruler.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        public TabStopRuler TabStopRuler
        {
            get { return tabStopRuler; }
        }

        /// <summary>
        /// Gets whether to perform word-wrapping.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        public bool WordWrap
        {
            get { return wordWrap; }
        }

        /// <summary>
        /// Gets the left margin width in pixels.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        public int LeftMargin
        {
            get { return leftMargin; }
        }

        /// <summary>
        /// Gets the right margin width in pixels.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        public int RightMargin
        {
            get { return rightMargin; }
        }

        /// <summary>
        /// Gets the first line indent in pixels.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        public int FirstLineIndent
        {
            get { return firstLineIndent; }
        }

        /// <inheritdoc />
        public virtual bool Equals(Style other)
        {
            return this == other
                || other != null
                && font == other.font
                && color == other.color
                && tabStopRuler == other.tabStopRuler
                && wordWrap == other.wordWrap
                && leftMargin == other.leftMargin
                && rightMargin == other.rightMargin
                && firstLineIndent == other.firstLineIndent;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as Style);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (hashCode != 0)
                return hashCode;

            int newHashCode = font.GetHashCode()
                ^ color.GetHashCode()
                ^ tabStopRuler.GetHashCode()
                ^ (wordWrap ? 0x4000000 : 0)
                ^ (leftMargin << 5)
                ^ (rightMargin << 10)
                ^ (firstLineIndent << 15);
            if (newHashCode == 0)
                newHashCode = 1;
            hashCode = newHashCode;
            return hashCode;
        }
    }
}
