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
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;
using Gallio.Runtime;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Default implementation of the shell.
    /// </summary>
    public class DefaultShell : IShell
    {
        private readonly ComponentHandle<IShellExtension, ShellExtensionTraits>[] extensionHandles;
        private readonly IRuntime runtime;
        private readonly ShellHooks shellHooks;

        private ShellPackage shellPackage;
        private ShellAddInHandler shellAddInHandler;
        private ILogger logger;

        /// <summary>
        /// Creates an uninitialized shell.
        /// </summary>
        /// <param name="extensionHandles">The array of shell extensions handles.</param>
        /// <param name="runtime">The runtime.</param>
        public DefaultShell(ComponentHandle<IShellExtension, ShellExtensionTraits>[] extensionHandles, IRuntime runtime)
        {
            this.extensionHandles = extensionHandles;
            this.runtime = runtime;

            shellHooks = new ShellHooks();
        }

        /// <inheritdoc />
        public bool IsInitialized
        {
            get { return shellPackage != null; }
        }

        /// <inheritdoc />
        public DTE2 DTE
        {
            get
            {
                ThrowIfNotInitialized();
                return shellAddInHandler.DTE;
            }
        }
        object IShell.DTE { get { return DTE; } }

        /// <inheritdoc />
        public ShellPackage ShellPackage
        {
            get
            {
                ThrowIfNotInitialized();
                return shellPackage;
            }
        }
        object IShell.ShellPackage { get { return ShellPackage; } }

        /// <inheritdoc />
        public AddIn ShellAddIn
        {
            get
            {
                ThrowIfNotInitialized();
                return shellAddInHandler.AddIn;
            }
        }
        object IShell.ShellAddIn { get { return ShellAddIn; } }

        /// <inheritdoc />
        public ShellAddInHandler ShellAddInHandler
        {
            get
            {
                ThrowIfNotInitialized();
                return shellAddInHandler;
            }
        }
        object IShell.ShellAddInHandler { get { return ShellAddInHandler; } }

        /// <inheritdoc />
        public ILogger Logger
        {
            get
            {
                ThrowIfNotInitialized();
                return logger;
            }
        }

        /// <inheritdoc />
        public IServiceProvider VsServiceProvider
        {
            get
            {
                ThrowIfNotInitialized();
                return shellPackage;
            }
        }

        /// <summary>
        /// Gets the Shell hooks with which to install handlers for Visual Studio events.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the Shell has not been initialized.</exception>
        public ShellHooks ShellHooks
        {
            get
            {
                ThrowIfNotInitialized();
                return shellHooks;
            }
        }

        /// <inheritdoc />
        public object GetVsService(Type serviceType)
        {
            ThrowIfNotInitialized();
            return VsServiceProvider.GetService(serviceType);
        }

        /// <inheritdoc />
        public T GetVsService<T>(Type serviceInterface)
        {
            return (T)GetVsService(serviceInterface);
        }

        /// <inheritdoc />
        public void ProfferVsService(Type serviceType, ServiceFactory factory)
        {
            IServiceContainer container = GetVsService<IServiceContainer>(typeof(IServiceContainer));
            if (container != null)
            {
                ServiceCreatorCallback callback = delegate {
                    try
                    {
                        return factory();
                    }
                    catch (Exception ex)
                    {
                        logger.Log(LogSeverity.Error,
                            string.Format("Failed to create service '{0}'.", serviceType),
                            ex);
                        throw;
                    }
                };
                container.AddService(serviceType, callback, true);
            }
        }

        internal void Initialize(ShellPackage shellPackage, ShellAddInHandler shellAddInHandler)
        {
            if (IsInitialized)
                Shutdown();

            this.shellPackage = shellPackage;
            this.shellAddInHandler = shellAddInHandler;
            logger = new ShellLogger(shellPackage);

            runtime.AddLogListener(logger);

            foreach (var extensionHandle in extensionHandles)
            {
                try
                {
                    extensionHandle.GetComponent().Initialize();
                }
                catch (Exception ex)
                {
                    logger.Log(LogSeverity.Error,
                        string.Format("Failed to initialize shell extension '{0}'.", extensionHandle.Id),
                        ex);
                }
            }
        }

        internal void Shutdown()
        {
            try
            {
                if (IsInitialized)
                {
                    foreach (var extensionHandle in extensionHandles)
                    {
                        try
                        {
                            extensionHandle.GetComponent().Shutdown();
                        }
                        catch (Exception ex)
                        {
                            logger.Log(LogSeverity.Error,
                                string.Format("Failed to shutdown shell extension '{0}'.", extensionHandle.Id),
                                ex);
                        }
                    }
                }
            }
            finally
            {
                if (logger != null)
                    runtime.RemoveLogListener(logger);

                logger = null;
                shellPackage = null;
                shellAddInHandler = null;
            }
        }

        private void ThrowIfNotInitialized()
        {
            if (! IsInitialized)
                throw new InvalidOperationException("The Shell has not been initialized.");
        }
    }
}
