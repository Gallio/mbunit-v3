using System;
using System.Collections.Generic;
using Gallio.Icarus.Models;
using Gallio.Model;

namespace Gallio.Icarus.TestExplorer
{
    public interface IController
    {
        event EventHandler SaveState;
        event EventHandler RestoreState;

        void SortTree(SortOrder sortOrder);
        void FilterStatus(TestStatus testStatus);
        void AddFiles(string[] fileNames);
        void RemoveAllFiles();
        void RemoveFile(string fileName);
        void RefreshTree();
        void ShowSourceCode(string testId);
        void ResetTests();
        void SetTreeSelection(IList<TestTreeNode> nodes);
    }
}