using Gallio.Icarus.Models.ProjectTreeNodes;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Models.ProjectTreeNodes
{
    [Category("Models"), Author("Graham Hay"), TestsOn(typeof(AssembliesNode)), Importance(Importance.NoOneReallyCaresAbout)]
    internal class AssembliesNodeTest
    {
        [Test]
        public void Check_node_text()
        {
            var node = new AssembliesNode();
            Assert.AreEqual("Assemblies", node.Text);
        }
    }
}