using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Properties;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;

namespace Gallio.Icarus.Annotations
{
    public class AnnotationsPackage : IPackage
    {
        private readonly IWindowManager windowManager;
        private readonly IAnnotationsController annotationsController;
        private readonly ISourceCodeController sourceCodeController;

        public const string WindowId = "Gallio.Icarus.Annotations";

        public AnnotationsPackage(IWindowManager windowManager, IAnnotationsController annotationsController, ISourceCodeController sourceCodeController)
    	{
    	    this.windowManager = windowManager;
    	    this.annotationsController = annotationsController;
    	    this.sourceCodeController = sourceCodeController;
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
                var view = new AnnotationsWindow(annotationsController, sourceCodeController);
                windowManager.Add(WindowId, view, Resources.AnnotationsPackage_Annotations);
            }, Location.Bottom);
        }

        private void AddMenuItem()
        {
            windowManager.MenuManager.Add("View", () => new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = Resources.AnnotationsPackage_Annotations,
            });
        }

        public void Dispose() { }
    }
}