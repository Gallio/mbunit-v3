using System;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Gallio.MSTestRunner;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.Vsip;
using Microsoft.VisualStudio;

namespace Gallio.MSTestRunner
{
    [PackageRegistration(UseManagedResourcesOnly = true, RegisterUsing=RegistrationMethod.Assembly)]
        // Note: can't register by CodeBase because the Tip loader assumes the assembly can be resolved by name.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
    [InstalledProductRegistration(true, null, null, null)]
    [ProvideLoadKey("Standard", "3.0", "Gallio.MSTestRunner", "Gallio Project", ResourceIds.ProductLoadKeyId)]
    [ProvideTip(typeof(GallioTip), typeof(SGallioTestService))]
    [ProvideServiceForTestType(typeof(GallioTestElement), typeof(SGallioTestService))]
    [RegisterTestTypeNoEditor(typeof(GallioTestElement), typeof(GallioTip), new string[] { "dll", "exe" },
        new int[] { ResourceIds.TestTypeIconId, ResourceIds.TestTypeIconId }, ResourceIds.TestTypeNameId)]
    [Guid(Guids.MSTestRunnerPkgGuidString)]
    [ComVisible(true)]
    internal sealed class GallioPackage : Package, IVsInstalledProduct
    {
        private static GallioPackage instance;

        public GallioPackage()
        {
            instance = this;
        }

        public static GallioPackage Instance
        {
            get { return instance; }
        }

        protected override void Initialize()
        {
            base.Initialize();

            ServiceCreatorCallback callback = OnCreateService;
            IServiceContainer container = GetService(typeof(IServiceContainer)) as IServiceContainer;

            if (container != null)
            {
                container.AddService(typeof(SGallioTestService), callback, true);
            }
        }

        private object OnCreateService(IServiceContainer container, Type serviceType)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            if (serviceType == typeof(SGallioTestService))
                return new GallioTuip(this);
            else
                return null;
        }

        public int IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 0;
            return VSConstants.S_OK;
        }

        public int OfficialName(out string pbstrName)
        {
            pbstrName = "Gallio MSTest Runner";
            return VSConstants.S_OK;
        }

        public int ProductID(out string pbstrPID)
        {
            pbstrPID = "Version " + GetType().Assembly.GetName().Version;
            return VSConstants.S_OK;
        }

        public int ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = "Gallio integration for Visual Studio Team System Test";
            return VSConstants.S_OK;
        }

        public int IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = ResourceIds.ProductIconId;
            return VSConstants.S_OK;
        }
    }
}