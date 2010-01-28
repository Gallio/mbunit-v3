using Gallio.Model.Filters;

namespace Gallio.Icarus.Services
{
    public interface IFilterService
    {
        void ApplyFilterSet(FilterSet<ITestDescriptor> filterSet);
        FilterSet<ITestDescriptor> GenerateFilterSetFromSelectedTests();
    }
}