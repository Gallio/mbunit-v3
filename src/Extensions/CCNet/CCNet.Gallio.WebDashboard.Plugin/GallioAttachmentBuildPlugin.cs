using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace CCNet.Gallio.WebDashboard.Plugin
{
    [ReflectorType("gallioAttachmentBuildPlugin")]
    public class GallioAttachmentBuildPlugin : IBuildPlugin
    {
        private readonly IActionInstantiator actionInstantiator;

        public GallioAttachmentBuildPlugin(IActionInstantiator actionInstantiator)
        {
            this.actionInstantiator = actionInstantiator;
        }

        public bool IsDisplayedForProject(IProjectSpecifier project)
        {
            return false;
        }

        public INamedAction[] NamedActions
        {
            get
            {
                GallioAttachmentBuildAction action = (GallioAttachmentBuildAction)
                    actionInstantiator.InstantiateAction(typeof(GallioAttachmentBuildAction));

                return new INamedAction[] { new ImmutableNamedAction(@"GallioAttachment", action) };
            }
        }

        public string LinkDescription
        {
            get { throw new NotSupportedException(); }
        }
    }
}
