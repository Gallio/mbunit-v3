namespace Gallio.Icarus.Models.ProjectTreeNodes
{
    internal sealed class ReportsNode : ProjectTreeNode
    {
        public ReportsNode()
        {
            // TODO: i18n
            Text = "Reports";
            Image = Properties.Resources.Report.ToBitmap();
        }
    }
}
