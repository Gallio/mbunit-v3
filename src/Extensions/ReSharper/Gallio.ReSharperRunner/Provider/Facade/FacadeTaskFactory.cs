using Gallio.ReSharperRunner.Provider.Tasks;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    public class FacadeTaskFactory
    {
        public RemoteTask CreateRootTask()
        {
            return new FacadeTaskWrapper(GallioTestRunTask.Instance);
        }

        public RemoteTask CreateAssemblyTaskFrom(GallioTestElement element)
        {
            var assemblyTask = new GallioTestAssemblyTask(element.GetAssemblyLocation(), 
                element.GetTypeClrName(), element.ShortName);
#if RESHARPER_51_OR_NEWER
            return new TestContainerFacadeTaskWrapper(assemblyTask);
#else
            return new FacadeTaskWrapper(assemblyTask);
#endif
        }

        public RemoteTask CreateExplicitTestTaskFrom(GallioTestElement element)
        {
            var explicitTask = new GallioTestExplicitTask(element.TestId, element.GetTypeClrName(), 
                element.ShortName);
            return CreateTestTask(element, explicitTask);
        }

        public RemoteTask CreateTestTaskFrom(GallioTestElement element)
        {
            var testItemTask = new GallioTestItemTask(element.TestId, element.GetTypeClrName(),
                element.ShortName);
            return CreateTestTask(element, testItemTask);
        }

        private static RemoteTask CreateTestTask(GallioTestElement element, FacadeTask facadeTask)
        {
#if RESHARPER_51_OR_NEWER
            if (element.IsTestCase)
            {
                return new UnitTestFacadeTaskWrapper(facadeTask);
            }
            return new TestContainerFacadeTaskWrapper(facadeTask);
#else
            return new FacadeTaskWrapper(facadeTask);
#endif
        }
    }
}