namespace Gallio.Icarus.Models.ProjectTreeNodes
{
    internal sealed class PropertiesNode : ProjectTreeNode
    {
        public PropertiesNode()
        {
            Text = "Properties";
            Image = Properties.Resources.Properties.ToBitmap();
        }
    }
}