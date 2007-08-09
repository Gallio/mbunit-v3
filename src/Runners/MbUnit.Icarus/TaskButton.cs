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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MbUnit.Icarus
{
    public class TaskButton :
        Control, IButtonControl
    {
        private const int MIN_HEIGHT = 40;
        private string description = string.Empty;
        private Font descriptionFont;
        private DialogResult dialogResult = DialogResult.None;
        private Image icon;
        private bool isDefault = false;
        private bool mouseOver = false;
        private Font titleFont;

        public TaskButton()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.CacheText, true);

            Initialize();
        }

        public bool IsDefault
        {
            get { return isDefault; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public Image Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        #region IButtonControl Members

        public DialogResult DialogResult
        {
            get { return dialogResult; }

            set
            {
                if (dialogResult != value)
                {
                    dialogResult = value;
                    OnDialogResultChanged();
                }
            }
        }

        public void NotifyDefault(bool value)
        {
            if (isDefault != value)
            {
                isDefault = value;
                OnIsDefaultChanged();
                Invalidate(true);
            }
        }

        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        #endregion

        private void Initialize()
        {
            titleFont = new Font(Font.Name, Font.Size + 2, FontStyle.Bold);
            descriptionFont = new Font(Font.Name, Font.Size, FontStyle.Regular);
        }

        public event EventHandler DialogResultChanged;

        protected virtual void OnDialogResultChanged()
        {
            if (DialogResultChanged != null)
                DialogResultChanged(this, EventArgs.Empty);
        }

        public event EventHandler IsDefaultChanged;

        protected virtual void OnIsDefaultChanged()
        {
            if (IsDefaultChanged != null)
                IsDefaultChanged(this, EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle bounds = new Rectangle(0 , 0, Width - 1, Height - 1);

            if (mouseOver)
            {
                e.Graphics.DrawRectangle(new Pen(SystemBrushes.ControlDark), bounds);

                Rectangle gradientArea = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 1, bounds.Height - 1);

                // Make sure that the darker system colour is on the bottom. This gives a better selection effect.
                float angle = 90F;
                if (SystemColors.Control.GetBrightness() > SystemColors.ControlLight.GetBrightness())
                    angle = -90F;

                e.Graphics.FillRectangle(
                    new LinearGradientBrush(gradientArea, SystemColors.ControlLight, SystemColors.Control, angle),
                    gradientArea);

            }

            SizeF titleSize = e.Graphics.MeasureString(Text, titleFont);
            SizeF descriptionSize = e.Graphics.MeasureString(Description, descriptionFont);

            float maxWidth = (bounds.Width - 40);
            float lineHeight = descriptionSize.Height;

            int rows = 1;
            if (descriptionSize.Width > maxWidth)
                rows = (int) Math.Ceiling(descriptionSize.Width/maxWidth);

            RectangleF descriptionArea = new RectangleF(35, titleSize.Height + 5, maxWidth, lineHeight*rows);

            e.Graphics.DrawString(Text, titleFont, SystemBrushes.WindowText, new PointF(35, 2));
            e.Graphics.DrawString(Description, descriptionFont, SystemBrushes.WindowText, descriptionArea);

            if (Icon != null)
                e.Graphics.DrawImage(Icon, 8, 3, 20, 20);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (Height == 0)
            {
                Height = MIN_HEIGHT;

                Graphics g = CreateGraphics();
                SizeF descriptionSize = g.MeasureString(Description, descriptionFont);

                float maxWidth = (Width - 40);
                float lineHeight = descriptionSize.Height;

                if (descriptionSize.Width > maxWidth)
                {
                    int rows = (int) Math.Ceiling(descriptionSize.Width/maxWidth);
                    Height = (int) ((MIN_HEIGHT + (lineHeight*rows)) - lineHeight);
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Cursor = Cursors.Hand;
            mouseOver = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mouseOver = false;
            Invalidate();
        }
    }
}