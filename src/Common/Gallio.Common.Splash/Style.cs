// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Drawing;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the style of a text run.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A style is immutable once created and can be compared for equality with other styles.
    /// </para>
    /// </remarks>
    public sealed class Style : IEquatable<Style>
    {
        private static Style defaultStyle;

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
        /// Gets a default style based on current system font and color settings.
        /// </summary>
        /// <value>The style.</value>
        public static Style Default
        {
            get
            {
                Style result = defaultStyle;
                if (result != null)
                    return result;

                result = new Style(SystemFonts.DefaultFont, SystemColors.WindowText,
                    new PixelTabStopRuler(60, 10), true, 0, 0, 0);
                defaultStyle = result;
                return result;
            }
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
        public bool Equals(Style other)
        {
            return this == other
                || other != null
                && Equals(font, other.font)
                && Equals(color, other.color)
                && Equals(tabStopRuler, other.tabStopRuler)
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
