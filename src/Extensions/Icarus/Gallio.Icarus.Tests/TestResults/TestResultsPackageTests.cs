using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.TestResults;
using Gallio.Icarus.Tests.WindowManager;
using Gallio.Icarus.WindowManager;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Menus;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;
using Action = Gallio.Common.Action;

namespace Gallio.Icarus.Tests.TestResults
{
    public class TestResultsPackageTests
    {
        private IWindowManager windowManager;
        private IMenuManager menuManager;
        private TestResultsPackage package;
        private MenuCommand menuCommand;

        [SetUp]
        public void SetUp()
        {
            windowManager = TestWindowManager.Create();

            menuManager = MockRepository.GenerateStub<IMenuManager>();
            windowManager.Stub(wm => wm.MenuManager).Return(menuManager);
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Common.Func<MenuCommand>>.Is.Anything))
                .Do((Common.Action<string, Common.Func<MenuCommand>>)((m, f) => menuCommand = f()));

            var testResultsController = MockRepository.GenerateStub<ITestResultsController>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testStatistics = new TestStatistics();

            package = new TestResultsPackage(windowManager, testResultsController, optionsController, testTreeModel, testStatistics);
        }

        [Test]
        public void Load_registers_action_with_correct_id()
        {
            package.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg.Is(TestResultsPackage.WindowId), 
                Arg<Action>.Is.Anything, Arg<Location>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_to_be_added()
        {
            package.Load();

            windowManager.AssertWasCalled(wm => wm.Add(Arg.Is(TestResultsPackage.WindowId), Arg<Icarus.TestResults.TestResults>.Is.Anything,
                Arg.Is(TestResultsResources.Test_Results)));
        }

        [Test]
        public void Load_registers_window_with_correct_default_location()
        {
            package.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg<string>.Is.Anything, 
                Arg<Action>.Is.Anything, Arg.Is(Location.Centre)));
        }

        [Test]
        public void Load_adds_menu_item_to_correct_menu()
        {
            package.Load();

            menuManager.AssertWasCalled(mm => mm.Add(Arg.Is("View"), Arg<Common.Func<MenuCommand>>.Is.Anything));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_text()
        {
            package.Load();

            Assert.That(menuCommand, Is.NotNull());
            Assert.That(menuCommand.Text, Is.EqualTo(TestResultsResources.Test_Results));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_command()
        {
            package.Load();

            Assert.That(menuCommand, Is.NotNull());
            menuCommand.Command.Execute(NullProgressMonitor.CreateInstance());
            windowManager.AssertWasCalled(wm => wm.Show(TestResultsPackage.WindowId));
        }
    }
}
