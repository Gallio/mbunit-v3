// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace MbUnit.Icarus.Controls
{
    internal class TestStatusBar : Control
    {
        private double elapsedTime;
        private Color failedColor = Color.Red;
        private int failedTests = 0;
        private Color ignoredColor = Color.Gold;
        private int ignoredTests = 0;

        private Color passedColor = Color.Green;
        private int passedTests = 0;
        private Color skippedColor = Color.SteelBlue;
        private int skippedTests = 0;
        private int totalTests = 0;

        public TestStatusBar()
        {
            //Font = new Font("Verdana", 8);
            //BackColor = Color.White;

            // Setup the control styles so that the control does not flicker
            // when it is resized or redrawn.
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Font = new Font("Verdana", 8);
            BackColor = Color.White;

            base.OnPaint(e);

            int alpha = 200;
            SolidBrush backBrush = new SolidBrush(BackColor);
            SolidBrush textBrush = new SolidBrush(FromColor(ForeColor, alpha));

            // Define the drawing area.
            Rectangle r = ClientRectangle;
            r = new Rectangle(
                r.Location,
                new Size(r.Width - 1, r.Height - 1)
                );

            // Fill the background.
            e.Graphics.FillRectangle(backBrush, r);

            SmoothingMode m = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (totalTests > 0)
            {
                // Draw passed region.
                float width = r.Width*(passedTests/(float) totalTests);
                float left = r.Left;
                float right = r.Left + width;
                DrawProgressRegion(e.Graphics, r, left, width, passedColor);

                // Draw ignored region.
                width = r.Width*(ignoredTests/(float) totalTests);
                left = right;
                right = left + width;
                DrawProgressRegion(e.Graphics, r, left, width, ignoredColor);

                // Draw skipped region.
                width = r.Width*(skippedTests/(float) totalTests);
                left = right;
                right = left + width;
                DrawProgressRegion(e.Graphics, r, left, width, skippedColor);

                // Draw failed region.
                width = r.Width*(failedTests/(float) totalTests);
                left = right;
                DrawProgressRegion(e.Graphics, r, left, width, failedColor);
            }

            // Draw a border around the control.
            e.Graphics.DrawRectangle(Pens.Black, r);

            // Build up the display text.
            string text = string.Format(CultureInfo.CurrentCulture, Text, totalTests, passedTests, ignoredTests, skippedTests, 
                failedTests, elapsedTime);

            // Draw the text to the center of the control.
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(
                text,
                Font,
                textBrush,
                r.Left + r.Width/2,
                (r.Top + r.Height/2) + 1,
                format
                );
            e.Graphics.SmoothingMode = m;
        }

        #region Public Functions

        /// <summary>
        /// Resets the state of the status bar.
        /// </summary>
        public void Clear()
        {
            passedTests = 0;
            failedTests = 0;
            ignoredTests = 0;
            skippedTests = 0;

            elapsedTime = 0;

            Invalidate();
        }

        #endregion

        #region Private Functions

        private static Color FromColor(Color c, int alpha)
        {
            return Color.FromArgb(
                alpha,
                c.R,
                c.G,
                c.B
                );
        }

        private static void DrawProgressRegion(Graphics g, Rectangle r, float left, float width, Color c)
        {
            if (width == 0)
                return;

            RectangleF re = new RectangleF(left, r.Y, width, r.Height);
            LinearGradientBrush brush = new LinearGradientBrush(re, FromColor(c, 225), FromColor(c, 75), 45, true);

            g.FillRectangle(brush, re);
        }

        #endregion

        #region Properties

        [Browsable(false)]
        public double ElapsedTime
        {
            get { return elapsedTime; }
            set
            {
                elapsedTime = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Total
        {
            get { return totalTests; }
            set
            {
                totalTests = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Passed
        {
            get { return passedTests; }
            set
            {
                passedTests = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Failed
        {
            get { return failedTests; }
            set
            {
                failedTests = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Ignored
        {
            get { return ignoredTests; }
            set
            {
                ignoredTests = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Skipped
        {
            get { return skippedTests; }
            set
            {
                skippedTests = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color PassedColor
        {
            get { return passedColor; }
            set
            {
                passedColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color FailedColor
        {
            get { return failedColor; }
            set
            {
                failedColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color IngoredColor
        {
            get { return ignoredColor; }
            set
            {
                ignoredColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color SkippedColor
        {
            get { return skippedColor; }
            set
            {
                skippedColor = value;
                Invalidate();
            }
        }

        #endregion

        #region Hidden Properties

        [Browsable(false)]
        public override string Text
        {
            // Force the control to display this text always.
            get { return "{0} tests - {1} successes - {2} ignored - {3} skipped - {4} failures - {5:0.0}s"; }
        }

        [Browsable(false)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }
        }

        #endregion
    }
}