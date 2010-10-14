using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Facade
{
#if RESHARPER_51_OR_NEWER
    [Serializable]
    public class TestContainerFacadeTaskWrapper : FacadeTaskWrapper, ITestContainerRemoteTask
    {
        internal TestContainerFacadeTaskWrapper(FacadeTask facadeTask) : base(facadeTask) { }

        public TestContainerFacadeTaskWrapper(XmlElement element) : base(element) { }

        public string TypeName
        {
            get { return FacadeTask.TypeName; }
        }
    }
#endif
}