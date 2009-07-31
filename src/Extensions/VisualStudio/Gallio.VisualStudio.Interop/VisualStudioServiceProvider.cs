using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides Visual Studio services.
    /// </summary>
    public class VisualStudioServiceProvider : IServiceProvider
    {
        private readonly ServiceProvider serviceProvider;

        /// <summary>
        /// Creates a service provider from a Visual Studio <see cref="DTE" /> instance.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dte"/> is null.</exception>
        public VisualStudioServiceProvider(DTE dte)
            : this(dte as IOleServiceProvider)
        {
        }

        /// <summary>
        /// Creates a service provider from an OLE <see cref="IOleServiceProvider" />.
        /// </summary>
        /// <param name="oleServiceProvider">The OLE service provider.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="oleServiceProvider"/> is null.</exception>
        public VisualStudioServiceProvider(IOleServiceProvider oleServiceProvider)
        {
            if (oleServiceProvider == null)
                throw new ArgumentNullException("oleServiceProvider");

            serviceProvider = new ServiceProvider(oleServiceProvider);
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return serviceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Gets a service given its SVs* service and IVs* interface type.
        /// </summary>
        /// <typeparam name="TService">The SVs* service type.</typeparam>
        /// <typeparam name="TInterface">The IVs* interface type.</typeparam>
        /// <returns>The service.</returns>
        public TInterface GetService<TService, TInterface>()
        {
            return (TInterface)serviceProvider.GetService(typeof(TService));
        }
    }
}
