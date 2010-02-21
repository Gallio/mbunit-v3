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
        private readonly IModel model;
        private const string WindowId = "Gallio.Icarus.Search";

        public Package(IWindowManager windowManager, IController controller, 
            IModel model)
        {
            this.windowManager = windowManager;
            this.controller = controller;
            this.model = model;
        }

        public void Load()
        {
            RegisterWindow();

            AddMenuItem();
        }

        private void AddMenuItem()
        {
            var menu = windowManager.MenuManager.GetMenu("Tools");

            var menuCommand = new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = Resources.Search_Package_AddMenuItem_Search,
                Shortcut = "Ctrl + F"
            };

            menu.Add(menuCommand);
        }

        private void RegisterWindow()
        {
            windowManager.Register(WindowId, () =>
            {
                var view = new View(controller, model);
                var caption = Resources.Search_Package_AddMenuItem_Search;
                windowManager.Add(WindowId, view, caption);
            });
        }

        public void Dispose() { }
    }
}
