using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Facade
{
#if RESHARPER_51_OR_NEWER
    [Serializable]
    public class UnitTestFacadeTaskWrapper : FacadeTaskWrapper, IUnitTestRemoteTask
    {
        internal UnitTestFacadeTaskWrapper(FacadeTask facadeTask) : base(facadeTask) { }

        public UnitTestFacadeTaskWrapper(XmlElement element) : base(element) { }

        public string TypeName
        {
            get { return FacadeTask.TypeName; }
        }

        public string ShortName
        {
            get { return FacadeTask.ShortName; }
        }
    }
#endif
}