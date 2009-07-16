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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Model.Isolation;
using Gallio.Runner;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Loader;
using Gallio.Common.Messaging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Model
{
    /// <summary>
    /// A base test driver that is intended to support exploring and executing tests
    /// inside .Net assemblies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This test driver is a simple component for building a test driver for .Net based
    /// test frameworks that do not require specialized hosting apparatus.  It explores and
    /// runs each test assembly within a separate isolated task that is configured based on the assembly
    /// config file, runtime version and processor requirements.  Depending on the isolation context,
    /// the test may run in a different AppDomain or a different Process with a code coverage
    /// tool, profiler or debugger attached.  Regardless, subclasses do not need to concern themselves
    /// with these details.
    /// </para>
    /// <para>
    /// Both the <see cref="ExploreAssembly" /> and <see cref="RunAssembly" /> methods execute on
    /// a remote created instance of the test driver within the test host.
    /// If the test driver does not have a default constructor then it must implement the
    /// <see cref="GetRemoteTestDriverArguments" /> method to provide the required constructor
    /// arguments for the remote instance.
    /// </para>
    /// </remarks>
    public abstract class DotNetTestDriver : BaseTestDriver
    {
        /// <summary>
        /// Explores tests in an assembly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override to enable tests
        /// to be explored.  This is required for presenting lists of tests in test runners like Icarus.
        /// </para>
        /// <para>
        /// This method executes within the test host which is most likely a different AppDomain
        /// or Process from the test runner itself.
        /// </para>
        /// </remarks>
        /// <param name="assembly">The test assembly, not null.</param>
        /// <param name="testExplorationOptions">The test exploration options, not null.</param>
        /// <param name="messageSink">The message sink, not null.</param>
        /// <param name="progressMonitor">The progress monitor, not null.</param>
        protected virtual void ExploreAssembly(Assembly assembly, TestExplorationOptions testExplorationOptions,
            IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
        }

        /// <summary>
        /// Runs tests in an assembly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override to enable tests
        /// to be run.  This is required for actually running tests.
        /// </para>
        /// <para>
        /// This method executes within the test host which is most likely a different AppDomain
        /// or Process from the test runner itself.
        /// </para>
        /// </remarks>
        /// <param name="assembly">The test assembly, not null.</param>
        /// <param name="testExplorationOptions">The test exploration options, not null.</param>
        /// <param name="testExecutionOptions">The test execution options, not null.</param>
        /// <param name="messageSink">The message sink, not null.</param>
        /// <param name="progressMonitor">The progress monitor, not null.</param>
        protected virtual void RunAssembly(Assembly assembly, TestExplorationOptions testExplorationOptions,
            TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
        }

        /// <summary>
        /// Configures the host for a particular assembly.
        /// </summary>
        /// <remarks>
        /// The default implementation does nothing.  Subclasses may override to customize
        /// host the test host is configured for a particular assembly.
        /// </remarks>
        /// <param name="hostSetup">The host setup, not null.</param>
        /// <param name="assemblyPath">The test assembly path, not null.</param>
        protected virtual void ConfigureHostSetupForAssembly(HostSetup hostSetup, string assemblyPath)
        {
        }

        /// <summary>
        /// Gets arguments used to construct the a remote instance of the test driver.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the test driver does not have a default constructor then it must implement the
        /// <see cref="GetRemoteTestDriverArguments" /> method to provide the required constructor arguments.
        /// </para>
        /// <para>
        /// All arguments must be serializable.
        /// </para>
        /// <para>
        /// The default implementation returns an empty array.
        /// </para>
        /// </remarks>
        /// <returns></returns>
        protected virtual object[] GetRemoteTestDriverArguments()
        {
            return EmptyArray<object>.Instance;
        }

        /// <inheritdoc />
        sealed protected override void ExploreImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions,
            IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            ExploreOrRun(testIsolationContext, testPackage, testExplorationOptions, null, messageSink, progressMonitor,
                "Exploring tests.");
        }

        /// <inheritdoc />
        sealed protected override void RunImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions,
            TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            ExploreOrRun(testIsolationContext, testPackage, testExplorationOptions, testExecutionOptions, messageSink,
                progressMonitor, "Running tests.");
        }

        private void ExploreOrRun(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions,
            TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor, string taskName)
        {
            using (progressMonitor.BeginTask(taskName, Math.Max(testPackage.Files.Count, 1)))
            {
                foreach (FileInfo file in testPackage.Files)
                {
                    if (progressMonitor.IsCanceled)
                        return;

                    RemoteMessageSink remoteMessageSink = new RemoteMessageSink(messageSink);
                    ExploreOrRunAssembly(testIsolationContext, testPackage, testExplorationOptions, testExecutionOptions,
                        remoteMessageSink, progressMonitor.CreateSubProgressMonitor(1), taskName, file);
                }
            }
        }

        private void ExploreOrRunAssembly(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions,
            TestExecutionOptions testExecutionOptions, RemoteMessageSink remoteMessageSink, IProgressMonitor progressMonitor, string taskName,
            FileInfo file)
        {
            using (progressMonitor.BeginTask(taskName, 100))
            {
                if (progressMonitor.IsCanceled)
                    return;

                string assemblyPath = file.FullName;
                progressMonitor.SetStatus("Getting test assembly metadata.");
                AssemblyMetadata assemblyMetadata = AssemblyUtils.GetAssemblyMetadata(assemblyPath, AssemblyMetadataFields.Default);
                progressMonitor.Worked(2);

                if (progressMonitor.IsCanceled)
                    return;

                if (assemblyMetadata != null)
                {
                    Type driverType = GetType();
                    object[] driverArguments = GetRemoteTestDriverArguments();

                    HostSetup hostSetup = CreateHostSetup(testPackage, assemblyPath, assemblyMetadata);

                    using (var remoteProgressMonitor = new RemoteProgressMonitor(
                        progressMonitor.CreateSubProgressMonitor(97)))
                    {
                        testIsolationContext.RunIsolatedTask<ExploreOrRunTask>(hostSetup,
                            (statusMessage) => progressMonitor.SetStatus(statusMessage),
                            new object[] { driverType, driverArguments, assemblyPath, testExplorationOptions, testExecutionOptions, remoteMessageSink, remoteProgressMonitor });
                    }

                    // Record one final work unit after the isolated task has been fully cleaned up.
                    progressMonitor.SetStatus("");
                    progressMonitor.Worked(1);
                }
            }
        }

        /// <summary>
        /// Creates a host setup for a particular assembly within a test package.
        /// </summary>
        /// <param name="testPackage">The test package, not null.</param>
        /// <param name="assemblyPath">The assembly path, not null.</param>
        /// <param name="assemblyMetadata">The assembly metadata, not null.</param>
        /// <returns>The host setup setup.</returns>
        protected HostSetup CreateHostSetup(TestPackage testPackage, string assemblyPath, AssemblyMetadata assemblyMetadata)
        {
            HostSetup hostSetup = testPackage.CreateHostSetup();
            ConfigureHostSetupForAssembly(hostSetup, assemblyPath, assemblyMetadata);
            ConfigureHostSetup(hostSetup, testPackage, assemblyPath, assemblyMetadata);
            return hostSetup;
        }

        /// <summary>
        /// Configures the host setup prior to the initialization of the script runtime.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override this method
        /// to customize the host setup.
        /// </para>
        /// </remarks>
        /// <param name="hostSetup">The host setup, not null.</param>
        /// <param name="testPackage">The test package, not null.</param>
        /// <param name="assemblyPath">The assembly path, not null.</param>
        /// <param name="assemblyMetadata">The assembly metadata, not null.</param>
        protected virtual void ConfigureHostSetup(HostSetup hostSetup, TestPackage testPackage, string assemblyPath, AssemblyMetadata assemblyMetadata)
        {
        }

        private void ConfigureHostSetupForAssembly(HostSetup hostSetup, string assemblyPath, AssemblyMetadata assemblyMetadata)
        {
            string assemblyDir = Path.GetDirectoryName(assemblyPath);

            if (hostSetup.ApplicationBaseDirectory == null)
                hostSetup.ApplicationBaseDirectory = assemblyDir;

            if (hostSetup.WorkingDirectory == null)
                hostSetup.WorkingDirectory = assemblyDir;

            hostSetup.ConfigurationFileLocation = ConfigurationFileLocation.AppBase;

            string assemblyConfigFilePath = assemblyPath + @".config";
            if (File.Exists(assemblyConfigFilePath))
            {
                string configurationXml = File.ReadAllText(assemblyConfigFilePath);
                hostSetup.Configuration.ConfigurationXml = configurationXml;

                if (hostSetup.RuntimeVersion == null)
                    hostSetup.RuntimeVersion = GetPreferredRuntimeVersion(configurationXml);
            }

            foreach (AssemblyBinding assemblyBinding in RuntimeAccessor.Instance.GetAllPluginAssemblyBindings())
            {
                if (assemblyBinding.CodeBase != null)
                    hostSetup.Configuration.AddAssemblyBinding(assemblyBinding);
            }

            if (hostSetup.ProcessorArchitecture == ProcessorArchitecture.None
                || hostSetup.ProcessorArchitecture == ProcessorArchitecture.MSIL)
                hostSetup.ProcessorArchitecture = assemblyMetadata.ProcessorArchitecture;

            ConfigureHostSetupForAssembly(hostSetup, assemblyPath);
        }

        private static string GetPreferredRuntimeVersion(string configurationXml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(configurationXml);

            foreach (XmlElement supportedRuntimeNode in doc.SelectNodes("/configuration/startup/supportedRuntime"))
            {
                string runtimeVersion = supportedRuntimeNode.GetAttribute("version");
                if (runtimeVersion != null)
                    return runtimeVersion;
            }

            return null;
        }

        private class ExploreOrRunTask : IsolatedTask
        {
            protected override object RunImpl(object[] args)
            {
                ExploreOrRun(
                    (Type)args[0],
                    (object[])args[1],
                    (string)args[2],
                    (TestExplorationOptions)args[3],
                    (TestExecutionOptions)args[4],
                    (IMessageSink)args[5],
                    (IProgressMonitor)args[6]);
                return null;
            }

            private void ExploreOrRun(Type driverType, object[] driverArguments,
                string assemblyPath, TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions,
                IMessageSink messageSink, IProgressMonitor progressMonitor)
            {
                Assembly assembly = LoadAssembly(assemblyPath);

                using (var queuedMessageSink = new QueuedMessageSink(messageSink))
                {
                    var testDriver = (DotNetTestDriver) Activator.CreateInstance(driverType, driverArguments);

                    if (testExecutionOptions == null)
                        testDriver.ExploreAssembly(assembly, testExplorationOptions, queuedMessageSink, progressMonitor);
                    else
                        testDriver.RunAssembly(assembly, testExplorationOptions, testExecutionOptions, queuedMessageSink, progressMonitor);
                }
            }

            private static Assembly LoadAssembly(string assemblyPath)
            {
                try
                {
                    IAssemblyLoader assemblyLoader = RuntimeAccessor.AssemblyLoader;
                    return assemblyLoader.LoadAssemblyFrom(assemblyPath);
                }
                catch (Exception ex)
                {
                    throw new RunnerException(String.Format(CultureInfo.CurrentCulture,
                        "Could not load test assembly from '{0}'.", assemblyPath), ex);
                }
            }
        }
    }
}
