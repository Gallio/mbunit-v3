using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Gallio.Common.Collections;
using Gallio.Common.Concurrency;
using Gallio.Common.IO;
using Gallio.Common.Messaging;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Model.Isolation;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Microsoft.Scripting.Hosting;

namespace Gallio.DLRIntegration.Model
{
    /// <summary>
    /// A base class for implementing test drivers using the DLR.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides support for implementing test frameworks and adapters
    /// in DLR-supported dynamic languages.  This code takes care of entering a
    /// test isolation context, initializing the DLR script runtime and then invoking
    /// the provided test driver script.
    /// </para>
    /// <para>
    /// A test driver script is a script written in a DLR language.  The test driver
    /// communicates with the script by setting a global variable called "ScriptParameters"
    /// in the DLR interpreter environment to a dictionary that contains a verb
    /// specifying the service to perform along with other parameters.
    /// </para>
    /// <para>
    /// The "ScriptParameters" dictionary contains the following key/value pairs:
    /// <list type="bullet">
    /// <item>Verb: The command to perform.  Either "Explore" or "Run".</item>
    /// <item>TestPackage: The <see cref="TestPackage" /> object specifying test package options including the list of files to run.</item>
    /// <item>TestExplorationOptions: The <see cref="TestExplorationOptions" /> object specifying test exploration options.</item>
    /// <item>TestExecutionOptions: The <see cref="TestExecutionOptions" /> object specifying test execution options.  Only provided when the verb is "Run".</item>
    /// <item>MessageSink: The <see cref="IMessageSink" /> object providing a return-path to send test exploration and execution messages to the test runner.</item>
    /// <item>ProgressMonitor: The <see cref="IProgressMonitor" /> object providing progress reporting capabilities.</item>
    /// <item>Logger: The <see cref="ILogger" /> object providing logging capabilities.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public abstract class DLRTestDriver : BaseTestDriver
    {
        /// <summary>
        /// Gets the name of the variable used to pass script parameters.
        /// </summary>
        /// <value>"ScriptParameters"</value>
        public static readonly string ScriptParametersVariableName = "ScriptParameters";

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a DLR test driver.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        protected DLRTestDriver(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.logger = logger;
        }

        /// <summary>
        /// Gets the test driver's logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
        }

