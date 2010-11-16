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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Messaging;
using Gallio.Common.Reflection;
using Gallio.MbUnitCppAdapter.Model.Tasks;
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Model;
using Gallio.Model.Helpers;
using Gallio.Model.Isolation;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.MbUnitCppAdapter.Model
{
    /// <summary>
    /// Test drive for unmanaged MbUnitCpp tests.
    /// </summary>
    public class MbUnitCppTestDriver : BaseTestDriver
    {
        private readonly ILogger logger;

        /// <summary>
        /// Constructs the test driver.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        public MbUnitCppTestDriver(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        protected sealed override void ExploreImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            ExploreOrRun<ExploreTask>(testIsolationContext, testPackage, testExplorationOptions, null, messageSink, progressMonitor, "Exploring MbUnitCpp tests.");
        }

        /// <inheritdoc />
        protected sealed override void RunImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            ExploreOrRun<RunTask>(testIsolationContext, testPackage, testExplorationOptions, testExecutionOptions, messageSink, progressMonitor, "Running MbUnitCpp tests.");
        }

        private void ExploreOrRun<TTask>(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor, string taskName)
            where TTask : AbstractTask, new()
        {
            double totalWorkUnits = Math.Max(testPackage.Files.Count, 1);

            using (progressMonitor.BeginTask(taskName, totalWorkUnits))
            {
                var remoteMessageSink = new RemoteMessageSink(messageSink);
                var remoteLogger = new RemoteLogger(logger);

                if (progressMonitor.IsCanceled)
                    return;

                using (var remoteProgressMonitor = new RemoteProgressMonitor(progressMonitor))
                {
                    foreach (FileInfo file in testPackage.Files)
                    {
                        HostSetup hostSetup = CreateHostSetup(testPackage, file);
                        testIsolationContext.RunIsolatedTask<TTask>(hostSetup, progressMonitor.SetStatus, new object[] 
                        { 
                            testPackage, 
                            testExplorationOptions, 
                            testExecutionOptions, 
                            remoteMessageSink, 
                            remoteProgressMonitor, 
                            remoteLogger,
                            file,
                        });
                    }

                    remoteProgressMonitor.Worked(1);
                }
            }
        }

        private HostSetup CreateHostSetup(TestPackage testPackage, FileInfo file)
        {
            HostSetup hostSetup = testPackage.CreateHostSetup();
            hostSetup.ProcessorArchitecture = UnmanagedDllHelper.GetArchitecture(file.FullName);
            return hostSetup;
        }
    }
}
