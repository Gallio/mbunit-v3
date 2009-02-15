// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
