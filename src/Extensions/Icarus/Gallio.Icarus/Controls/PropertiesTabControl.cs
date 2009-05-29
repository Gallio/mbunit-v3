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
