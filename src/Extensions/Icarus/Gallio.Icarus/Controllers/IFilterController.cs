using System;
using System.ComponentModel;
using Gallio.Runner.Projects;

namespace Gallio.Icarus.Controllers
{
    interface IFilterController
    {
        BindingList<FilterInfo> TestFilters { get; }

        void ApplyFilter(string filter);
        void DeleteFilter(FilterInfo filterInfo);
        void SaveFilter(string filterName);
    }
}
