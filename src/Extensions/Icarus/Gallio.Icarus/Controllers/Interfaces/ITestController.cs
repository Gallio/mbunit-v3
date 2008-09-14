using System;
using System.Collections.Generic;
using System.ComponentModel;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface ITestController
    {
        BindingList<string> SelectedTests { get; }
        ITreeModel Model { get; }
        object TreeViewCategory { get; set; }
        Report Report { get; }
        IList<string> TestFrameworks { get; }
        int TestCount { get; }

        event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        event EventHandler<TestStepFinishedEventArgs> TestStepFinished;
        event EventHandler RunStarted;
        event EventHandler RunFinished;
        event EventHandler LoadStarted;
        event EventHandler LoadFinished;
        event EventHandler UnloadStarted;
        event EventHandler UnloadFinished;
        event EventHandler<ShowSourceCodeEventArgs> ShowSourceCode;

        void ApplyFilter(Filter<ITest> filter);
        void Cancel();
        Filter<ITest> GetCurrentFilter();
        void Reload();
        void Reload(TestPackageConfig config);
        void ResetTests();
        void RunTests();
        void UnloadTestPackage();
        void ViewSourceCode(string testId);
    }
}
