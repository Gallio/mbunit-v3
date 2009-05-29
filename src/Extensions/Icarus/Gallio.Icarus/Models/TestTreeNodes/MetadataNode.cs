namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class MetadataNode : TestTreeNode
    {
        public MetadataNode(string metadata, string metadataType)
            : base(metadata, metadata)
        {
            TestKind = metadataType;
        }
    }
}