        /// <inheritdoc />
        sealed protected override void ExploreImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            ExploreOrRun(testIsolationContext, testPackage, testExplorationOptions, null, messageSink, progressMonitor,
                "Exploring tests.");
        }

        /// <inheritdoc />
        sealed protected override void RunImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            ExploreOrRun(testIsolationContext, testPackage, testExplorationOptions, testExecutionOptions, messageSink,
                progressMonitor, "Running tests.");
        }

        private void ExploreOrRun(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions,
            TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor, string taskName)
        {
            using (progressMonitor.BeginTask(taskName, 1))
            {
                if (progressMonitor.IsCanceled)
                    return;

                FileInfo testDriverScriptFile = GetTestDriverScriptFile(testPackage);
                if (testDriverScriptFile == null)
                    return;

                HostSetup hostSetup = CreateHostSetup(testPackage);
                ScriptRuntimeSetup scriptRuntimeSetup = CreateScriptRuntimeSetup(testPackage);
                string testDriverScriptPath = testDriverScriptFile.FullName;
                var remoteMessageSink = new RemoteMessageSink(messageSink);
                var remoteLogger = new RemoteLogger(logger);

                using (var remoteProgressMonitor = new RemoteProgressMonitor(progressMonitor.CreateSubProgressMonitor(1)))
                {
                    testIsolationContext.RunIsolatedTask<ExploreOrRunTask>(hostSetup,
                        (statusMessage) => progressMonitor.SetStatus(statusMessage),
                        new object[] { testPackage, scriptRuntimeSetup, testDriverScriptPath, testExplorationOptions, testExecutionOptions,
                            remoteMessageSink, remoteProgressMonitor, remoteLogger });
                }
            }
        }

        /// <summary>
        /// Gets the test driver script file to run for a test package.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses must override this method to return a test driver script file for
        /// test exploration or execution.
        /// </para>
        /// </remarks>
        /// <param name="testPackage">The test package, not null.</param>
        /// <returns>The test driver script file to run, or null to script the test run.</returns>
        protected abstract FileInfo GetTestDriverScriptFile(TestPackage testPackage);

        /// <summary>
        /// Creates a host setup for a test package.
        /// </summary>
        /// <param name="testPackage">The test package, not null.</param>
        /// <returns>The host setup setup.</returns>
        protected HostSetup CreateHostSetup(TestPackage testPackage)
        {
            HostSetup hostSetup = testPackage.CreateHostSetup();
            ConfigureHostSetup(hostSetup, testPackage);
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
        protected virtual void ConfigureHostSetup(HostSetup hostSetup, TestPackage testPackage)
        {
        }

        /// <summary>
        /// Creates a script runtime setup for a test package.
        /// </summary>
        /// <param name="testPackage">The test package, not null.</param>
        /// <returns>The script runtime setup.</returns>
        protected ScriptRuntimeSetup CreateScriptRuntimeSetup(TestPackage testPackage)
        {
            ScriptRuntimeSetup scriptRuntimeSetup = ReadScriptRuntimeSetupFromConfiguration();
            ConfigureIronRuby(scriptRuntimeSetup);
            ConfigureScriptRuntimeSetup(scriptRuntimeSetup, testPackage);
            return scriptRuntimeSetup;
        }

        private static ScriptRuntimeSetup ReadScriptRuntimeSetupFromConfiguration()
        {
            string assemblyFile = AssemblyUtils.GetFriendlyAssemblyLocation(typeof(DLRTestDriver).Assembly);
            string configFile = assemblyFile + ".config";

            return ScriptRuntimeSetup.ReadConfiguration(configFile);
        }

        /// <summary>
        /// Configures the script runtime setup prior to the initialization of the script runtime.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override this method
        /// to customize the script runtime setup.
        /// </para>
        /// </remarks>
        /// <param name="scriptRuntimeSetup">The script runtime setup, not null.</param>
        /// <param name="testPackage">The test package, not null.</param>
        protected virtual void ConfigureScriptRuntimeSetup(ScriptRuntimeSetup scriptRuntimeSetup, TestPackage testPackage)
        {
        }

        private void ConfigureIronRuby(ScriptRuntimeSetup scriptRuntimeSetup)
        {
            LanguageSetup languageSetup = GenericCollectionUtils.Find(scriptRuntimeSetup.LanguageSetups, x => x.Names.Contains("IronRuby"));
            if (languageSetup == null)
                return;

            IList<string> libraryPaths = GetIronRubyLibraryPaths(languageSetup);
            ConfigureIronRuby(languageSetup, libraryPaths);
            CanonicalizeLibraryPaths(libraryPaths);
            SetIronRubyLibraryPaths(languageSetup, libraryPaths);
        }

        /// <summary>
        /// Configures the IronRuby language options.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation does nothing.  Subclasses may override to
        /// configure the IronRuby language options.
        /// </para>
        /// </remarks>
        /// <param name="languageSetup">The IronRuby language setup.</param>
        /// <param name="libraryPaths">The list of IronRuby library paths.</param>
        protected virtual void ConfigureIronRuby(LanguageSetup languageSetup, IList<string> libraryPaths)
        {
        }

        private static IList<string> GetIronRubyLibraryPaths(LanguageSetup languageSetup)
        {
            List<string> libraryPaths = new List<string>();

            object value;
            if (languageSetup.Options.TryGetValue("LibraryPaths", out value))
            {
                libraryPaths.AddRange(((string)value).Split(';'));
            }

            return libraryPaths;
        }

        private static void SetIronRubyLibraryPaths(LanguageSetup languageSetup, IList<string> libraryPaths)
        {
            languageSetup.Options["LibraryPaths"] = string.Join(";", GenericCollectionUtils.ToArray(libraryPaths));
        }

        private static void CanonicalizeLibraryPaths(IList<string> libraryPaths)
        {
            string pluginBaseDirectory = RuntimeAccessor.Registry.Plugins["Gallio.DLRIntegration"].BaseDirectory.FullName;
            FileUtils.CanonicalizePaths(pluginBaseDirectory, libraryPaths);
        }

        private sealed class ExploreOrRunTask : IsolatedTask
        {
            protected override object RunImpl(object[] args)
            {
                ExploreOrRun(
                    (TestPackage)args[0],
                    (ScriptRuntimeSetup)args[1],
                    (string)args[2],
                    (TestExplorationOptions)args[3],
                    (TestExecutionOptions)args[4],
                    (IMessageSink)args[5],
                    (IProgressMonitor)args[6],
                    (ILogger)args[7]);
                return null;
            }

            private static void ExploreOrRun(TestPackage testPackage, ScriptRuntimeSetup scriptRuntimeSetup, string testDriverScriptPath,
                TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions,
                IMessageSink messageSink, IProgressMonitor progressMonitor, ILogger logger)
            {
                using (BufferedLogWriter outputWriter = new BufferedLogWriter(logger, LogSeverity.Info, Encoding.Default),
                    errorWriter = new BufferedLogWriter(logger, LogSeverity.Error, Encoding.Default))
                {
                    using (var queuedMessageSink = new QueuedMessageSink(messageSink))
                    {
                        using (new ConsoleRedirection(outputWriter, errorWriter))
                        {
                            var scriptRuntime = new ScriptRuntime(scriptRuntimeSetup);

                            scriptRuntime.IO.SetInput(Stream.Null, TextReader.Null, Encoding.Default);
                            scriptRuntime.IO.SetOutput(new TextWriterStream(outputWriter), outputWriter);
                            scriptRuntime.IO.SetErrorOutput(new TextWriterStream(errorWriter), errorWriter);

                            try
                            {
                                var scriptParameters = new Dictionary<string, object>();
                                scriptParameters.Add("Verb", testExecutionOptions != null ? "Run" : "Explore");
                                scriptParameters.Add("TestPackage", testPackage);
                                scriptParameters.Add("TestExplorationOptions", testExplorationOptions);
                                scriptParameters.Add("TestExecutionOptions", testExecutionOptions);
                                scriptParameters.Add("MessageSink", queuedMessageSink);
                                scriptParameters.Add("ProgressMonitor", progressMonitor);
                                scriptParameters.Add("Logger", logger);

                                scriptRuntime.Globals.SetVariable(ScriptParametersVariableName, scriptParameters);

                                RunScript(scriptRuntime, testDriverScriptPath);
                            }
                            finally
                            {
                                scriptRuntime.Shutdown();
                            }
                        }
                    }
                }
            }

            private static void RunScript(ScriptRuntime scriptRuntime, string scriptPath)
            {
                string exceptionMessage = null;
                var thread = new Thread(() =>
                {
                    ScriptEngine engine = scriptRuntime.GetEngineByFileExtension(Path.GetExtension(scriptPath));
                    try
                    {
                        engine.ExecuteFile(scriptPath);
                    }
                    catch (Exception ex)
                    {
                        exceptionMessage = engine.GetService<ExceptionOperations>().FormatException(ex);
                    }
                });

                thread.Name = "DLR Test Driver";
                thread.SetApartmentState(ApartmentState.STA);

                // Suppressing flow of the execution context trims a couple of frames off the stack.
                // It's kind of silly to do except that IronRuby reports the caller's full stack
                // trace for each exception all the way up to the top of the Thread and it's no
                // fun to see stack traces for test failures cluttered with mscorlib stuff that is
                // not necessary in this scenario. This is also part of why we create a new thread
                // for the test run.  (But we also create a thread to force the use of STA mode.)
                // -- Jeff.
                using (ExecutionContext.IsFlowSuppressed() ? (IDisposable) null : ExecutionContext.SuppressFlow())
                    thread.Start();

                thread.Join();

                if (exceptionMessage != null)
                    throw new ModelException(exceptionMessage);
            }
        }

        private sealed class TextWriterStream : Stream
        {
            private readonly TextWriter writer;

            public TextWriterStream(TextWriter writer)
            {
                this.writer = writer;
            }

            public override void Flush()
            {
                writer.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                writer.Write(Encoding.Default.GetString(buffer, offset, count));
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override long Length
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            public override long Position
            {
                get
                {
                    throw new NotSupportedException();
                }
                set
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
