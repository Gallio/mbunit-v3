// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using Gallio.Common.IO;
using Gallio.Common.Xml;
using Gallio.Icarus.Events;
using Gallio.Icarus.Properties;
using Gallio.UI.Common.Policies;
using Gallio.UI.Events;

namespace Gallio.Icarus.Projects
{
    public class UserOptionsController : IUserOptionsController, Handles<ProjectLoaded>, Handles<ProjectSaved>, 
        Handles<TreeViewCategoryChanged>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerializer xmlSerializer;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;

        private bool unsavedChanges;

        public IEnumerable<string> CollapsedNodes { get; private set; }
        public string TreeViewCategory { get; private set; }

        public UserOptionsController(IEventAggregator eventAggregator, IFileSystem fileSystem,
            IXmlSerializer xmlSerializer, IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.eventAggregator = eventAggregator;
            this.fileSystem = fileSystem;
            this.xmlSerializer = xmlSerializer;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;

            TreeViewCategory = UserOptions.DefaultTreeViewCategory;
            CollapsedNodes = new List<string>();
        }

        private void LoadUserOptions(string projectLocation)
        {
            var projectUserOptionsFile = projectLocation + UserOptions.Extension;
            var userOptions = new UserOptions();

            if (fileSystem.FileExists(projectUserOptionsFile))
            {
                try
                {
                    userOptions = xmlSerializer.LoadFromXml<UserOptions>(projectUserOptionsFile);                
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report(Resources.UserOptionsController_Failed_to_load_user_options_, ex);
                }
            }

            TreeViewCategory = userOptions.TreeViewCategory;
            eventAggregator.Send(new TreeViewCategoryChanged(TreeViewCategory));

            CollapsedNodes = userOptions.CollapsedNodes;

            eventAggregator.Send(new UserOptionsLoaded());
        }

        private void SaveUserOptions(string projectName)
        {
            if (unsavedChanges == false)
                return;

            var projectUserOptionsFile = projectName + UserOptions.Extension;
            var userOptions = new UserOptions
            {
                TreeViewCategory = TreeViewCategory,
                CollapsedNodes = new List<string>(CollapsedNodes)
            };

            xmlSerializer.SaveToXml(userOptions, projectUserOptionsFile);

            eventAggregator.Send(new UserOptionsSaved());
        }

        public void SetCollapsedNodes(IEnumerable<string> collapsedNodes)
        {
            CollapsedNodes = collapsedNodes;
            unsavedChanges = true;
        }

        public void Handle(ProjectLoaded @event)
        {
            LoadUserOptions(@event.ProjectLocation);
        }

        public void Handle(ProjectSaved @event)
        {
            SaveUserOptions(@event.ProjectLocation);
        }

        public void Handle(TreeViewCategoryChanged @event)
        {
            var treeViewCategory = @event.TreeViewCategory;

            if (TreeViewCategory == treeViewCategory)
                return;

            TreeViewCategory = treeViewCategory;
            unsavedChanges = true;
        }
    }
}
