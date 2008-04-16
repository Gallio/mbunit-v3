using System;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Model;
using Aga.Controls.Tree;
using System.Collections.ObjectModel;
using System.Collections;

namespace Gallio.Icarus.Controls.Interfaces
{
    public interface ITestTreeModel : ITreeModel
    {
        Collection<Node> Nodes { get; }
        bool FilterPassed { set; }
        bool FilterFailed { set; }
        bool FilterSkipped { set; }
        int TestCount { get; }
        TestTreeNode Root { get; }

        Node FindNode(TreePath path);
        TreePath GetPath(Node node);
        void OnStructureChanged(TreePathEventArgs args);
        void ResetTestStatus();
        void UpdateTestStatus(string testId, TestStatus testStatus);
        void FilterTree();
        void OnTestCountChanged(EventArgs e);
        void OnTestResult(TestResultEventArgs e);

        event EventHandler<EventArgs> TestCountChanged;
        event EventHandler<TestResultEventArgs> TestResult;
    }
}
