using System.Drawing;
using Gallio.Common;
using Gallio.Icarus.RuntimeLog;
using Gallio.Icarus.WindowManager;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Menus;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.RuntimeLog
{
    public class RuntimeLogPackageTests
    {
        private IWindowManager windowManager;
        private IMenuManager menuManager;
        private RuntimeLogPackage runtimeLogPackage;
        private MenuCommand menuCommand;

        [SetUp]
        public void SetUp()
        {
            windowManager = MockRepository.GenerateStub<IWindowManager>();
            windowManager.Stub(wm => wm.Register(Arg<string>.Is.Anything, Arg<Action>.Is.Anything, Arg<Location>.Is.Anything))
                .Do((Action<string, Action, Location>)((i, a, l) => a()));

            menuManager = MockRepository.GenerateStub<IMenuManager>();
            windowManager.Stub(wm => wm.MenuManager).Return(menuManager);
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Func<MenuCommand>>.Is.Anything))
                .Do((Action<string, Func<MenuCommand>>)((m, f) => menuCommand = f()));

            var runtimeLogController = MockRepository.GenerateStub<IRuntimeLogController>();

            runtimeLogPackage = new RuntimeLogPackage(windowManager, runtimeLogController);
        }

        [Test]
        public void Load_registers_action_with_correct_id()
        {
            runtimeLogPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg.Is(RuntimeLogPackage.WindowId), 
                Arg<Action>.Is.Anything, Arg<Location>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_to_be_added()
        {
            runtimeLogPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Add(Arg.Is(RuntimeLogPackage.WindowId), Arg<RuntimeLogWindow>.Is.Anything,
                Arg.Is(RuntimeLogResources.Runtime_Log), Arg<Icon>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_with_correct_default_location()
        {
            runtimeLogPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg<string>.Is.Anything, Arg<Action>.Is.Anything, Arg.Is(Location.Bottom)));
        }

        [Test]
        public void Load_adds_menu_item_to_correct_menu()
        {
            runtimeLogPackage.Load();

            menuManager.AssertWasCalled(mm => mm.Add(Arg.Is("View"), Arg<Func<MenuCommand>>.Is.Anything));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_text()
        {
            runtimeLogPackage.Load();

            Assert.That(menuCommand, Is.NotNull());
            Assert.That(menuCommand.Text, Is.EqualTo(RuntimeLogResources.Runtime_Log));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_command()
        {
            runtimeLogPackage.Load();

            Assert.That(menuCommand, Is.NotNull());
            menuCommand.Command.Execute(NullProgressMonitor.CreateInstance());
            windowManager.AssertWasCalled(wm => wm.Show(RuntimeLogPackage.WindowId));
        }
    }
}