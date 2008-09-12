// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Gallio.Loader;
using Gallio.VisualStudio.Shell.Resources;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// <para>
    /// The shell package is a meta-package for all Gallio-related extensions to Visual Studio.
    /// By itself it does nothing much except to display Gallio product information in the
    /// Visual Studio About Box.  Actual functionality is contributed by other plugins
    /// that implement <see cref="IShellExtension" />.
    /// </para>
    /// <para>
    /// The shell package exposes Visual Studio services by way of its associated <see cref="IShell" />.
    /// </para>
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, RegisterUsing = RegistrationMethod.Assembly)]
    // Note: Can't register by CodeBase because the Tip loader assumes the assembly can be resolved by name
    //       which means it has to be present in the VS PrivateAssemblies.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
    [InstalledProductRegistration(true, null, null, null)]
    [ProvideLoadKey("Standard", "3.0", "Gallio", "Gallio Project", VSPackageResourceIds.ProductLoadKeyId)]
    [Guid(Guids.ShellPkgGuidString)]
    [ComVisible(true)]
    public sealed class ShellPackage : Package, IVsInstalledProduct, IShellAccessor
    {
        private static ShellPackage instance;
        private Shell shell;

        static ShellPackage()
        {
            GallioLoader.Initialize();
        }

        /// <summary>
        /// Creates the shell package.
        /// </summary>
        public ShellPackage()
        {
            instance = this;
        }

        /// <summary>
        /// Gets the singleton instance of the shell package.
        /// </summary>
        public static ShellPackage Instance
        {
            get { return instance; }
        }

        /// <inheritdoc />
        public IShell Shell
        {
            get { return shell; }
        }

        internal Shell ShellInternal
        {
            get { return shell; }
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            shell = new Shell();
            shell.OnPackageInitialized(this);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);

                if (disposing && shell != null)
                    shell.OnPackageDisposed();
            }
            finally
            {
                instance = null;
                shell = null;
            }
        }

        int IVsInstalledProduct.IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 0;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.OfficialName(out string pbstrName)
        {
            pbstrName = VSPackage.PackageName;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductID(out string pbstrPID)
        {
            pbstrPID = String.Format(VSPackage.PackageVersionFormat, GetType().Assembly.GetName().Version);
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = VSPackage.PackageDescription;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = VSPackageResourceIds.ProductIconId;
            return VSConstants.S_OK;
        }
    }
}
