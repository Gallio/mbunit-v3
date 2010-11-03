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
        private const string WindowId = "Gallio.Icarus.ExtensionSample.View";

        public MyPackage(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        public void Load()
        {
            RegisterComponents();
            RegisterWindow();
            AddMenuItem();
        }

        private static void RegisterComponents()
        {
            var scanner = new DefaultConventionScanner(RuntimeAccessor.Registry, "Gallio.Icarus.ExtensionSample");
            scanner.Scan(typeof(MyPackage).Assembly);
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
