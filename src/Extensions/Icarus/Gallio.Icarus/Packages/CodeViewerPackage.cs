using System.Collections.Generic;
using System.IO;
using Gallio.Common.Reflection;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.Extensibility;
using Gallio.Common.Concurrency;

namespace Gallio.Icarus.Packages
{
    internal class CodeViewerPackage : IPackage
    {
        private ISourceCodeController sourceCodeController;
        private WindowManager windowManager;
        private List<string> openWindows = new List<string>();

        public void Load(IServiceLocator serviceLocator)
        {
            sourceCodeController = serviceLocator.Resolve<ISourceCodeController>();
            windowManager = (WindowManager)serviceLocator.Resolve<IWindowManager>();

            sourceCodeController.ShowSourceCode += (sender, e) =>
            {
                string identifier = e.CodeLocation.Path ?? "(unknown)";

                if (!openWindows.Contains(e.CodeLocation.Path))
                {
                    string caption;
                    if (e.CodeLocation == CodeLocation.Unknown)
                        caption = "(unknown)";
                    else
                        caption = Path.GetFileName(e.CodeLocation.Path) ?? "(unknown)";

                    Sync.Invoke(windowManager.DockPanel, () =>
                    {
                        var window = windowManager.Add(identifier, new CodeViewer(e.CodeLocation), caption);
                        window.FormClosed += (sender2, e2) => { openWindows.Remove(identifier); };
                        windowManager.Show(identifier);
                        openWindows.Add(identifier);
                    });
                }
                else
                {
                    Sync.Invoke(windowManager.DockPanel, () =>
                    {
                        var window = windowManager.Get(identifier);
                        var codeViewer = (CodeViewer)window.Content;
                        if (e.CodeLocation != CodeLocation.Unknown)
                            codeViewer.JumpTo(e.CodeLocation.Line, e.CodeLocation.Column);
                        windowManager.Show(identifier);
                    });
                }
            };
        }

        public void Unload()
        { }
    }
}
