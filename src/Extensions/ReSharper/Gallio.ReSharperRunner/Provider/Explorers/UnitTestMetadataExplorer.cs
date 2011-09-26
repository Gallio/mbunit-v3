using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Gallio.ReSharperRunner.Provider.Explorers
{
    [MetadataUnitTestExplorer]
    public class UnitTestMetadataExplorer : IUnitTestMetadataExplorer
    {
        private readonly GallioTestProvider provider;

        public UnitTestMetadataExplorer(GallioTestProvider provider)
        {
            this.provider = provider;
        }

        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
        {
            provider.ExploreAssembly(assembly, project, consumer);
        }

        public IUnitTestProvider Provider
        {
            get { return provider; }
        }
    }
}