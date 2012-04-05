using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Caches2;
using JetBrains.ReSharper.UnitTestFramework;

namespace Gallio.ReSharperRunner.Provider.Explorers
{
    [MetadataUnitTestExplorer]
    public class UnitTestMetadataExplorer : IUnitTestMetadataExplorer
    {
        private readonly GallioTestProvider provider;

#if RESHARPER_60
        public UnitTestMetadataExplorer(GallioTestProvider provider)
#else
		public UnitTestMetadataExplorer(GallioTestProvider provider, PsiModuleManager psiModuleManager, CacheManagerEx cacheManager, IUnitTestProvidersManager unitTestProvidersManager)
#endif
        {
            this.provider = provider;
#if RESHARPER_61_OR_NEWER
        	provider.PsiModuleManager = psiModuleManager;
        	provider.CacheManager = cacheManager;
			provider.UnitTestProvidersManager = unitTestProvidersManager;
#endif
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