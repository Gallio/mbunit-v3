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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MbUnit.Icarus
{
    public class TaskButton : Control, IButtonControl
    {
        private bool isDefault = false;
        private DialogResult dialogResult = DialogResult.None;

        private string description = string.Empty;
        private Image icon;
        
        private bool mouseOver = false;

        private const int MIN_HEIGHT = 40;

        private Font titleFont;
        private Font descriptionFont;

        public TaskButton()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.CacheText, true);

            titleFont = new Font(this.Font.Name, this.Font.Size + 2, FontStyle.Bold);
            descriptionFont = new Font(this.Font.Name, this.Font.Size, FontStyle.Regular);
        }

        #region IButtonControl Members

        public DialogResult DialogResult
        {
            get { return this.dialogResult; }

            set
            {
                if (this.dialogResult != value)
                {
                    this.dialogResult = value;
                    OnDialogResultChanged();
                }
            }
        }

        public void NotifyDefault(bool value)
        {
            if (this.isDefault != value)
            {
                this.isDefault = value;
                OnIsDefaultChanged();
                Invalidate(true);
            }
        }

        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        #endregion

        public event EventHandler DialogResultChanged;
        protected virtual void OnDialogResultChanged()
        {
            if (DialogResultChanged != null)
            {
                DialogResultChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDefaultChanged;
        protected virtual void OnIsDefaultChanged()
        {
            if (IsDefaultChanged != null)
            {
                IsDefaultChanged(this, EventArgs.Empty);
            }
        }

        public bool IsDefault
        {
            get { return this.isDefault; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle bounds = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);

            if (this.mouseOver)
            {
                e.Graphics.DrawRectangle(new Pen(SystemBrushes.ControlDark), bounds);

                Rectangle gradientArea = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 1, bounds.Height - 1);
                e.Graphics.FillRectangle(new LinearGradientBrush(gradientArea, SystemColors.Control, SystemColors.ControlLight, 90F), gradientArea);
            }

            SizeF titleSize = e.Graphics.MeasureString(this.Text, titleFont);
            SizeF descriptionSize = e.Graphics.MeasureString(this.Description, descriptionFont);

            float maxWidth = (bounds.Width - 40);
            float lineHeight = descriptionSize.Height;

            int rows = 1;
            if (descriptionSize.Width > maxWidth)
                rows = (int)Math.Ceiling(descriptionSize.Width / maxWidth);

            RectangleF descriptionArea = new RectangleF(35, titleSize.Height + 5, maxWidth, lineHeight * rows);

            e.Graphics.DrawString(this.Text, titleFont, SystemBrushes.WindowText, new PointF(35, 2));
            e.Graphics.DrawString(this.Description, descriptionFont, SystemBrushes.WindowText, descriptionArea);

            if (this.Icon != null)
                e.Graphics.DrawImage(this.Icon, 8, 3, 20, 20);
        }

        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        public Image Icon
        {
            get { return this.icon; }
            set { this.icon = value; }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (this.Height == 0)
            {
                this.Height = MIN_HEIGHT;

                Graphics g = this.CreateGraphics();
                SizeF descriptionSize = g.MeasureString(this.Description, descriptionFont);

                float maxWidth = (this.Width - 40);
                float lineHeight = descriptionSize.Height;

                int rows = 1;
                if (descriptionSize.Width > maxWidth)
                {
                    rows = (int)Math.Ceiling(descriptionSize.Width / maxWidth);
                    this.Height = (int)((MIN_HEIGHT + (lineHeight * rows)) - lineHeight);
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            this.mouseOver = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.mouseOver = false;
            Invalidate();
        }

    }
}
