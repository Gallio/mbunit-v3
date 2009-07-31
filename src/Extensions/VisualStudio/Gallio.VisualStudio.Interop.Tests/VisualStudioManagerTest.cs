using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using MbUnit.Framework;

namespace Gallio.VisualStudio.Interop.Tests
{
    [Category("Integration")]
    [TestsOn(typeof(VisualStudioManager))]
    public class VisualStudioManagerTest
    {
        [Test]
        public void GetVisualStudio_WhenLoggerIsNull_Throws()
        {
            var manager = VisualStudioManager.Instance;

            Assert.Throws<ArgumentNullException>(() => manager.GetVisualStudio(VisualStudioVersion.Any, false, null));
        }

        [Test]
        public void LaunchVisualStudio_WhenLoggerIsNull_Throws()
        {
            var manager = VisualStudioManager.Instance;

            Assert.Throws<ArgumentNullException>(() => manager.LaunchVisualStudio(VisualStudioVersion.Any, null));
        }

        [Test]
        public void LaunchAndGetVisualStudio()
        {
            var manager = VisualStudioManager.Instance;
            var logger = new MarkupStreamLogger(TestLog.Default);

            IVisualStudio visualStudio = manager.LaunchVisualStudio(VisualStudioVersion.Any, logger);
            Assert.IsNotNull(visualStudio, "Should have started an instance.");

            try
            {
                Assert.IsTrue(visualStudio.WasLaunched, "New instance launched so WasLaunched should be true.");

                AssertVisualStudioCanBeUsed(visualStudio);
                AssertExistingInstanceOfVisualStudioCanBeLocated(manager, logger, visualStudio.Version);
            }
            finally
            {
                visualStudio.Quit();
            }
        }

        [Test]
        public void GetVisualStudio_WhenVisualStudioIsNotRunning_ReturnsNull()
        {
            var manager = VisualStudioManager.Instance;
            var logger = new MarkupStreamLogger(TestLog.Default);

            foreach (VisualStudioVersion version in new[] { VisualStudioVersion.VS2005, VisualStudioVersion.VS2008, VisualStudioVersion.VS2010 })
            {
                if (manager.GetVisualStudio(version, false, logger) == null)
                    return;
            }

            Assert.Inconclusive("Expected to find at least one version of Visual Studio not already running but they all seem to be.");
        }

        [Test]
        public void GetVisualStudio_WhenLaunchFlagIsTrue_ProvidesAnInstanceOrStartsOne()
        {
            var manager = VisualStudioManager.Instance;
            var logger = new MarkupStreamLogger(TestLog.Default);

            IVisualStudio visualStudio = manager.GetVisualStudio(VisualStudioVersion.Any, true, logger);
            Assert.IsNotNull(visualStudio, "Should have found or started an instance.");

            try
            {
                AssertVisualStudioCanBeUsed(visualStudio);
                AssertExistingInstanceOfVisualStudioCanBeLocated(manager, logger, visualStudio.Version);
            }
            finally
            {
                if (visualStudio.WasLaunched)
                    visualStudio.Quit();
            }
        }

        private static void AssertVisualStudioCanBeUsed(IVisualStudio visualStudio)
        {
            string version = null;
            visualStudio.Call(dte => version = dte.Version);
            Assert.IsNotNull(version);

            Assert.Throws<ArgumentNullException>(() => visualStudio.Call(null));

            Assert.DoesNotThrow(() => visualStudio.BringToFront());

            Assert.IsNotNull(visualStudio.GetDebugger(new DebuggerSetup()));

            Assert.Throws<ArgumentNullException>(() => visualStudio.GetDebugger(null));
        }

        private static void AssertExistingInstanceOfVisualStudioCanBeLocated(IVisualStudioManager manager, ILogger logger, VisualStudioVersion version)
        {
            IVisualStudio foundVisualStudio = manager.GetVisualStudio(version, false, logger);
            Assert.IsNotNull(foundVisualStudio, "Since Visual Studio is running, GetVisualStudio should return a valid instance.");
            Assert.IsFalse(foundVisualStudio.WasLaunched, "Existing instance found so WasLaunched should be false.");

            foundVisualStudio = manager.GetVisualStudio(version, true, logger);
            Assert.IsNotNull(foundVisualStudio, "Since Visual Studio is running, GetVisualStudio should return a valid instance.");
            Assert.IsFalse(foundVisualStudio.WasLaunched, "Existing instance found so WasLaunched should be false.");
        }
    }
}
