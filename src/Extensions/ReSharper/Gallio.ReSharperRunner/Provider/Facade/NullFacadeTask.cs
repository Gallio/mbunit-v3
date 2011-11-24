using System;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    [Serializable]
    public class NullFacadeTask : FacadeTask
    {
        public NullFacadeTask() : base("", "")
        {
        }
    }
}