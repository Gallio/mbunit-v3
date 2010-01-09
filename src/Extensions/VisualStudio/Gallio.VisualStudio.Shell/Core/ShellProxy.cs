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
using Gallio.Loader;
using Gallio.Runtime;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Initializes the Gallio runtime and the <see cref="DefaultShell" />
    /// when both the Shell Add-In and Shell Package have been installed and activated.
    /// </summary>
    public class ShellProxy
    {
        private static readonly ShellProxy instance = new ShellProxy();

        private ShellAddInHandler addInHandler;
        private ShellPackage package;
        private ShellHolder holder;

        internal delegate void HookAccessor(ShellHooks hooks);

        private ShellProxy()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the proxy.
        /// </summary>
        public static ShellProxy Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Gets the add-in handler, or null if the add-in is not connected.
        /// </summary>
        public ShellAddInHandler AddInHandler
        {
            get { return addInHandler; }
        }

        /// <summary>
        /// Gets the add-in handler, or null if the package is not initialized.
        /// </summary>
        public ShellPackage Package
        {
            get { return package; }
        }

        /// <summary>
        /// Called when the Shell Add-In has been connected.
        /// </summary>
        /// <param name="addInHandler">The add-in handler.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="addInHandler"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a different <paramref name="addInHandler"/> has already reported being connected.</exception>
        public void AddInConnected(ShellAddInHandler addInHandler)
        {
            if (addInHandler == null)
                throw new ArgumentNullException("addInHandler");

            ShellLock.WithWriterLock(() =>
            {
                if (this.addInHandler != null)
                {
                    if (this.addInHandler == addInHandler)
                        return;

                    throw new InvalidOperationException("Multiple add-in handlers appear to be attempting to activate the shell.");
                }

                this.addInHandler = addInHandler;

                UpdateShellActivationWithWriterLockHeld();
            });
        }

        /// <summary>
        /// Called when the Shell Add-In has been disconnected.
        /// </summary>
        public void AddInDisconnected()
        {
            ShellLock.WithWriterLock(() =>
            {
                addInHandler = null;

                UpdateShellActivationWithWriterLockHeld();
            });
        }

        /// <summary>
        /// Called when the Shell package has been initialized.
        /// </summary>
        /// <param name="package">The shell package</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="package"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a different <paramref name="package"/> has already reported being initialized.</exception>
        public void PackageInitialized(ShellPackage package)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            ShellLock.WithWriterLock(() =>
            {
                if (this.package != null)
                {
                    if (this.package == package)
                        return;

                    throw new InvalidOperationException(
                        "Multiple packages appear to be attempting to activate the shell.");
                }

                this.package = package;

                UpdateShellActivationWithWriterLockHeld();
            });
        }

        /// <summary>
        /// Called when the Shell package has been disposed.
        /// </summary>
        public void PackageDisposed()
        {
            ShellLock.WithWriterLock(() =>
            {
                package = null;

                UpdateShellActivationWithWriterLockHeld();
            });
        }

        internal void InvokeHook(HookAccessor accessor)
        {
            ShellLock.WithReaderLock(() =>
            {
                if (holder != null)
                    holder.InvokeHook(accessor);
            });
        }

        private void UpdateShellActivationWithWriterLockHeld()
        {
            if (package != null && addInHandler != null)
            {
                SetupRuntime();

                if (holder == null)
                    holder = ShellHolder.Initialize(package, addInHandler);
            }
            else
            {
                if (holder != null)
                {
                    try
                    {
                        holder.Dispose();
                    }
                    finally
                    {
                        holder = null;
                    }
                }
            }
        }

        private static void SetupRuntime()
        {
            GallioLoader.Initialize().SetupRuntime();
        }

        private sealed class ShellHolder : IDisposable
        {
            private readonly DefaultShell shell;

            private ShellHolder(DefaultShell shell)
            {
                this.shell = shell;
            }

            public static ShellHolder Initialize(ShellPackage shellPackage, ShellAddInHandler shellAddInHandler)
            {
                DefaultShell shell = ShellAccessor.Instance;
                shell.Initialize(shellPackage, shellAddInHandler);
                return new ShellHolder(shell);
            }

            public void Dispose()
            {
                shell.Shutdown();
            }

            public void InvokeHook(HookAccessor accessor)
            {
                accessor(shell.ShellHooks);
            }
        }
    }
}
