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
using Gallio.Common.IO;
using Gallio.Copy.Commands;
using Gallio.Copy.Events;
using Gallio.Copy.Model;
using Gallio.Loader;
using Gallio.UI.Common.Policies;
using Gallio.UI.DataBinding;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Copy.Controllers
{
    public class CopyController : ICopyController, Handles<PluginFolderUpdated>
    {
        private readonly ITaskManager taskManager;
        private readonly IUnhandledExceptionPolicy exceptionPolicy;
        private readonly IEventAggregator eventAggregator;
        private readonly IFileSystem fileSystem;

        public Observable<string> SourcePluginFolder { get; private set; }
        public Observable<string> TargetPluginFolder { get; private set; }

        public event EventHandler<PluginFolderUpdatedEvent> PluginFolderUpdated = (s, e) => { };

        public PluginTreeModel SourcePlugins { get; private set; }
        public PluginTreeModel TargetPlugins { get; private set; }

        public CopyController(ITaskManager taskManager, IFileSystem fileSystem, IUnhandledExceptionPolicy exceptionPolicy, 
            IEventAggregator eventAggregator)
        {
            this.taskManager = taskManager;
            this.fileSystem = fileSystem;
            this.exceptionPolicy = exceptionPolicy;
            this.eventAggregator = eventAggregator;

            SourcePluginFolder = new Observable<string>();
            TargetPluginFolder = new Observable<string>();

            SourcePlugins = new PluginTreeModel(fileSystem);
            TargetPlugins = new PluginTreeModel(fileSystem);
        }

        public void CopyPlugins()
        {
            var selectedPlugins = SourcePlugins.GetSelectedPlugins();
            
            var command = new CopyPluginsCommand(selectedPlugins, SourcePluginFolder, 
                TargetPluginFolder, exceptionPolicy, fileSystem);
            taskManager.QueueTask(command);
            
            UpdatePluginFolder(TargetPlugins, TargetPluginFolder, "target");
        }

        public void Load()
        {
            UpdateSourcePluginFolder(GetSourcePluginFolder());
        }

        public void Shutdown()
        {
            taskManager.ClearQueue();
        }

        public void UpdateSourcePluginFolder(string sourcePluginFolder)
        {
            SourcePluginFolder.Value = sourcePluginFolder;
            UpdatePluginFolder(SourcePlugins, sourcePluginFolder, "source");
        }

        public void UpdateTargetPluginFolder(string targetPluginFolder)
        {
            TargetPluginFolder.Value = targetPluginFolder;
            UpdatePluginFolder(TargetPlugins, targetPluginFolder, "target");
        }

        private void UpdatePluginFolder(PluginTreeModel pluginTreeModel, string targetPluginFolder, string folder)
        {
            var command = new UpdatePluginFolderCommand(pluginTreeModel, 
                targetPluginFolder, folder, eventAggregator);
            taskManager.QueueTask(command);
        }

        private static string GetSourcePluginFolder()
        {
            return new DefaultRuntimeLocator().GetRuntimePath();
        }

        public void Handle(PluginFolderUpdated @event)
        {
            PluginFolderUpdated(this, new PluginFolderUpdatedEvent(@event.Folder));
        }
    }
}
