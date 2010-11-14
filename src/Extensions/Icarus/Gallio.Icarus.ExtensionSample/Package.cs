using Gallio.Icarus.Commands;
using Gallio.Icarus.Runtime;
using Gallio.Icarus.WindowManager;
using Gallio.Runtime;
using Gallio.UI.Menus;

namespace Gallio.Icarus.ExtensionSample
{
    public class MyPackage : IPackage
    {
        private readonly IWindowManager windowManager;
        private readonly IPluginScanner pluginScanner;
        private const string WindowId = "Gallio.Icarus.ExtensionSample.View";

        public MyPackage(IWindowManager windowManager, IPluginScanner pluginScanner)
        {
            this.windowManager = windowManager;
            this.pluginScanner = pluginScanner;
        }

        public void Load()
        {
            RegisterComponents();
            RegisterWindow();
            AddMenuItem();
        }

        private void RegisterComponents()
        {
            pluginScanner.Scan("Gallio.Icarus.ExtensionSample", typeof(MyPackage).Assembly);
        }

        private void RegisterWindow()
        {
            // register an action to create the window on demand
            windowManager.Register(WindowId, () =>
            {
                var controller = RuntimeAccessor.ServiceLocator.Resolve<IController>();
                var pluginBrowserControl = new View(controller);
                const string caption = "A View";
                windowManager.Add(WindowId, pluginBrowserControl, caption);
            });
        }

        private void AddMenuItem()
        {
            var menu = windowManager.MenuManager.GetMenu("Tools");

            var menuCommand = new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = "Extension Example"
            };

            menu.Add(menuCommand);
        }

        public void Dispose() { }
    }
}
