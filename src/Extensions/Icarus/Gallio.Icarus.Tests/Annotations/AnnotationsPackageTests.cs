using System.Drawing;
using Gallio.Common;
using Gallio.Icarus.Annotations;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.WindowManager;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Menus;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Annotations
{
    public class AnnotationsPackageTests
    {
        private IWindowManager windowManager;
        private IMenuManager menuManager;
        private IAnnotationsController annotationsController;
        private ISourceCodeController sourceCodeController;
        
        private AnnotationsPackage annotationsPackage;

        [SetUp]
        public void SetUp()
        {
            windowManager = MockRepository.GenerateStub<IWindowManager>();
            windowManager.Stub(wm => wm.Register(Arg<string>.Is.Anything, Arg<Action>.Is.Anything, Arg<Location>.Is.Anything))
                .Do((Action<string, Action, Location>)((i, a, l) => a()));

            menuManager = MockRepository.GenerateStub<IMenuManager>();
            windowManager.Stub(wm => wm.MenuManager).Return(menuManager);

            annotationsController = MockRepository.GenerateStub<IAnnotationsController>();

            sourceCodeController = MockRepository.GenerateStub<ISourceCodeController>();

            annotationsPackage = new AnnotationsPackage(windowManager, annotationsController, sourceCodeController);
        }

        [Test]
        public void Load_registers_action_with_correct_id()
        {
            annotationsPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg.Is(AnnotationsPackage.WindowId), Arg<Action>.Is.Anything, Arg<Location>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_to_be_added()
        {
            annotationsPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Add(Arg.Is(AnnotationsPackage.WindowId), Arg<AnnotationsWindow>.Is.Anything, 
                Arg.Is(AnnotationsResources.Annotations), Arg<Icon>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_with_correct_default_location()
        {
            annotationsPackage.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg<string>.Is.Anything, Arg<Action>.Is.Anything, Arg.Is(Location.Bottom)));
        }

        [Test]
        public void Load_adds_menu_item_to_correct_menu()
        {
            annotationsPackage.Load();

            menuManager.AssertWasCalled(mm => mm.Add(Arg.Is("View"), Arg<Func<MenuCommand>>.Is.Anything));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_text()
        {
            MenuCommand menuCommand = null;
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Func<MenuCommand>>.Is.Anything))
                .Do((Action<string, Func<MenuCommand>>)((m, f) => menuCommand = f()));

            annotationsPackage.Load();

            Assert.That(menuCommand, Is.NotNull());
            Assert.That(menuCommand.Text, Is.EqualTo(AnnotationsResources.Annotations));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_command()
        {
            MenuCommand menuCommand = null;
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Func<MenuCommand>>.Is.Anything))
                .Do((Action<string, Func<MenuCommand>>)((m, f) => menuCommand = f()));

            annotationsPackage.Load();

            Assert.That(menuCommand, Is.NotNull());
            menuCommand.Command.Execute(NullProgressMonitor.CreateInstance());
            windowManager.AssertWasCalled(wm => wm.Show(AnnotationsPackage.WindowId));
        }
    }
}