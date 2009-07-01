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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Displays a caption overlayed on top of an image.
    /// </summary>
    public class CaptionOverlay : Overlay
    {
        private const int Margin = 16;

        private string text;
        private int fontSize;

        /// <summary>
        /// Creates a caption overlay.
        /// </summary>
        public CaptionOverlay()
        {
            text = "";
            fontSize = 16;
        }

        /// <summary>
        /// Gets or sets the text of the caption.
        /// </summary>
        /// <value>The text of the caption or an empty string if none.</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string Text
        {
            get { return text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                text = value;
            }
        }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        /// <value>The font size.  Default is 16.</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 1.</exception>
        public int FontSize
        {
            get { return fontSize; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "Font size must be at least 1 point.");
                fontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.  Default is <see cref="Media.HorizontalAlignment.Left" />.</value>
        public HorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>The vertical alignment.  Default is <see cref="Media.VerticalAlignment.Top" />.</value>
        public VerticalAlignment VerticalAlignment { get; set; }

        /// <inheritdoc />
        protected override void PaintImpl(OverlayPaintRequest request)
        {
            if (text.Length == 0)
                return;

            Rectangle layoutRect = new Rectangle(
                request.FramesBounds.Left + Margin,
                request.FramesBounds.Top + Margin,
                request.FramesBounds.Width - Margin * 2,
                request.FramesBounds.Height - Margin * 2);

            StringFormat format = new StringFormat(StringFormat.GenericDefault)
            {
                Alignment = ToStringAlignment(HorizontalAlignment),
                LineAlignment = ToStringAlignment(VerticalAlignment)
            };

            GraphicsPath path = new GraphicsPath();
            path.AddString(text, FontFamily.GenericSansSerif, (int) FontStyle.Regular, fontSize, layoutRect, format);

            Brush fillBrush = Brushes.Black;
            Pen outlinePen = new Pen(Brushes.White, fontSize / 2.0f);
            outlinePen.Alignment = PenAlignment.Outset;

            request.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            request.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            request.Graphics.DrawPath(outlinePen, path);
            request.Graphics.FillPath(fillBrush, path);
        }

        private static StringAlignment ToStringAlignment(HorizontalAlignment alignment)
        {
            switch (alignment)
            {
                case HorizontalAlignment.Left:
                    return StringAlignment.Near;
                case HorizontalAlignment.Center:
                    return StringAlignment.Center;
                case HorizontalAlignment.Right:
                    return StringAlignment.Far;
                default:
                    throw new ArgumentOutOfRangeException("alignment");
            }
        }

        private static StringAlignment ToStringAlignment(VerticalAlignment alignment)
        {
            switch (alignment)
            {
                case VerticalAlignment.Top:
                    return StringAlignment.Near;
                case VerticalAlignment.Middle:
                    return StringAlignment.Center;
                case VerticalAlignment.Bottom:
                    return StringAlignment.Far;
                default:
                    throw new ArgumentOutOfRangeException("alignment");
            }
        }
    }
}
