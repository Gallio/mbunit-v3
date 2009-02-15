// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using EnvDTE80;
using Extensibility;
using Gallio.Loader;

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// The Shell add-in functions as an adjunct to the package to provide the user with
    /// the ability to enable and disable it.  It is also used to access a few APIs of the
    /// DTE (such as AddNamedCommand) that require an add-in instance.
    /// </summary>
    [ComVisible(true)]
    public class ShellAddInHandler : IDTExtensibility2, IDTCommandTarget
    {
        private DTE2 dte;
        private AddIn addIn;
        private Shell shell;

        /// <summary>
        /// Gets the automation object.
        /// </summary>
        public DTE2 DTE
        {
            get { return dte; }
        }

        /// <summary>
        /// Gets the add-in instance.
        /// </summary>
        public AddIn AddIn
        {
            get { return addIn; }
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface.
        /// Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            dte = (DTE2)application;
            addIn = (AddIn)addInInst;

            shell = ShellAccessor.GetShellInternal(true);
            shell.OnAddInConnected(this);
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface.
        /// Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            if (shell != null)
            {
                shell.OnAddInDisconnected();
                shell = null;
            }

            addIn = null;
            dte = null;
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
        /// Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />               
        void IDTExtensibility2.OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface.
        /// Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
        /// Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnBeginShutdown(ref Array custom)
        {
        }

        void IDTCommandTarget.QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            if (shell != null)
                shell.QueryStatus(commandName, neededText, ref statusOption, ref commandText);
        }

        void IDTCommandTarget.Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            if (shell != null)
                shell.Exec(commandName, executeOption, ref variantIn, ref variantOut, ref handled);
        }
    }
}
