using System;
using Gallio.TDNetRunner.Facade;
using TestDriven.Framework;
using TestDriven.Framework.Resident;

namespace Gallio.TDNetRunner
{
    /// <summary>
    /// Gallio resident test runner for TestDriven.NET.
    /// (A resident test runner is one that TestDriven.Net creates within its principal
    /// AppDomain instead of starting a new one for each test.  So this way Gallio can
    /// start memory resident across multiple test runs.
    /// </summary>
    public class GallioResidentTestRunner : BaseTestRunner, IResidentTestRunner
    {
        public TestRunState Run(ITestListener testListener, string assemblyFile, string cref)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (assemblyFile == null)
                throw new ArgumentNullException("assemblyFile");

            FacadeTestRunState result = TestRunner.Run(new AdapterFacadeTestListener(testListener), assemblyFile, cref);
            return FacadeUtils.ToTestRunState(result);
        }

        public void Abort()
        {
            TestRunner.Abort();
        }
    }
}