// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.IO;
using System.Windows.Forms;
using System.Xml.XPath;
using Gallio.Hosting;
using JetBrains.UI.Shell;

namespace Gallio.ReSharperRunner.Hosting
{
    /// <summary>
    /// Resolves and initializes the Gallio <see cref="Runtime" /> environment.
    /// This class is constructed in such a way as to conceal dependencies
    /// on the Gallio assemblies.  If for whatever reason the assemblies cannot
    /// be loaded, the runtime proxy will revert to null implementations of
    /// services to avoid generating repeated errors.
    /// </summary>
    internal static class RuntimeProxy
    {
        private const string GallioInstallationPathConfigXPath = "//GallioInstallationPath";

        private static readonly object initializationLock = new object();
        private static bool? initializationState;

        /// <summary>
        /// Returns true if the runtime has been initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get { return initializationState.GetValueOrDefault(false); }
        }

        /// <summary>
        /// Tries to initialize the runtime if not already performed.
        /// If an error occurs, displays a message to the user and returns false.
        /// </summary>
        /// <returns>True if the runtime has been initialized</returns>
        public static bool TryInitializeWithPrompt()
        {
            for (; ; )
            {
                try
                {
                    Initialize();

                    return IsInitialized;
                }
                catch (RuntimeProxyException ex)
                {
                    if (!DisplayInitializationError(ex))
                        return false;

                    initializationState = null;
                }
            }
        }

        /// <summary>
        /// Initializes the runtime if not already performed.
        /// </summary>
        /// <exception cref="RuntimeProxyException">Thrown if the runtime could not been initialized.</exception>
        public static void Initialize()
        {
            lock (initializationLock)
            {
                try
                {
                    if (initializationState.HasValue)
                    {
                        if (initializationState.Value == false)
                            throw new RuntimeProxyException("The Gallio runtime components could not be initialized.  Aborted the initialization attempt due to a previous failure.");
                    }
                    else
                    {
                        InitializeRuntime();

                        initializationState = true;
                    }
                }
                catch (Exception ex)
                {
                    initializationState = false;

                    throw new RuntimeProxyException("The Gallio runtime components could not be initialized.", ex);
                }
            }
        }

        /// <summary>
        /// Resolves a component that implements a runtime service.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>The component</returns>
        /// <exception cref="RuntimeProxyException">Thrown if the runtime has not been initialized.</exception>
        /// <exception cref="Exception">Other exceptions may be thrown if the service cannot be resolved.</exception>
        public static T Resolve<T>()
        {
            if (! IsInitialized)
                throw new RuntimeProxyException("The runtime has not been initialized.");

            return Protected.Resolve<T>();
        }

        private static string GetConfigurationFilePath()
        {
            return typeof(RuntimeProxy).Assembly.Location + ".config";
        }

        private static void InitializeRuntime()
        {
            string configFilePath = GetConfigurationFilePath();

            AssemblyLoader loader = CreateAssemblyLoaderIfNeeded(configFilePath);
            try
            {
                Protected.InitializeRuntime(configFilePath);
            }
            catch
            {
                if (loader != null)
                    loader.Dispose();

                throw;
            }
        }

        private static AssemblyLoader CreateAssemblyLoaderIfNeeded(string configFilePath)
        {
            if (IsRuntimeAssemblyAccessible())
                return null;

            Exception exception;
            try
            {
                XPathDocument doc = new XPathDocument(configFilePath);
                XPathNavigator navigator = doc.CreateNavigator().SelectSingleNode(GallioInstallationPathConfigXPath);

                if (navigator != null)
                {
                    string installationPath = navigator.Value;

                    if (!Path.IsPathRooted(installationPath))
                    {
                        string pluginBasePath = Path.GetDirectoryName(Path.GetFullPath(typeof(RuntimeProxy).Assembly.Location));

                        if (installationPath.Length == 0)
                            installationPath = pluginBasePath;
                        else
                            installationPath = Path.GetFullPath(Path.Combine(pluginBasePath, installationPath));
                    }

                    AssemblyLoader loader = new AssemblyLoader();
                    loader.RegisterPath(installationPath);
                    return loader;
                }

                exception = null;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            throw new RuntimeProxyException(String.Format("Could not obtain the Gallio installation path from the config file: '{0}'.",
                                                          configFilePath), exception);
        }

        private static bool DisplayInitializationError(Exception ex)
        {
            // TODO: Improve error reporting.
            return WindowUtil.ShowMessageBox(
                       String.Format("An exception occurred while initializing the Gallio "
                                     + "runtime components.  The Gallio test runner plugin will be "
                                     + "disabled for this session.\n\n{0}", ex),
                       "Gallio Runtime Error",
                       MessageBoxButtons.RetryCancel) == DialogResult.Retry;
        }

        private static bool IsRuntimeAssemblyAccessible()
        {
            try
            {
                Protected.ThrowAnExceptionIfTheRuntimeAssemblyIsNotAccessible();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// These methods are isolated protect the caller from type loading errors.
        /// </summary>
        private static class Protected
        {
            public static T Resolve<T>()
            {
                return Runtime.Instance.Resolve<T>();
            }

            public static void InitializeRuntime(string configFilePath)
            {
                RuntimeSetup setup = new RuntimeSetup();
                setup.ConfigurationFilePath = configFilePath;

                Runtime.Initialize(setup);
            }

            public static void ThrowAnExceptionIfTheRuntimeAssemblyIsNotAccessible()
            {
                // Check whether we can run this code without failure.
                GC.KeepAlive(Runtime.IsInitialized);
            }
        }
    }
}