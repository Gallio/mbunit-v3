// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using Gallio.Hosting;
using Gallio.Utilities;
using Castle.Core.Logging;

namespace Gallio.Hosting
{
    /// <summary>
    /// <para>
    /// Provides functions for obtaining runtime services such as XML documentation
    /// resolution.
    /// </para>
    /// </summary>
    public static class Runtime
    {
        private static IRuntime instance;

        /// <summary>
        /// Gets the runtime instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the runtime has not been initialized</exception>
        public static IRuntime Instance
        {
            get
            {
                if (instance == null)
                    throw new InvalidOperationException("The runtime has not been initialized.");

                return instance;
            }
        }

        /// <summary>
        /// Gets the runtime's logger, or a <see cref="NullLogger" /> if the runtime is not initialized.
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                IRuntime cachedInstance = instance;
                return cachedInstance != null ? cachedInstance.Resolve<ILogger>() : NullLogger.Instance;
            }
        }

        /// <summary>
        /// Returns true if the runtime has been initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get { return instance != null; }
        }

        /// <summary>
        /// The event dispatched when the value of the current runtime
        /// <see cref="Instance" /> changes.
        /// </summary>
        public static event EventHandler InstanceChanged;

        /// <summary>
        /// Returns true if the application is running within the Mono runtime.
        /// </summary>
        /// <remarks>
        /// It is occasionally necessary to tailor the execution of the test runner
        /// depending on whether Mono is running.  However, the number of such
        /// customizations should be very limited.
        /// </remarks>
        public static bool IsUsingMono
        {
            get { return Type.GetType(@"Mono.Runtime") != null; }
        }

        /// <summary>
        /// Gets the local path of the Gallio installation.
        /// </summary>
        /// <returns>The installation path</returns>
        public static string InstallationPath
        {
            get
            {
                return Runtime.Instance.GetRuntimeSetup().InstallationPath;
            }
        }

        /// <summary>
        /// <para>
        /// Initializes the runtime.
        /// </para>
        /// <para>
        /// Loads plugins and initalizes the runtime component model.  The
        /// specifics of the system can be configured by editing the appropriate
        /// *.plugin files to register new components and facilities as required.
        /// </para>
        /// </summary>
        /// <param name="setup">The runtime setup parameters</param>
        /// <param name="logger">The runtime logging service</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="setup"/> or <paramref name="logger"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the runtime has already been initialized</exception>
        public static void Initialize(RuntimeSetup setup, ILogger logger)
        {
            if (setup == null)
                throw new ArgumentNullException("setup");
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (IsInitialized)
                throw new InvalidOperationException("The runtime has already been started.");

            IRuntimeFactory runtimeFactory = GetRuntimeFactory(setup);
            IRuntime runtime = runtimeFactory.CreateRuntime(setup);
            Debug.Assert(runtime != null, "The runtime returned by the runtime factory must not be null.");

            try
            {
                SetRuntime(runtime);
                runtime.Initialize(logger);

                if (! UnhandledExceptionPolicy.HasReportUnhandledExceptionHandler)
                    UnhandledExceptionPolicy.ReportUnhandledException += HandleUnhandledExceptionNotification;
            }
            catch (Exception)
            {
                SetRuntime(null);
                throw;
            }
        }

        /// <summary>
        /// Shuts down the runtime if it is currently initialized.
        /// Does nothing if the runtime has not been initialized.
        /// </summary>
        public static void Shutdown()
        {
            if (! IsInitialized)
                return;

            try
            {
                instance.Dispose();
            }
            finally
            {
                UnhandledExceptionPolicy.ReportUnhandledException -= HandleUnhandledExceptionNotification;

                SetRuntime(null);
            }
        }

        private static IRuntimeFactory GetRuntimeFactory(RuntimeSetup setup)
        {
            if (setup.RuntimeFactoryType == null)
                return new WindsorRuntimeFactory();

            Type type = Type.GetType(setup.RuntimeFactoryType);
            if (type == null)
                throw new InvalidOperationException(String.Format("Cannot resolve '{0}' to a runtime factory type because the type was not found.",
                    setup.RuntimeFactoryType));

            if (! typeof(IRuntimeFactory).IsAssignableFrom(type))
                throw new InvalidOperationException(String.Format("Cannot resolve '{0}' to a runtime factory type because the type does not implement {1}.",
                    setup.RuntimeFactoryType, typeof(IRuntimeFactory).Name));

            return (IRuntimeFactory) Activator.CreateInstance(type);
        }

        private static void SetRuntime(IRuntime runtime)
        {
            EventHandler instanceChangedHandlers = InstanceChanged;
            instance = runtime;

            EventHandlerUtils.SafeInvoke(instanceChangedHandlers, null, EventArgs.Empty);
        }

        private static void HandleUnhandledExceptionNotification(object sender, CorrelatedExceptionEventArgs e)
        {
            if (e.IsRecursive)
                return;

            Logger.Fatal("Internal error: " + e.GetDescription());
        }
    }
}