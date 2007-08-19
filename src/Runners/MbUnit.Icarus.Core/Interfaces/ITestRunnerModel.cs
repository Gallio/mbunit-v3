using MbUnit.Core.Harness;
using MbUnit.Core.Runner;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Icarus.Core.Interfaces
{
    public interface ITestRunnerModel
    {
        TestModel LoadUpAssembly(ITestRunner runner, TestPackage testpackage);
    }
}