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
