// Copyright 2010 Nicolas Graziano 
// largely inspired by NCoverIntegration in Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Model.Isolation;
using Gallio.PartCoverIntegration.Tools;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;

namespace Gallio.PartCoverIntegration
{
    /// <summary>
    /// An PartCover test isolation context.
    /// </summary>
    public class PartCoverTestIsolationContext : HostedTestIsolationContext
    {
        private Batch batch;

        /// <summary>
        /// Creates an PartCover test isolation context.
        /// </summary>
        /// <param name="testIsolationOptions">The test isolation options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="runtime">The runtime.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testIsolationOptions" />,
        /// <paramref name="logger"/> or <paramref name="runtime"/> is null.</exception>
        public PartCoverTestIsolationContext(TestIsolationOptions testIsolationOptions, ILogger logger,
            IRuntime runtime)
            : base(new PartCoverHostFactory(runtime), testIsolationOptions, logger)
        {
        }

        /// <inheritdoc />
        protected override object RunIsolatedTaskInHost<TIsolatedTask>(HostSetup hostSetup, StatusReporter statusReporter, object[] args)
        {
            if (!PartCoverTool.IsInstalled())
                throw new TestIsolationException(string.Format("{0} does not appear to be installed.", PartCoverTool.Name));

            string partCoverCoverageReportDir = PartCoverHost.GetPartCoverCoverageReportDir(hostSetup);
         
            if (batch != null)
            {
                hostSetup = hostSetup.Copy();

                string tempCoverageFile = batch.Enlist(partCoverCoverageReportDir);
                PartCoverHost.SetPartCoverCoverageFile(hostSetup, tempCoverageFile);
            }
            return base.RunIsolatedTaskInHost<TIsolatedTask>(hostSetup, statusReporter, args);
        }

        /// <inheritdoc />
        protected override IDisposable BeginBatchImpl(StatusReporter statusReporter)
        {
            batch = new Batch(statusReporter, Logger);
            return batch;
        }

        /// <summary>
        /// A Batch class containing all reference to coverage file 
        /// </summary>
        private sealed class Batch : IDisposable
        {
            private readonly StatusReporter statusReporter;
            private readonly MultiMap<string, string> coverageFiles;
            private readonly ILogger logger;

            /// <summary>
            /// Initializes a new instance of the <see cref="Batch"/> class.
            /// </summary>
            /// <param name="statusReporter">The status reporter.</param>
            /// <param name="logger">The logger.</param>
            public Batch(StatusReporter statusReporter, ILogger logger)
            {
                this.statusReporter = statusReporter;
                this.logger = logger;

                coverageFiles = new MultiMap<string, string>();
                
            }

            /// <summary>
            /// Exécute les tâches définies par l'application associées à la libération ou à la redéfinition des ressources non managées.
            /// </summary>
            public void Dispose()
            {
                statusReporter("Merging PartCover coverage files.");

                foreach (var pair in coverageFiles)
                {
                    Merge(pair.Value, pair.Key);
                }
                
                statusReporter("");
            }

            /// <summary>
            /// Enlists the specified coverage file.
            /// </summary>
            /// <param name="coverageFile">The coverage directory.</param>
            /// <returns>Temporary name file.</returns>
            public string Enlist(string coverageFile)
            {
                string temporaryFile = SpecialPathPolicy.For("PartCover").CreateTempFileWithUniqueName().FullName;
                coverageFiles.Add(coverageFile, temporaryFile);
                return temporaryFile;
            }

            /// <summary>
            /// Merges the specified sources.
            /// </summary>
            /// <param name="sources">The sources.</param>
            /// <param name="destination">The destination.</param>
            private void Merge(IList<string> sources, string destination)
            {
                PartCoverTool.CreateReport(sources, destination, logger);
            }
        }
    }
}
