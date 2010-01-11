namespace Gallio.Icarus.Events
{
    public class TreeViewCategoryChanged : Event
    {
        public string TreeViewCategory { get; private set; }

        public TreeViewCategoryChanged(string treeViewCategory)
        {
            TreeViewCategory = treeViewCategory;
        }
    }
}