extern alias MbUnit2;
using MbUnit2::MbUnit.Framework;
using System;
using MbUnit.Tasks.MSBuild;
using Microsoft.Build.Utilities;

namespace MbUnit.Tasks.MSBuild.Tests
{
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(MSBuildLogger))]
    public class MSBuildLoggerTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InstantiateLogeerWithNullArgument()
        {
            MSBuildLogger logger = new MSBuildLogger(null);
        }

        [Test]
        public void InstantiateLogger()
        {
            MbUnit task = new MbUnit();
            TaskLoggingHelper taskLogger = new TaskLoggingHelper(task);
            new MSBuildLogger(taskLogger);
        }

        [Test]
        public void CreateChildLogger()
        {
            MbUnit task = new MbUnit();
            TaskLoggingHelper taskLogger = new TaskLoggingHelper(task);
            MSBuildLogger logger = new MSBuildLogger(taskLogger);
            Assert.AreSame(logger.CreateChildLogger("child").GetType(), typeof (MSBuildLogger));
        }
    }
}
