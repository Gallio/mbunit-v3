// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.AutoCAD.ProcessManagement;
using Gallio.Common;
using Gallio.Model.Isolation;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Loader;
using Gallio.Runtime.Logging;

namespace Gallio.AutoCAD.Isolation
{
    /// <summary>
    /// A test isolation context that launches AutoCAD and runs tasks remotely.
    /// </summary>
    public class AcadTestIsolationContext : BaseTestIsolationContext
    {
        private static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(5);

        private readonly ILogger logger;
        private readonly IAcadProcess process;

        private Batch batch;

        /// <summary>
        /// Creates a test isolation context.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="process">The AutoCAD process.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="logger"/> or <paramref name="process"/> is null.
        /// </exception>
        public AcadTestIsolationContext(ILogger logger, IAcadProcess process)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (process == null)
                throw new ArgumentNullException("process");

            this.logger = logger;
            this.process = process;
        }

        /// <inheritdoc />
        public override bool RequiresSingleThreadedExecution
        {
            get { return true; }
        }

        /// <inheritdoc />
        protected override IDisposable BeginBatchImpl(StatusReporter statusReporter)
        {
            batch = new Batch(statusReporter, process);
            return batch;
        }

        /// <inheritdoc />
        protected override object RunIsolatedTaskImpl<TIsolatedTask>(HostSetup hostSetup, StatusReporter statusReporter, object[] args)
        {
            if (batch == null)
            {
                using (BeginBatch(statusReporter))
                    RunIsolatedTaskImpl<TIsolatedTask>(hostSetup, statusReporter, args);
            }

            TestIsolationServer server = batch.GetTestIsolationServer(hostSetup.DebuggerSetup);
            return server.RunIsolatedTaskOnClient(typeof(TIsolatedTask), args);
        }

        /// <summary>
        /// Gets the AutoCAD process used by the isolation context.
        /// </summary>
        public IAcadProcess Process
        {
            get { return process; }
        }

        private sealed class Batch : IDisposable
        {
            private readonly StatusReporter statusReporter;
            private readonly IAcadProcess process;

            private TestIsolationServer server;

            public Batch(StatusReporter statusReporter, IAcadProcess process)
            {
                this.statusReporter = statusReporter;
                this.process = process;
            }

            public TestIsolationServer GetTestIsolationServer(DebuggerSetup debuggerSetup)
            {
                if (server != null)
                    return server;

                try
                {
                    statusReporter("Attaching to AutoCAD.");

                    string gallioLoaderAssemblyPath = GallioLoaderLocator.GetGallioLoaderAssemblyPath();
                    string ipcPortName = "AcadTestIsolationContext." + Hash64.CreateUniqueHash();
                    Guid uniqueId = Guid.NewGuid();
                    server = new TestIsolationServer(ipcPortName, uniqueId);
                    process.Start(ipcPortName, uniqueId, gallioLoaderAssemblyPath, debuggerSetup);
                    return server;
                }
                catch
                {
                    if (process != null)
                        process.Dispose();
                    if (server != null)
                        server.Dispose();
                    throw;
                }
            }

            public void Dispose()
            {
                statusReporter("Detaching from AutoCAD.");

                if (server != null)
                {
                    server.Shutdown(ShutdownTimeout);
                    server = null;
                }

                if (process != null)
                {
                    process.Dispose();
                }
            }
        }
    }
}