// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides Visual Studio services.
    /// </summary>
    public class VisualStudioServiceProvider : IServiceProvider
    {
        private static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");

        private readonly IOleServiceProvider oleServiceProvider;

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

            this.oleServiceProvider = oleServiceProvider;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return GetServiceImpl(serviceType);
        }

        /// <summary>
        /// Gets a service given its SVs* service and IVs* interface type.
        /// </summary>
        /// <typeparam name="TService">The SVs* service type.</typeparam>
        /// <typeparam name="TInterface">The IVs* interface type.</typeparam>
        /// <returns>The service.</returns>
        public TInterface GetService<TService, TInterface>()
        {
            return (TInterface)GetServiceImpl(typeof(TService));
        }

        private object GetServiceImpl(Type serviceType)
        {
            return GetServiceImpl(serviceType.GUID);
        }

        private object GetServiceImpl(Guid serviceGuid)
        {
            if (serviceGuid != Guid.Empty)
            {
                IntPtr pUnk = IntPtr.Zero;
                try
                {
                    Guid unknownGuid = IID_IUnknown;
                    if (oleServiceProvider.QueryService(ref serviceGuid, ref unknownGuid, out pUnk) >= 0
                        && (pUnk != IntPtr.Zero))
                        return Marshal.GetObjectForIUnknown(pUnk);
                }
                finally
                {
                    Marshal.Release(pUnk);
                }
            }

            return null;
        }
    }
}
