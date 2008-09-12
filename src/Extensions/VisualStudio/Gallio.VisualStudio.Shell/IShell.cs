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
using System.Text;
using EnvDTE;
using EnvDTE80;
using Gallio.VisualStudio.Shell.Actions;

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// Provides services for integration with Visual Studio.
    /// </summary>
    public interface IShell
    {
        /// <summary>
        /// Gets the Visual Studio DTE.
        /// </summary>
        DTE2 DTE { get; }

        /// <summary>
        /// Gets the action manager.
        /// </summary>
        IActionManager ActionManager { get; }

        /// <summary>
        /// Gets the package, or null if not initialized.
        /// </summary>
        ShellPackage Package { get; }

        /// <summary>
        /// Gets the add-in, or null if not initialized.
        /// </summary>
        AddIn AddIn { get; }

        /// <summary>
        /// Gets a Visual Studio service.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>The service object</returns>
        object GetVsService(Type serviceType);

        /// <summary>
        /// Gets a Visual Studio service.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>The service object</returns>
        /// <typeparam name="T">The interface type</typeparam>
        T GetVsService<T>(Type serviceType);

        /// <summary>
        /// Proffers a Visual Studio service.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <param name="factory">The service factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/>
        /// or <paramref name="factory"/> is null</exception>
        void ProfferVsService(Type serviceType, Func<object> factory);
    }
}
