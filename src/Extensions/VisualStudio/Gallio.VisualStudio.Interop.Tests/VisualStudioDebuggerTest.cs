using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Runtime;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.VisualStudio.Interop.Tests
{
    [Category("Integration")]
    [TestsOn(typeof(VisualStudioDebugger))]
    public class VisualStudioDebuggerTest
    {
        [Test]
        public void Constructor_WhenDebuggerSetupIsNull_Throws()
        {
            var logger = MockRepository.GenerateStub<ILogger>();
            var visualStudio = MockRepository.GenerateStub<IVisualStudio>();

            Assert.Throws<ArgumentNullException>(() => new VisualStudioDebugger(null, logger, visualStudio));
        }

        [Test]
        public void Constructor_WhenLoggerIsNull_Throws()
        {
            var debuggerSetup = new DebuggerSetup();
            var visualStudio = MockRepository.GenerateStub<IVisualStudio>();

            Assert.Throws<ArgumentNullException>(() => new VisualStudioDebugger(debuggerSetup, null, visualStudio));
        }

        [Test]
        public void IsAttachedToProcess_WhenProcessIsNull_Throws()
        {
            var debuggerSetup = new DebuggerSetup();
            var logger = MockRepository.GenerateStub<ILogger>();
            var debugger = new VisualStudioDebugger(debuggerSetup, logger, null);

            Assert.Throws<ArgumentNullException>(() => debugger.IsAttachedToProcess(null));
        }

        [Test]
        public void AttachToProcess_WhenProcessIsNull_Throws()
        {
            var debuggerSetup = new DebuggerSetup();
            var logger = MockRepository.GenerateStub<ILogger>();
            var debugger = new VisualStudioDebugger(debuggerSetup, logger, null);

            Assert.Throws<ArgumentNullException>(() => debugger.AttachToProcess(null));
        }

        [Test]
        public void DetachFromProcess_WhenProcessIsNull_Throws()
        {
            var debuggerSetup = new DebuggerSetup();
            var logger = MockRepository.GenerateStub<ILogger>();
            var debugger = new VisualStudioDebugger(debuggerSetup, logger, null);

            Assert.Throws<ArgumentNullException>(() => debugger.DetachFromProcess(null));
        }

        [Test]
        public void LaunchProcess_WhenProcessStartInfoIsNull_Throws()
        {
            var debuggerSetup = new DebuggerSetup();
            var logger = MockRepository.GenerateStub<ILogger>();
            var debugger = new VisualStudioDebugger(debuggerSetup, logger, null);

            Assert.Throws<ArgumentNullException>(() => debugger.LaunchProcess(null));
        }

        [Test]
        public void CanAttachToProcessWithDebugger()
        {
            var debuggerSetup = new DebuggerSetup();
            var logger = new MarkupStreamLogger(TestLog.Default);
            var debugger = new VisualStudioDebugger(debuggerSetup, logger, null);

            ProcessStartInfo processStartInfo = CreateProcessStartInfoForDummyProcess();

            Process process = Process.Start(processStartInfo);

            Assert.IsFalse(debugger.IsAttachedToProcess(process), "Initially the debugger is not attached.");

            Assert.AreEqual(AttachDebuggerResult.Attached, debugger.AttachToProcess(process), "Should attach to process.");
            Assert.IsTrue(debugger.IsAttachedToProcess(process), "Should report that it has attached to process.");
            Assert.AreEqual(AttachDebuggerResult.AlreadyAttached, debugger.AttachToProcess(process), "Should report that it was already attached to process.");

            /* This fails because Visual Studio returns "The requested operation is not supported."
             * It appears to only be supported when we launch the process ourselves.
            Assert.AreEqual(DetachDebuggerResult.Detached, debugger.DetachFromProcess(process), "Should detach from process.");
            Assert.IsFalse(debugger.IsAttachedToProcess(process), "Finally the debugger is not attached.");
            Assert.AreEqual(DetachDebuggerResult.AlreadyDetached, debugger.DetachFromProcess(process), "Should report that it was already detached from process.");
             */
        }

        [Test]
        public void CanLaunchProcessWithDebugger()
        {
            var debuggerSetup = new DebuggerSetup();
            var logger = new MarkupStreamLogger(TestLog.Default);
            var debugger = new VisualStudioDebugger(debuggerSetup, logger, null);

            ProcessStartInfo processStartInfo = CreateProcessStartInfoForDummyProcess();

            Process process = debugger.LaunchProcess(processStartInfo);
            Assert.IsNotNull(process, "Should launch process with debugger.");
            Assert.Contains(process.ProcessName, "Gallio.Host");
            Assert.IsTrue(debugger.IsAttachedToProcess(process), "Should report that it has attached to process.");
            Assert.AreEqual(AttachDebuggerResult.AlreadyAttached, debugger.AttachToProcess(process), "Should report that it was already attached to process.");

            Assert.AreEqual(DetachDebuggerResult.Detached, debugger.DetachFromProcess(process), "Should detach from process.");
            Assert.IsFalse(debugger.IsAttachedToProcess(process), "Finally the debugger is not attached.");
            Assert.AreEqual(DetachDebuggerResult.AlreadyDetached, debugger.DetachFromProcess(process), "Should report that it was already detached from process.");
        }

        private static ProcessStartInfo CreateProcessStartInfoForDummyProcess()
        {
            return new ProcessStartInfo()
            {
                FileName = Path.Combine(RuntimeAccessor.RuntimePath, "Gallio.Host.exe"),
                Arguments = "/timeout:60 /ipc-port:VisualStudioDebuggerTest." + Guid.NewGuid()
            };
        }
    }
}
