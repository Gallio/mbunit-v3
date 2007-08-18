using MbUnit.Core.Runner;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Icarus.Core.Interfaces
{
    public interface IMainOpModel
    {
        TestModel LoadUpAssembly(ITestRunner runner);
    }
}