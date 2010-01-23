using Gallio.Icarus.Commands;
using Gallio.Icarus.Properties;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;

namespace Gallio.Icarus.Search
{
    public class Package : IPackage
    {
        private readonly IWindowManager windowManager;
        private readonly IController controller;
        private const string WindowId = "Gallio.Icarus.Search";

        public Package(IWindowManager windowManager, IController controller)
        {
            this.windowManager = windowManager;
            this.controller = controller;
        }

        public void Load()
        {
            RegisterWindow();

            AddMenuItem();
        }

        private void AddMenuItem()
        {
            var menu = windowManager.MenuManager.GetMenu("View");

            var menuCommand = new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = Resources.Search_Package_AddMenuItem_Search
            };

            menu.Add(menuCommand);
        }

        private void RegisterWindow()
        {
            // register an action to create the window on demand
            windowManager.Register(WindowId, () =>
            {
                var view = new View(controller);
                var caption = Resources.Search_Package_AddMenuItem_Search;
                windowManager.Add(WindowId, view, caption);
            });
        }
        public void Dispose() { }
    }
}
