using Gallio.Icarus.Commands;
using Gallio.Icarus.ExtensionSample.Properties;
using Gallio.Icarus.Runtime;
using Gallio.Icarus.WindowManager;
using Gallio.Runtime;
using Gallio.UI.Menus;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.ExtensionSample
{
    public class Package : IPackage
    {
        private readonly IWindowManager windowManager;
        private readonly IPluginScanner pluginScanner;
        private readonly ITaskManager taskManager;
        private IController controller;
        private const string WindowId = "Gallio.Icarus.ExtensionSample.View";

        public Package(IWindowManager windowManager, IPluginScanner pluginScanner, ITaskManager taskManager)
        {
            this.windowManager = windowManager;
            this.pluginScanner = pluginScanner;
            this.taskManager = taskManager;
        }

        public void Load()
        {
            RegisterComponents();
            RegisterWindow();
            AddMenuItems();
        }

        private void RegisterComponents()
        {
            pluginScanner.Scan("Gallio.Icarus.ExtensionSample", typeof(Package).Assembly);
            controller = RuntimeAccessor.ServiceLocator.Resolve<IController>();
        }

        private void RegisterWindow()
        {
            // register an action to create the window on demand
            windowManager.Register(WindowId, () =>
            {
                var pluginBrowserControl = new View(controller);
                const string caption = "A View";
                windowManager.Add(WindowId, pluginBrowserControl, caption, Resources.Sample);
            });
        }

        private void AddMenuItems()
        {
            var menuManager = windowManager.MenuManager;
            const string menuId = "Extension Sample";

            menuManager.Add(menuId, () => new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = "Show View",
                Image = Resources.Sample.ToBitmap(),
            });

            const string queueId = "ExtensionSample";
            var doWorkCommand = new DelegateCommand(pm2 => controller.DoSomeWork());

            menuManager.Add(menuId, () => new MenuCommand
            {
                Command = new DelegateCommand(pm => taskManager.QueueTask(queueId, doWorkCommand)),
                Text = "Do Some Work",
                Shortcut = "Ctrl + D"
            });
        }

        public void Dispose()
        {
        }
    }
}
