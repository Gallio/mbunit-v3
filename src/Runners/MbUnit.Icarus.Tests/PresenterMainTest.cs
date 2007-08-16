using MbUnit.Core.Runner;
using MbUnit.Core.Services.Runtime;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Serialization;
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

            MainOpModel main = new MainOpModel();

            TestModel t = main.LoadUpAssembly(runner);
        }

    }
    
}
