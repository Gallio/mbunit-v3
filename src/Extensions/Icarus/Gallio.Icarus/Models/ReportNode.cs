using Aga.Controls.Tree;
using System.IO;

namespace Gallio.Icarus.Models
{
    internal class ReportNode : Node
    {
        public ReportNode(string file)
        {
            Text = Path.GetFileNameWithoutExtension(file);
            Image = Properties.Resources.XmlFile.ToBitmap();
            Tag = file;
        }
    }
}
