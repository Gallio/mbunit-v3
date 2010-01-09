using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Model;

namespace Gallio.Icarus.TestExplorer
{
    public class Controller
    {
        private readonly Model model;
        private readonly IEventAggregator eventAggregator;

        public Controller(Model model, IEventAggregator eventAggregator)
        {
            this.model = model;
            this.eventAggregator = eventAggregator;
        }

        public void SortTree(SortOrder sortOrder)
        {
            eventAggregator.Send(new SortTreeEvent(sortOrder));
        }

        public void FilterStatus(TestStatus testStatus)
        {
            switch (testStatus)
            {
                case TestStatus.Passed:
                    model.FilterPassed.Value = !model.FilterPassed;
                    break;

                case TestStatus.Failed:
                    model.FilterFailed.Value = !model.FilterFailed;
                    break;

                case TestStatus.Inconclusive:
                    model.FilterInconclusive.Value = !model.FilterInconclusive;
                    break;
            }
            eventAggregator.Send(new FilterTestStatusEvent(testStatus));
        }
    }
}
