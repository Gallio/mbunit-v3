using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Gallio.Common.Platform;
using Gallio.Common.Policies;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Common.Remoting;

namespace Gallio.Runtime.Security
{
    /// <summary>
    /// Default implementation of <see cref="IElevationManager" />.
    /// </summary>
    public class DefaultElevationManager : IElevationManager
    {
        private readonly IRuntime runtime;

        /// <summary>
        /// Creates an elevation manager.
        /// </summary>
        /// <param name="runtime">The runtime, not null</param>
        public DefaultElevationManager(IRuntime runtime)
        {
            this.runtime = runtime;
        }

        /// <inheritdoc />
        public bool HasElevatedPrivileges
        {
            get
            {
                return CurrentUserHasElevatedPrivileges();
            }
        }

        /// <inheritdoc />
        public bool TryAcquireElevationContext(string reason, out IElevationContext context)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            if (HasElevatedPrivileges)
            {
                context = new LocalElevationContext(runtime);
                return true;
            }

            IHost host = null;
            try
            {
                host = CreateHost();

                if (TryInitializeHostedElevationContext(host, out context))
                    return true;
            }
            catch (Exception ex)
            {
                runtime.Logger.Log(LogSeverity.Error, "Failed to create elevation context out of process.", ex);
            }

            if (host != null)
                host.Dispose();

            context = null;
            return false;
        }

        private IHost CreateHost()
        {
            var hostSetup = new HostSetup();
            hostSetup.Elevated = true;

            var hostFactory = new IsolatedProcessHostFactory(runtime);
            var host = hostFactory.CreateHost(hostSetup, runtime.Logger);
            return host;
        }

        private bool TryInitializeHostedElevationContext(IHost host, out IElevationContext elevationContext)
        {
            var hostedElevationContext = new HostedElevationContext(host);
            if (! hostedElevationContext.Initialize(runtime.GetRuntimeSetup(), runtime.Logger))
            {
                elevationContext = null;
                return false;
            }

            elevationContext = hostedElevationContext;
            return true;
        }

        private static bool CurrentUserHasElevatedPrivileges()
        {
            if (DotNetRuntimeSupport.IsUsingMono)
                return true; // FIXME

            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static object Execute(IRuntime runtime, string elevatedCommandId, object parameters, IProgressMonitor progressMonitor)
        {
            if (!CurrentUserHasElevatedPrivileges())
                throw new RuntimeException("Expected the process to have elevated privileges despite having established an elevation context.");

            IElevatedCommand command = (IElevatedCommand)runtime.ServiceLocator.ResolveByComponentId(elevatedCommandId);
            return command.Execute(parameters, progressMonitor);
        }

        private sealed class LocalElevationContext : IElevationContext
        {
            private readonly IRuntime runtime;

            public LocalElevationContext(IRuntime runtime)
            {
                this.runtime = runtime;
            }

            public void Dispose()
            {
            }

            public object Execute(string elevatedCommandId, object arguments, IProgressMonitor progressMonitor)
            {
                object result = DefaultElevationManager.Execute(runtime, elevatedCommandId, RoundTripSerializeForDebugging(arguments), progressMonitor);
                return RoundTripSerializeForDebugging(result);
            }

            private static object RoundTripSerializeForDebugging(object obj)
            {
#if true
                // For debugging, perform the serialization even when not required
                // so that we can ensure that arguments and results can be serialized
                // correctly even if the developer has UAC turned off.
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.FilterLevel = TypeFilterLevel.Full;
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                return formatter.Deserialize(stream);
#else
                return obj;
#endif
            }
        }

        private sealed class HostedElevationContext : IElevationContext
        {
            private readonly IHost host;
            private ElevationHook hook;

            public HostedElevationContext(IHost host)
            {
                this.host = host;
            }

            public void Dispose()
            {
                host.Dispose();
            }

            public bool Initialize(RuntimeSetup runtimeSetup, ILogger logger)
            {
                hook = HostUtils.CreateInstance<ElevationHook>(host);

                if (!hook.HasElevatedPrivileges())
                {
                    logger.Log(LogSeverity.Warning, "Failed to create elevation context out of process.  The process was created successfully but it did not acquire elevated privileges for an unknown reason.");
                    return false;
                }

                hook.Initialize(runtimeSetup, new RemoteLogger(logger));
                return true;
            }

            public object Execute(string elevatedCommandId, object arguments, IProgressMonitor progressMonitor)
            {
                return hook.Execute(elevatedCommandId, arguments, new RemoteProgressMonitor(progressMonitor));
            }
        }

        private sealed class ElevationHook : LongLivedMarshalByRefObject
        {
            public bool HasElevatedPrivileges()
            {
                return CurrentUserHasElevatedPrivileges();
            }

            public void Initialize(RuntimeSetup runtimeSetup, ILogger logger)
            {
                RuntimeBootstrap.Initialize(runtimeSetup, logger);
            }

            public object Execute(string elevatedCommandId, object arguments, IProgressMonitor progressMonitor)
            {
                return DefaultElevationManager.Execute(RuntimeAccessor.Instance, elevatedCommandId, arguments, progressMonitor);
            }
        }
    }
}
