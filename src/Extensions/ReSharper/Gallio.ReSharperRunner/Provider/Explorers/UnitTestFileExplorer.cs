using JetBrains.Application;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

namespace Gallio.ReSharperRunner.Provider.Explorers
{
    [FileUnitTestExplorer]
    public class UnitTestFileExplorer : IUnitTestFileExplorer
    {
        private readonly GallioTestProvider provider;

        public UnitTestFileExplorer(GallioTestProvider provider)
        {
            this.provider = provider;
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            provider.ExploreFile(psiFile, consumer, interrupted);
        }

        public IUnitTestProvider Provider
        {
            get { return provider; }
        }
    }
}