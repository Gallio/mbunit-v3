using System;
using System.IO;
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Utilities
{
    [TestFixture]
    [TestsOn(typeof(CurrentDirectorySwitcher))]
    public class CurrentDirectorySwitcherTest
    {
        [Test, ExpectedArgumentNullException]
        public void ShouldThrowIfDirectoryIsNull()
        {
            new CurrentDirectorySwitcher(null);
        }

        [Test]
        public void ShouldSetAndResetCurrentDirectory()
        {
            string originalDirectory = Environment.CurrentDirectory;
            string newDirectory = Path.GetTempPath().TrimEnd('\\');

            using (new CurrentDirectorySwitcher(newDirectory))
            {
                Assert.AreEqual(newDirectory, Environment.CurrentDirectory);
            }

            Assert.AreEqual(originalDirectory, Environment.CurrentDirectory);
        }
    }
}
