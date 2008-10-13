using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Gallio.VisualStudio.Shell.UI
{
    internal class ShellWindowManager : BaseShellExtension, IWindowManager
    {
        /// <summary>
        /// Map string ids to ints.  We try to make the system a little friendlier
        /// by providing a larger id space defined by strings since we are unable to
        /// use subclasses of <see cref="ToolWindowPane" /> to create separate id namespaces.
        /// </summary>
        private readonly Dictionary<string, int> windowIdMap = new Dictionary<string, int>();

        private int nextId;

        public ShellToolWindow FindToolWindow(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            int internalId;
            if (!windowIdMap.TryGetValue(id, out internalId))
                return null;

            var pane = (ShellToolWindowPane)Shell.Package.FindToolWindow(typeof(ShellToolWindowPane), internalId, false);
            return pane.ToolWindowContainer.ToolWindow;
        }

        public void OpenToolWindow(string id, ShellToolWindow window)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (window == null)
                throw new ArgumentNullException("window");

            int internalId;
            if (!windowIdMap.TryGetValue(id, out internalId))
            {
                internalId = nextId++;
                windowIdMap.Add(id, internalId);
            }

            var pane = (ShellToolWindowPane)Shell.Package.FindToolWindow(typeof(ShellToolWindowPane), internalId, true);
            if (pane == null)
                throw new ShellException("Could not create an instance of the Shell tool window pane.");

            pane.Disposed += delegate { windowIdMap.Remove(id); };
            pane.ToolWindowContainer.ToolWindow = window;
            window.Show();
        }

        public void CloseToolWindow(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            ShellToolWindow window = FindToolWindow(id);
            if (window != null)
                window.Close();
        }

        protected override void InitializeImpl()
        {
            // TODO: register editor types
        }

        protected override void ShutdownImpl()
        {
        }

        [ComVisible(true)]
        private sealed class EditorFactory : IVsEditorFactory
        {
            private readonly ShellWindowManager manager;
            private readonly Guid editorGuid;
            private bool registered;
            private uint registrationCookie;
            private ServiceProvider serviceProvider;

            public EditorFactory(ShellWindowManager manager, Guid editorGuid)
            {
                this.manager = manager;
                this.editorGuid = editorGuid;
            }

            public void Register()
            {
                if (!registered)
                {
                    Guid editorGuid = this.editorGuid;
                    VsRegisterEditors.RegisterEditor(ref editorGuid, this, out registrationCookie);
                    registered = true;
                }
            }

            public void Unregister()
            {
                if (registered)
                {
                    registered = false;
                    VsRegisterEditors.UnregisterEditor(registrationCookie);
                }
            }

            private IVsRegisterEditors VsRegisterEditors
            {
                get { return manager.Shell.GetVsService<IVsRegisterEditors>(typeof(SVsRegisterEditors)); }
            }

            int IVsEditorFactory.CreateEditorInstance(uint grfCreateDoc, string pszMkDocument,
                string pszPhysicalView, IVsHierarchy pvHier, uint itemid, IntPtr punkDocDataExisting,
                out IntPtr ppunkDocView, out IntPtr ppunkDocData, out string pbstrEditorCaption,
                out Guid pguidCmdUI, out int pgrfCDW)
            {
                ppunkDocData = punkDocDataExisting;
                ppunkDocView = punkDocDataExisting;
                pbstrEditorCaption = string.Empty;
                pguidCmdUI = Guid.Empty;
                pgrfCDW = 0;
                return 0;
            }

            int IVsEditorFactory.SetSite(IServiceProvider psp)
            {
                serviceProvider = new ServiceProvider(psp);
                return 0;
            }

            int IVsEditorFactory.Close()
            {
                serviceProvider = null;
                return 0;
            }

            int IVsEditorFactory.MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
            {
                pbstrPhysicalView = null;
                return 0;
            }
        }
    }
}
