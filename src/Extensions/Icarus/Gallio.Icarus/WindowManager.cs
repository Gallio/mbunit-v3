namespace Gallio.Icarus
{
    public class WindowManager : IWindowManager
    {
        public WindowManager(IStatusStrip statusStrip, IWindowCollection windowCollection, 
            IToolStripManager toolStripManager, IMenuManager menuManager)
        {
            StatusStrip = statusStrip;
            Windows = windowCollection;
            ToolStripManager = toolStripManager;
            MenuManager = menuManager;
        }

        public IStatusStrip StatusStrip { get; private set; }

        public IWindowCollection Windows { get; private set; }

        public IToolStripManager ToolStripManager { get; private set; }

        public IMenuManager MenuManager { get; private set; }
    }
}