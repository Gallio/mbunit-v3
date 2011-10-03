using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;

namespace Gallio.Icarus.Filters
{
    public class FiltersPackage : IPackage
    {
        private readonly IWindowManager windowManager;
        private readonly IFilterController filterController;
        private readonly IProjectController projectController;
        public const string WindowId = "Gallio.Icarus.Filters";

        public FiltersPackage(IWindowManager windowManager, IFilterController filterController, IProjectController projectController)
        {
            this.windowManager = windowManager;
            this.filterController = filterController;
            this.projectController = projectController;
        }

        public void Load()
        {
            RegisterWindow();
            AddMenuItem();
        }

        private void RegisterWindow()
        {
            windowManager.Register(WindowId, () =>
            {
                var view = new FiltersView(filterController, projectController);
                const string caption = "Filters";
                windowManager.Add(WindowId, view, caption);
            });
        }

        private void AddMenuItem()
        {
            windowManager.MenuManager.Add("View", () => new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = "Test Filters",
                //Shortcut = "Ctrl + T"
            });
        }

        public void Dispose()
        {
            
        }
    }
}