// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using System.Windows.Forms;
using System.Drawing;

namespace Gallio.Icarus.Controls
{
    internal class PropertiesTabControl : TabControl
    {
        public PropertiesTabControl()
        {
            Alignment = TabAlignment.Left;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(35, 125);
            DrawMode = TabDrawMode.OwnerDrawFixed;
            DrawItem += (sender, e) =>
            {
                var tabPage = TabPages[e.Index];
                var tabBounds = GetTabRect(e.Index);

                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                var textColor = e.State == DrawItemState.Selected ? 
                    Color.Black : e.ForeColor;
                var textBrush = new SolidBrush(textColor);
                
                var stringFlags = new StringFormat();
                stringFlags.Alignment = StringAlignment.Center;
                stringFlags.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(tabPage.Text, e.Font, textBrush, 
                    tabBounds, new StringFormat(stringFlags));
            };
        }
    }
}
