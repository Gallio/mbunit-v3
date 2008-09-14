using System;
using System.Collections.Generic;
using System.ComponentModel;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IProjectController
    {
        ITreeModel Model { get; }
        TestPackageConfig TestPackageConfig { get; }
        BindingList<FilterInfo> TestFilters { get; }
        BindingList<string> HintDirectories { get; }
        string ProjectFileName { get; }

        event EventHandler<AssemblyChangedEventArgs> AssemblyChanged;

        void AddAssemblies(IList<string> assemblies);
        void DeleteFilter(string filterName);
        Filter<ITest> GetFilter(string filterName);
        void NewProject();
        void OpenProject(string projectName);
        void RemoveAllAssemblies();
        void RemoveAssembly(string fileName);
        void SaveFilter(string filterName, Filter<ITest> filter);
        void SaveProject(string projectName);
    }
}
