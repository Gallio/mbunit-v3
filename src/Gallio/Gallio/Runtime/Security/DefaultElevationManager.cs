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
        /// <param name="runtime">The runtime, not null.</param>
        public DefaultElevationManager(IRuntime runtime)
        {
            this.runtime = runtime;
        }

        /// <inheritdoc />
        public bool HasElevatedPrivileges
        {
            get { return ProcessSupport.HasElevatedPrivileges; }
        }

        /// <inheritdoc />
        public bool TryElevate(ElevationAction elevationAction, string reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");
            if (elevationAction == null)
                throw new ArgumentNullException("elevationAction");

            if (HasElevatedPrivileges)
            {
                var elevationContext = new LocalElevationContext(runtime);
                return elevationAction(elevationContext);
            }

            try
            {
                using (IHost host = CreateHost())
                {
                    var elevationContext = new HostedElevationContext(host);
                    if (!elevationContext.Initialize(runtime.GetRuntimeSetup(), runtime.Logger))
                        return false;

                    return elevationAction(elevationContext);
                }
            }
            catch (Exception ex)
            {
                runtime.Logger.Log(LogSeverity.Error, "Failed to create an elevation context out of process.", ex);
                return false;
            }
        }

        private IHost CreateHost()
        {
            var hostSetup = new HostSetup();
            hostSetup.Elevated = true;

            var hostFactory = new IsolatedProcessHostFactory(runtime);
            var host = hostFactory.CreateHost(hostSetup, runtime.Logger);
            return host;
        }

        private static object Execute(IRuntime runtime, string elevatedCommandId, object parameters, IProgressMonitor progressMonitor)
        {
            if (!ProcessSupport.HasElevatedPrivileges)
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

            public object Execute(string elevatedCommandId, object arguments, IProgressMonitor progressMonitor)
            {
                object result = DefaultElevationManager.Execute(runtime, elevatedCommandId, RoundTripSerializeForDebugging(arguments), progressMonitor);
                return RoundTripSerializeForDebugging(result);
            }

            private static object RoundTripSerializeForDebugging(object obj)
            {
#if true
                if (obj != null)
                {
                    // For debugging, perform the serialization even when not required
                    // so that we can ensure that arguments and results can be serialized
                    // correctly even if the developer has UAC turned off.
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.FilterLevel = TypeFilterLevel.Full;
                    MemoryStream stream = new MemoryStream();
                    formatter.Serialize(stream, obj);
                    stream.Position = 0;
                    return formatter.Deserialize(stream);
                }
#endif
                return obj;
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

            public bool Initialize(RuntimeSetup runtimeSetup, ILogger logger)
            {
                hook = HostUtils.CreateInstance<ElevationHook>(host);

                if (!hook.HasElevatedPrivileges())
                {
                    logger.Log(LogSeverity.Warning, "Failed to create an elevation context out of process.  The process was created successfully but it did not acquire elevated privileges for an unknown reason.");
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
                return ProcessSupport.HasElevatedPrivileges;
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
