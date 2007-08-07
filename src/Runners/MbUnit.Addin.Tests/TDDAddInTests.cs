using System;
using MbUnit.AddIn;
using TDF = TestDriven.Framework;

namespace MbUnit.AddIn.Tests
{
    public class TDDAddInTests
    {
        public void Test()
        {
            TDF.ITestRunner tr = new MbUnitTestRunner();
            tr.RunAssembly(null, null);
        }
    }
}
