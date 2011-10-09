using System.Windows.Forms;
using Gallio.Icarus.WindowManager;
using MbUnit.Framework;
using NHamcrest.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus.Tests.WindowManager
{
    public class WindowManagerTests
    {
        [Test]
        public void Show_defaults()
        {
            var windowManager = new Icarus.WindowManager.WindowManager(null);
            windowManager.SetDockPanel(new DockPanel());
            const string identifier = "id";
            windowManager.Register(identifier, () => windowManager.Add(identifier, new Control(), "caption"), Location.Left);
            
            windowManager.ShowDefaults();

            var window = windowManager.Get(identifier);
            Assert.That(window.DockState, Is.EqualTo(DockState.DockLeft));
        }
    }
}