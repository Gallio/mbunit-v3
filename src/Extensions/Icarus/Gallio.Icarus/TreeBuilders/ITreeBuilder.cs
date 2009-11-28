using Gallio.Icarus.Models;
using Gallio.Model.Schema;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.TreeBuilders
{
    internal interface ITreeBuilder
    {
        bool CanHandle(string treeViewCategory);
        TestTreeNode BuildTree(IProgressMonitor progressMonitor, TestModelData testModelData, 
            TreeBuilderOptions options);
    }
}