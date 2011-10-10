using Gallio.Common;
using Gallio.Icarus.Annotations;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.ExecutionLog;
using Gallio.Icarus.Properties;
using Gallio.Icarus.WindowManager;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Menus;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.ExecutionLog
{
    public class ExecutionLogPackageTests
    {
        private IWindowManager windowManager;
        private IMenuManager menuManager;
        
        private ExecutionLogPackage executionLogPackage;

        [SetUp]
        public void SetUp()
        {
            windowManager = MockRepository.GenerateStub<IWindowManager>();
            windowManager.Stub(wm => wm.Register(Arg<string>.Is.Anything, Arg<Action>.Is.Anything, Arg<Location>.Is.Anything))
                .Do((Action<string, Action, Location>)((i, a, l) => a()));

            menuManager = MockRepository.GenerateStub<IMenuManager>();
            windowManager.Stub(wm => wm.MenuManager).Return(menuManager);

            var executionLogController = MockRepository.GenerateStub<IExecutionLogController>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();

            executionLogPackage = new ExecutionLogPackage(windowManager, executionLogController, optionsController);
        }

        [Test]
        public void Load_registers_action_with_correct_id()
        {
            executionLogPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg.Is(ExecutionLogPackage.WindowId), Arg<Action>.Is.Anything, Arg<Location>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_to_be_added()
        {
            executionLogPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Add(Arg.Is(ExecutionLogPackage.WindowId), Arg<ExecutionLogWindow>.Is.Anything,
                Arg.Is(Resources.ExecutionLogPackage_Execution_Log)));
        }

        [Test]
        public void Load_registers_window_with_correct_default_location()
        {
            executionLogPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg<string>.Is.Anything, Arg<Action>.Is.Anything, Arg.Is(Location.Centre)));
        }

        [Test]
        public void Load_adds_menu_item_to_correct_menu()
        {
            executionLogPackage.Load();

            menuManager.AssertWasCalled(mm => mm.Add(Arg.Is("View"), Arg<Func<MenuCommand>>.Is.Anything));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_text()
        {
            MenuCommand menuCommand = null;
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Func<MenuCommand>>.Is.Anything))
                .Do((Action<string, Func<MenuCommand>>)((m, f) => menuCommand = f()));

            executionLogPackage.Load();

            Assert.That(menuCommand, Is.NotNull());
            Assert.That(menuCommand.Text, Is.EqualTo(Resources.ExecutionLogPackage_Execution_Log));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_command()
        {
            MenuCommand menuCommand = null;
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Func<MenuCommand>>.Is.Anything))
                .Do((Action<string, Func<MenuCommand>>)((m, f) => menuCommand = f()));

            executionLogPackage.Load();

            Assert.That(menuCommand, Is.NotNull());
            menuCommand.Command.Execute(NullProgressMonitor.CreateInstance());
            windowManager.AssertWasCalled(wm => wm.Show(ExecutionLogPackage.WindowId));
        }
    }
}