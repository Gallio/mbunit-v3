using System.Diagnostics;
using Autodesk.AutoCAD.ApplicationServices;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.AutoCAD.Tests.Integration
{
    /// <summary>
    /// A sample test to run within AutoCAD.
    /// </summary>
    [Explicit("Sample test.")]
    public class AcadSampleTest
    {
        [Test]
        public void Test()
        {
            TestLog.WriteLine("Running with AutoCAD version {0}.", Application.Version);
            TestLog.WriteLine("Process name {0}.", Process.GetCurrentProcess().ProcessName);

            Assert.IsNotNull(Application.DocumentManager);
        }
    }
}