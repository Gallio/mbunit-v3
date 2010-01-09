using Aga.Controls.Tree;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.TestExplorer;
using Gallio.Model;
using MbUnit.Framework;
using Rhino.Mocks;
using SortOrder=Gallio.Icarus.Models.SortOrder;

namespace Gallio.Icarus.Tests.TestExplorer
{
    [TestsOn(typeof(Controller))]
    public class ControllerTest
    {
        private IEventAggregator eventAggregator;
        private Controller controller;
        private Icarus.TestExplorer.Model model;

        [SetUp]
        public void SetUp()
        {
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            model = new Icarus.TestExplorer.Model(MockRepository.GenerateStub<ITreeModel>());
            controller = new Controller(model, eventAggregator);
        }

        [Test]
        public void SortTree_should_send_a_SortTreeEvent()
        {
            const SortOrder sortOrder = SortOrder.Ascending;
            controller.SortTree(sortOrder);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<SortTreeEvent>.Matches(ste => 
                ste.SortOrder == sortOrder)));
        }

        [Test]
        public void Filter_passed_status_should_update_the_model_when_not_set()
        {
            Assert.IsFalse(model.FilterPassed);

            controller.FilterStatus(TestStatus.Passed);

            Assert.IsTrue(model.FilterPassed);
        }

        [Test]
        public void Filter_passed_status_should_update_the_model_when_already_set()
        {
            model.FilterPassed.Value = true;

            controller.FilterStatus(TestStatus.Passed);
            
            Assert.IsFalse(model.FilterPassed);
        }

        [Test]
        public void Filter_failed_status_should_update_the_model_when_not_set()
        {
            Assert.IsFalse(model.FilterFailed);

            controller.FilterStatus(TestStatus.Failed);

            Assert.IsTrue(model.FilterFailed);
        }

        [Test]
        public void Filter_failed_status_should_update_the_model_when_already_set()
        {
            model.FilterFailed.Value = true;

            controller.FilterStatus(TestStatus.Failed);

            Assert.IsFalse(model.FilterFailed);
        }

        [Test]
        public void Filter_inconclusive_status_should_update_the_model_when_not_set()
        {
            Assert.IsFalse(model.FilterInconclusive);

            controller.FilterStatus(TestStatus.Inconclusive);

            Assert.IsTrue(model.FilterInconclusive);
        }

        [Test]
        public void Filter_inconclusive_status_should_update_the_model_when_already_set()
        {
            model.FilterInconclusive.Value = true;

            controller.FilterStatus(TestStatus.Inconclusive);

            Assert.IsFalse(model.FilterInconclusive);
        }

        [Test]
        public void Filter_status_should_send_FilterTestStatusEvent()
        {
            const TestStatus testStatus = TestStatus.Inconclusive;
            controller.FilterStatus(testStatus);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<FilterTestStatusEvent>.Matches(ftse => 
                ftse.TestStatus == testStatus)));
        }
    }
}
