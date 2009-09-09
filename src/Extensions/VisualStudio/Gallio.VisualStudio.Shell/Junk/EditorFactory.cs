using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gallio.VisualStudio.Shell.Junk
{
    [ComVisible(true)]
    private sealed class EditorFactory : IVsEditorFactory
    {
        private readonly DefaultToolWindowManager manager;
        private readonly Guid editorGuid;
        private bool registered;
        private uint registrationCookie;
        private ServiceProvider serviceProvider;

        public EditorFactory(DefaultToolWindowManager manager, Guid editorGuid)
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
