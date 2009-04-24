using System.IO;
using Aga.Controls.Tree;

namespace Gallio.Icarus.Models
{
    internal class AssemblyNode : Node
    {
        public AssemblyNode(string assemblyFile)
        {
            Text = Path.GetFileNameWithoutExtension(assemblyFile);
            Image = Properties.Resources.Assembly;
            Tag = assemblyFile;
        }
    }
}
