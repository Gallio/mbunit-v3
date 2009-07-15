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
using System.Collections.Generic;
using System.Windows.Forms;
using Gallio.Common.IO;
using Gallio.Common.Policies;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Copy
{
    internal class CopyController : ICopyController
    {
        private readonly IFileSystem fileSystem;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private readonly ITaskManager taskManager;
        private readonly IRegistry registry;
        private readonly Timer timer = new Timer();
        private List<string> plugins;

        public ObservableProgressMonitor ProgressMonitor
        {
            get { return taskManager.ProgressMonitor; }
        }

        public event EventHandler ShowProgressDialog;
        public event EventHandler ProgressUpdate;

        public CopyController(IFileSystem fileSystem, IUnhandledExceptionPolicy unhandledExceptionPolicy, 
            ITaskManager taskManager, IRegistry registry)
        {
            this.fileSystem = fileSystem;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
            this.taskManager = taskManager;
            this.registry = registry;

            taskManager.ProgressUpdate += (sender, e) => EventHandlerPolicy.SafeInvoke(ProgressUpdate, this, EventArgs.Empty);
            taskManager.TaskStarted += (sender, e) => timer.Start();
            taskManager.TaskCompleted += (sender, e) => timer.Stop();
            taskManager.TaskCanceled += (sender, e) => timer.Stop();

            timer.Interval = 2000;
            timer.Tick += (sender, e) =>
            {
                EventHandlerPolicy.SafeInvoke(ShowProgressDialog, this, EventArgs.Empty);
                timer.Stop();
            };
        }

        public IList<string> Plugins
        {
            get
            {
                if (plugins == null)
                {
                    plugins = new List<string>();
                    foreach (var pluginDescriptor in RuntimeAccessor.Registry.Plugins)
                    {
                        if (pluginDescriptor.PluginId != "BuiltIn")
                            plugins.Add(pluginDescriptor.PluginId);
                    }
                    plugins.Sort();
                }
                return plugins;
            }
        }

        public void CopyTo(string destinationFolder, IList<string> selectedPlugins)
        {
            var copyCommand = new CopyCommand(destinationFolder, selectedPlugins, registry, 
                fileSystem, unhandledExceptionPolicy);

            taskManager.QueueTask(copyCommand);
        }
    }
}
