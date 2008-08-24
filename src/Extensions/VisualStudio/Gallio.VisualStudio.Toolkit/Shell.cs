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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Gallio.VisualStudio.Toolkit.Actions;

namespace Gallio.VisualStudio.Toolkit
{
    /// <summary>
    /// The shell provides the container for all components managed by an add-in.
    /// It is also the top-level service provider used to initialize the add-in.
    /// </summary>
    [ComVisible(true)]
    public class Shell : IDTExtensibility2, IDTCommandTarget
    {
        private readonly ActionManager actionManager;

        private AddIn addIn;
        private DTE2 dte;

        public Shell()
        {
            actionManager = new ActionManager(this);
        }

        public AddIn AddIn
        {
            get { return addIn; }
        }

        public DTE2 DTE
        {
            get { return dte; }
        }

        public ActionManager ActionManager
        {
            get { return actionManager; }
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface.
        /// Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public virtual void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            addIn = (AddIn)addInInst;
            dte = (DTE2)application;

            if (connectMode == ext_ConnectMode.ext_cm_AfterStartup
                || connectMode == ext_ConnectMode.ext_cm_Startup)
                actionManager.Install();
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface.
        /// Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public virtual void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            if (disconnectMode == ext_DisconnectMode.ext_dm_UserClosed)
                actionManager.Uninstall();
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
        /// Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public virtual void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface.
        /// Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public virtual void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
        /// Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public virtual void OnBeginShutdown(ref Array custom)
        {
        }

        public virtual void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            IDTCommandTarget target = ActionManager;
            target.QueryStatus(commandName, neededText, ref statusOption, ref commandText);
        }

        public virtual void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            IDTCommandTarget target = ActionManager;
            target.Exec(commandName, executeOption, ref variantIn, ref variantOut, ref handled);
        }
    }
}
