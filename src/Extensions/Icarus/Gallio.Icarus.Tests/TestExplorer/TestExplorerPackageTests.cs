using System.Drawing;
using Gallio.Common;
using Gallio.Icarus.TestExplorer;
using Gallio.Icarus.Tests.WindowManager;
using Gallio.Icarus.WindowManager;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Menus;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.TestExplorer
{
    public class TestExplorerPackageTests
    {
        private IWindowManager windowManager;
        private IMenuManager menuManager;
        private TestExplorerPackage package;
        private MenuCommand menuCommand;

        [SetUp]
        public void SetUp()
        {
            windowManager = TestWindowManager.Create();

            menuManager = MockRepository.GenerateStub<IMenuManager>();
            windowManager.Stub(wm => wm.MenuManager).Return(menuManager);
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Func<MenuCommand>>.Is.Anything))
                .Do((Action<string, Func<MenuCommand>>)((m, f) => menuCommand = f()));

        	var testExplorerController = MockRepository.GenerateStub<ITestExplorerController>();
        	var testExplorerModel = MockRepository.GenerateStub<ITestExplorerModel>();
        	
			package = new TestExplorerPackage(windowManager, testExplorerController, testExplorerModel);
        }

        [Test]
        public void Load_registers_action_with_correct_id()
        {
            package.Load();

			windowManager.AssertWasCalled(wm => wm.Register(Arg.Is(TestExplorerPackage.WindowId), 
                Arg<Action>.Is.Anything, Arg<Location>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_to_be_added()
        {
            package.Load();

			windowManager.AssertWasCalled(wm => wm.Add(Arg.Is(TestExplorerPackage.WindowId), Arg<TestExplorerView>.Is.Anything,
                Arg.Is(TestExplorerResources.Test_Explorer), Arg<Icon>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_with_correct_default_location()
        {
            package.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg<string>.Is.Anything, 
                Arg<Action>.Is.Anything, Arg.Is(Location.Left)));
        }

        [Test]
        public void Load_adds_menu_item_to_correct_menu()
        {
            package.Load();

            menuManager.AssertWasCalled(mm => mm.Add(Arg.Is("View"), Arg<Func<MenuCommand>>.Is.Anything));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_text()
        {
            package.Load();

            Assert.That(menuCommand, Is.NotNull());
            Assert.That(menuCommand.Text, Is.EqualTo(TestExplorerResources.Test_Explorer));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_command()
        {
            package.Load();

            Assert.That(menuCommand, Is.NotNull());
            menuCommand.Command.Execute(NullProgressMonitor.CreateInstance());
            windowManager.AssertWasCalled(wm => wm.Show(TestExplorerPackage.WindowId));
        }
    }
}
