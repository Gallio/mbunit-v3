using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace MbUnit.GUI.Controls
{
    class TestStatusBar : Control
    {
        private int totalTests = 0;

        private int passedTests = 0;
        private int failedTests = 0;
        private int skippedTests = 0;
        private int ignoredTests = 0;

        private double elapsedTime = 0;

        private Color passedColor = Color.Green;
        private Color failedColor = Color.Red;
        private Color skippedColor = Color.SteelBlue;
        private Color ignoredColor = Color.Gold;

        public TestStatusBar()
        {
            this.Font = new Font("Verdana", 8);
            this.BackColor = Color.White;

            // Setup the control styles so that the control does not flicker
            // when it is resized or redrawn.
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int alpha = 200;
            SolidBrush backBrush = new SolidBrush(this.BackColor);
            SolidBrush textBrush = new SolidBrush(FromColor(this.ForeColor, alpha));

            // Define the drawing area.
            Rectangle r = this.ClientRectangle;
            r = new Rectangle(
                r.Location,
                new Size(r.Width - 1, r.Height - 1)
                );

            // Fill the background.
            e.Graphics.FillRectangle(backBrush, r);

            SmoothingMode m = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (this.totalTests > 0)
            {
                // Draw passed region.
                float width = r.Width * (this.passedTests / (float)this.totalTests);
                float left = r.Left;
                float right = r.Left + width;
                DrawProgressRegion(e.Graphics, r, left, width, passedColor);

                // Draw ignored region.
                width = r.Width * (this.ignoredTests / (float)this.totalTests);
                left = right;
                right = left + width;
                DrawProgressRegion(e.Graphics, r, left, width, ignoredColor);

                // Draw skipped region.
                width = r.Width * (this.skippedTests / (float)this.totalTests);
                left = right;
                right = left + width;
                DrawProgressRegion(e.Graphics, r, left, width, skippedColor);

                // Draw failed region.
                width = r.Width * (this.failedTests / (float)this.totalTests);
                left = right;
                right = left + width;
                DrawProgressRegion(e.Graphics, r, left, width, failedColor);
            }

            // Draw a border around the control.
            e.Graphics.DrawRectangle(Pens.Black, r);

            // Build up the display text.
            string text = string.Format(
                this.Text,
                this.totalTests,
                this.passedTests,
                this.ignoredTests,
                this.skippedTests,
                this.failedTests,
                this.elapsedTime
                );

            // Draw the text to the center of the control.
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(
                text,
                this.Font,
                textBrush,
                r.Left + r.Width / 2,
                (r.Top + r.Height / 2) + 1,
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
            this.passedTests = 0;
            this.failedTests = 0;
            this.ignoredTests = 0;
            this.skippedTests = 0;
            this.totalTests = 0;

            this.elapsedTime = 0;

            this.Invalidate();
        }

        #endregion

        #region Private Functions

        private Color FromColor(Color c, int alpha)
        {
            return Color.FromArgb(
                alpha,
                c.R,
                c.G,
                c.B
                );
        }

        private void DrawProgressRegion(Graphics g, Rectangle r, float left, float width, Color c)
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
            get { return this.elapsedTime; }
            set { 
                this.elapsedTime = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Total
        {
            get { return this.totalTests; }
            set { 
                this.totalTests = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Passed
        {
            get { return this.passedTests; }
            set { 
                this.passedTests = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Failed
        {
            get { return this.failedTests; }
            set { 
                this.failedTests = value;
                this.Invalidate();
            } 
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Ignored
        {
            get { return this.ignoredTests; }
            set { 
                this.ignoredTests = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Test Status")]
        public int Skipped
        {
            get { return this.skippedTests; }
            set { 
                this.skippedTests = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color PassedColor
        {
            get { return this.passedColor; }
            set { 
                this.passedColor = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color FailedColor
        {
            get { return this.failedColor; }
            set {
                this.failedColor = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color IngoredColor
        {
            get { return this.ignoredColor; }
            set {
                this.ignoredColor = value;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color SkippedColor
        {
            get { return this.skippedColor; }
            set {
                this.skippedColor = value;
                this.Invalidate();
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
