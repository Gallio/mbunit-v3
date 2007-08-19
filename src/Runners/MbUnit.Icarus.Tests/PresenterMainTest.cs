using MbUnit.Core.Harness;
using MbUnit.Core.Runner;
using MbUnit.Core.Services.Runtime;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Icarus.Core.Model;

namespace MbUnit.Icarus.Tests
{
    [TestFixture]
    public class PresenterMainTest
    {
        [Test]
        public void LoadUpAssemblyAndGetTestTree_Test()
        {
            RuntimeSetup runtimeSetup = new RuntimeSetup();
            AutoRunner runner = AutoRunner.CreateRunner(runtimeSetup);

            TestPackage testpackage = new TestPackage();
            testpackage.AssemblyFiles.Add("C:\\Source\\MbUnitGoogle\\mb-unit\\v3\\src\\TestResources\\MbUnit.TestResources.MbUnit2\\bin\\MbUnit.TestResources.MbUnit2.dll");
            
            TestRunnerModel main = new TestRunnerModel();
            
            TestModel t = main.LoadUpAssembly(runner, testpackage);
        }

    }
    
}
