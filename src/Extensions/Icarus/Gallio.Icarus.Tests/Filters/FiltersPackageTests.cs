using System.Collections.Generic;
using Gallio.Common;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Filters;
using Gallio.Icarus.WindowManager;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;
using Gallio.UI.Menus;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Filters
{
    public class FiltersPackageTests
    {
        private IWindowManager windowManager;
        private IFilterController filterController;
        private IProjectController projectController;
        private FiltersPackage filtersPackage;
        private IMenuManager menuManager;

        [SetUp]
        public void SetUp()
        {
            windowManager = MockRepository.GenerateStub<IWindowManager>();
            windowManager.Stub(wm => wm.Register(Arg<string>.Is.Anything, Arg<Action>.Is.Anything))
                .Do((Action<string, Action>)((i, a) => a()));

            menuManager = MockRepository.GenerateStub<IMenuManager>();
            windowManager.Stub(wm => wm.MenuManager).Return(menuManager);

            filterController = MockRepository.GenerateStub<IFilterController>();

            projectController = MockRepository.GenerateStub<IProjectController>();
            projectController.Stub(pc => pc.TestFilters).Return(new Observable<IList<FilterInfo>>());

            filtersPackage = new FiltersPackage(windowManager, filterController, projectController);
        }

        [Test]
        public void Load_registers_action_with_correct_id()
        {
            filtersPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg.Is(FiltersPackage.WindowId), Arg<Action>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_to_be_added()
        {
            filtersPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Add(Arg.Is(FiltersPackage.WindowId), Arg<FiltersWindow>.Is.Anything, Arg.Is("Filters")));
        }

        [Test]
        public void Load_adds_menu_item_to_correct_menu()
        {
            filtersPackage.Load();

            menuManager.AssertWasCalled(mm => mm.Add(Arg.Is("View"), Arg<Func<MenuCommand>>.Is.Anything));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_text()
        {
            MenuCommand menuCommand = null;
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Func<MenuCommand>>.Is.Anything))
                .Do((Action<string, Func<MenuCommand>>)((m, f) => menuCommand = f()));

            filtersPackage.Load();

            Assert.That(menuCommand, Is.NotNull());
            Assert.That(menuCommand.Text, Is.EqualTo("Test Filters"));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_command()
        {
            MenuCommand menuCommand = null;
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Func<MenuCommand>>.Is.Anything))
                .Do((Action<string, Func<MenuCommand>>)((m, f) => menuCommand = f()));

            filtersPackage.Load();

            Assert.That(menuCommand, Is.NotNull());
            menuCommand.Command.Execute(NullProgressMonitor.CreateInstance());
            windowManager.AssertWasCalled(wm => wm.Show(FiltersPackage.WindowId));
        }
    }
}