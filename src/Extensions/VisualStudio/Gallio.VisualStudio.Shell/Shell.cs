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
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Gallio.VisualStudio.Shell.Actions;
using Gallio.VisualStudio.Shell.Resources;
using Microsoft.VisualStudio.Shell.Interop;

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// Default implementation of the shell.
    /// </summary>
    public class Shell : IShell
    {
        private readonly DefaultActionManager actionManager;

        private ShellPackage package;
        private DTE2 dte;
        private AddIn addIn;
        private ShellAddInHandler addInHandler;

        public Shell()
        {
            actionManager = new DefaultActionManager(this);
        }

        /// <inheritdoc />
        public DTE2 DTE
        {
            get { return dte; }
        }

        /// <inheritdoc />
        public ShellPackage Package
        {
            get { return package; }
        }

        /// <inheritdoc />
        public AddIn AddIn
        {
            get { return addIn; }
        }

        /// <inheritdoc />
        public IActionManager ActionManager
        {
            get { return actionManager; }
        }

        /// <inheritdoc />
        public object GetVsService(Type serviceType)
        {
            IServiceProvider serviceProvider = package;
            return serviceProvider.GetService(serviceType);
        }

        /// <inheritdoc />
        public T GetVsService<T>(Type serviceInterface)
        {
            return (T)GetVsService(serviceInterface);
        }

        /// <inheritdoc />
        public void ProfferVsService(Type serviceType, Func<object> factory)
        {
            IServiceContainer container = GetVsService<IServiceContainer>(typeof(IServiceContainer));
            if (container != null)
            {
                ServiceCreatorCallback callback = delegate { return factory(); };
                container.AddService(serviceType, callback, true);
            }
        }

        internal void OnPackageInitialized(ShellPackage package)
        {
            this.package = package;

            dte = GetVsService<DTE2>(typeof(SDTE));

            if (dte != null)
            {
                string progId = typeof (ShellAddInHandler).GUID.ToString();
                addIn = dte.AddIns.Add(progId, VSPackage.PackageDescription, VSPackage.PackageName, false);
                addIn.Connected = true;
            }
        }

        internal void OnPackageDisposed()
        {
            if (addIn != null)
            {
                addIn.Remove();
                addIn = null;
            }

            package = null;
            dte = null;
        }

        internal void OnAddInConnected(ShellAddInHandler addInHandler)
        {
            this.addInHandler = addInHandler;

            if (package != null && addIn != null)
            {
                actionManager.Initialize();
            }
        }

        internal void OnAddInDisconnected()
        {
            if (package != null && addIn != null)
            {
                actionManager.Shutdown();
            }

            addInHandler = null;
        }

        internal void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            actionManager.QueryStatus(commandName, neededText, ref statusOption, ref commandText);
        }

        internal void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            actionManager.Exec(commandName, executeOption, ref variantIn, ref variantOut, ref handled);
        }
    }
}
