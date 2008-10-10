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

using System.Runtime.InteropServices;
using Gallio.Loader;
using Gallio.VisualStudio.Shell.Resources;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Gallio.VisualStudio.Shell.UI;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.OLE.Interop;
using System;

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
    public interface IShellPackage : IVsInstalledProduct, IVsPackage, Microsoft.VisualStudio.OLE.Interop.IServiceProvider, System.IServiceProvider, IOleCommandTarget, IVsPersistSolutionOpts, IServiceContainer, IVsUserSettings, IVsUserSettingsMigration, IVsToolWindowFactory
    {
        /// <summary>
        /// Creates the specified COM object using the Visual Studio's local registry CLSID object.
        /// </summary>
        /// <param name="clsid">The CLSID of the object to create.</param>
        /// <param name="iid">The interface IID the object implements.</param>
        /// <param name="type">The managed type of the object to return.</param>
        /// <returns>An instance of the created object.</returns>
        object CreateInstance(ref Guid clsid, ref Guid iid, Type type);

        /// <summary>
        /// Gets the tool window corresponding to the specified type and ID.
        /// </summary>
        /// <param name="toolWindowType">The type of tool window to create.</param>
        /// <param name="id">The tool window ID. This is 0 for a single-instance tool window.</param>
        /// <param name="create">If true, the tool window is created if it does not exist.</param>
        /// <returns>An instance of the requested tool window. If create is false and the tool window does not exist, null is returned.</returns>
        ToolWindowPane FindToolWindow(Type toolWindowType, int id, bool create);

        /// <summary>
        /// Gets the requested output window.
        /// </summary>
        /// <param name="page">The GUID corresponding to the pane. (See Microsoft.VisualStudio.VSConstants class for the GUIDs which correspond to output panes.)</param>
        /// <param name="caption">The caption to create if the pane does not exist.</param>
        /// <returns>The Microsoft.VisualStudio.Shell.Interop.IVsOutputWindowPane interface. Returns null in case of failure.</returns>
        IVsOutputWindowPane GetOutputPane(Guid page, string caption);
        
        /// <summary>
        /// Returns the locale associated with this service provider.
        /// </summary>
        /// <returns>Returns the locale identifier for the service provider.</returns>
        int GetProviderLocale();

        /// <summary>
        /// Displays a specified tools options page.
        /// </summary>
        /// <param name="optionsPageType">The options page to open. The options page is identified by the GUID of the optionsPageType object passed in.</param>
         void ShowOptionPage(Type optionsPageType);
    }
}
