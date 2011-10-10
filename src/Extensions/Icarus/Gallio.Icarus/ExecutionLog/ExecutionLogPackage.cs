using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Properties;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;

namespace Gallio.Icarus.ExecutionLog
{
    public class ExecutionLogPackage : IPackage
    {
        private readonly IWindowManager windowManager;
        private readonly IExecutionLogController executionLogController;
        private readonly IOptionsController optionsController;

        public const string WindowId = "Gallio.Icarus.ExecutionLog";

        public ExecutionLogPackage(IWindowManager windowManager, IExecutionLogController executionLogController, IOptionsController optionsController)
        {
            this.windowManager = windowManager;
            this.executionLogController = executionLogController;
            this.optionsController = optionsController;
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
                var view = new ExecutionLogWindow(executionLogController, optionsController);
                windowManager.Add(WindowId, view, Resources.ExecutionLogPackage_Execution_Log);
            }, Location.Centre);
        }

        private void AddMenuItem()
        {
            windowManager.MenuManager.Add("View", () => new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = Resources.ExecutionLogPackage_Execution_Log,
            });
        }

        public void Dispose() { }         
    }
}