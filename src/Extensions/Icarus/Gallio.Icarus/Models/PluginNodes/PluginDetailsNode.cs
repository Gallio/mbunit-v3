using Aga.Controls.Tree;

namespace Gallio.Icarus.Models.PluginNodes
{
    internal class PluginDetailsNode : Node
    {
        public string Name
        {
            get;
            private set;
        }

        public string Value
        {
            get;
            private set;
        }

        public PluginDetailsNode(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
