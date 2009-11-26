using System;
using System.Drawing;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Builds <see cref="Style" /> objects.
    /// </summary>
    public class StyleBuilder
    {
        private StyleProperty<Font> font;
        private StyleProperty<Color> color;
        private StyleProperty<TabStopRuler> tabStopRuler;
        private StyleProperty<bool> wordWrap;
        private StyleProperty<int> leftMargin;
        private StyleProperty<int> rightMargin;
        private StyleProperty<int> firstLineIndent;

        /// <summary>
        /// Creates a builder with all style properties inherited.
        /// </summary>
        public StyleBuilder()
        {
            font = StyleProperty<Font>.Inherit;
            color = StyleProperty<Color>.Inherit;
            tabStopRuler = StyleProperty<TabStopRuler>.Inherit;
            wordWrap = StyleProperty<bool>.Inherit;
            leftMargin = StyleProperty<int>.Inherit;
            rightMargin = StyleProperty<int>.Inherit;
            firstLineIndent = StyleProperty<int>.Inherit;
        }

        /// <summary>
        /// Creates a builder initialized as a copy of an existing style.
        /// </summary>
        /// <param name="style">The style to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> is null.</exception>
        public StyleBuilder(Style style)
        {
            if (style == null)
                throw new ArgumentNullException("style");

            font = new StyleProperty<Font>(style.Font);
            color = new StyleProperty<Color>(style.Color);
            tabStopRuler = new StyleProperty<TabStopRuler>(style.TabStopRuler);
            wordWrap = new StyleProperty<bool>(style.WordWrap);
            leftMargin = new StyleProperty<int>(style.LeftMargin);
            rightMargin = new StyleProperty<int>(style.RightMargin);
            firstLineIndent = new StyleProperty<int>(style.FirstLineIndent);
        }

        /// <summary>
        /// Creates a builder initialized as a copy of an existing style builder.
        /// </summary>
        /// <param name="styleBuilder">The style builder to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="styleBuilder"/> is null.</exception>
        public StyleBuilder(StyleBuilder styleBuilder)
        {
            if (styleBuilder == null)
                throw new ArgumentNullException("styleBuilder");

            font = styleBuilder.Font;
            color = styleBuilder.Color;
            tabStopRuler = styleBuilder.TabStopRuler;
            wordWrap = styleBuilder.WordWrap;
            leftMargin = styleBuilder.LeftMargin;
            rightMargin = styleBuilder.RightMargin;
            firstLineIndent = styleBuilder.FirstLineIndent;
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <remarks>
        /// This is an inline style property.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> contains null.</exception>
        public StyleProperty<Font> Font
        {
            get { return font; }
            set
            {
                if (! value.Inherited && value.Value == null)
                    throw new ArgumentNullException("value");
                font = value;
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <remarks>
        /// This is an inline style property.
        /// </remarks>
        public StyleProperty<Color> Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Gets or sets the tab stop ruler.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> contains null.</exception>
        public StyleProperty<TabStopRuler> TabStopRuler
        {
            get { return tabStopRuler; }
            set
            {
                if (! value.Inherited && value.Value == null)
                    throw new ArgumentNullException("value");
                tabStopRuler = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to perform word-wrapping.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        public StyleProperty<bool> WordWrap
        {
            get { return wordWrap; }
            set { wordWrap = value; }
        }

        /// <summary>
        /// Gets or sets the left margin width in pixels.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        public StyleProperty<int> LeftMargin
        {
            get { return leftMargin; }
            set { leftMargin = value; }
        }

        /// <summary>
        /// Gets or sets the right margin width in pixels.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        public StyleProperty<int> RightMargin
        {
            get { return rightMargin; }
            set { rightMargin = value; }
        }

        /// <summary>
        /// Gets or sets the first line indent in pixels.
        /// </summary>
        /// <remarks>
        /// This is a paragraph style property.
        /// </remarks>
        public StyleProperty<int> FirstLineIndent
        {
            get { return firstLineIndent; }
            set { firstLineIndent = value; }
        }

        /// <summary>
        /// Creates an immutable style object from the builder's properties.
        /// </summary>
        /// <param name="inheritedStyle">The style to inherit.</param>
        /// <returns>The new style object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inheritedStyle"/> is null.</exception>
        public Style ToStyle(Style inheritedStyle)
        {
            return new Style(
                font.GetValueOrInherit(inheritedStyle.Font),
                color.GetValueOrInherit(inheritedStyle.Color),
                tabStopRuler.GetValueOrInherit(inheritedStyle.TabStopRuler),
                wordWrap.GetValueOrInherit(inheritedStyle.WordWrap),
                leftMargin.GetValueOrInherit(inheritedStyle.LeftMargin),
                rightMargin.GetValueOrInherit(inheritedStyle.RightMargin),
                firstLineIndent.GetValueOrInherit(inheritedStyle.FirstLineIndent));
        }
    }
}
