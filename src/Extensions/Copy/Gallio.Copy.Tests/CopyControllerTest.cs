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

using Gallio.Common.IO;
using Gallio.Copy.Commands;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Copy.Tests
{
    public class CopyControllerTest
    {
        private ITaskManager taskManager;
        private CopyController controller;
        private IFileSystem fileSystem;
        private IUnhandledExceptionPolicy unhandledExceptionPolicy;

        [SetUp]
        public void SetUp()
        {
            taskManager = MockRepository.GenerateStub<ITaskManager>();
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            controller = new CopyController(taskManager, fileSystem);
        }

        [Test]
        public void UpdateSourcePluginFolder_should_queue_a_command_with_the_correct_plugin_model()
        {
            controller.UpdateSourcePluginFolder("pluginFolder");

            taskManager.AssertWasCalled(tm => tm.QueueTask(Arg<UpdatePluginFolderCommand>.
                Matches(c => c.PluginTreeModel == controller.SourcePlugins)));
        }

        [Test]
        public void UpdateSourcePluginFolder_should_queue_a_command_with_the_correct_folder()
        {
            const string pluginFolder = "pluginFolder";

            controller.UpdateSourcePluginFolder(pluginFolder);

            taskManager.AssertWasCalled(tm => tm.QueueTask(Arg<UpdatePluginFolderCommand>.
                Matches(c => c.PluginFolder == pluginFolder)));
        }

        [Test]
        public void UpdateTargetPluginFolder_should_queue_a_command_with_the_correct_plugin_model()
        {
            controller.UpdateTargetPluginFolder("pluginFolder");

            taskManager.AssertWasCalled(tm => tm.QueueTask(Arg<UpdatePluginFolderCommand>.
                Matches(c => c.PluginTreeModel == controller.TargetPlugins)));
        }

        [Test]
        public void UpdateTargetPluginFolder_should_queue_a_command_with_the_correct_folder()
        {
            const string pluginFolder = "pluginFolder";

            controller.UpdateTargetPluginFolder(pluginFolder);

            taskManager.AssertWasCalled(tm => tm.QueueTask(Arg<UpdatePluginFolderCommand>.
                Matches(c => c.PluginFolder == pluginFolder)));
        }

        [Test]
        public void Shutdown_should_clear_task_queue()
        {
            taskManager.Stub(tm => tm.ProgressMonitor).Return(new ObservableProgressMonitor());

            controller.Shutdown();

            taskManager.AssertWasCalled(tm => tm.ClearQueue());
        }

        [Test]
        public void Shutdown_should_cancel_current_progress()
        {
            var progressMonitor = new ObservableProgressMonitor();
            taskManager.Stub(tm => tm.ProgressMonitor).Return(progressMonitor);

            controller.Shutdown();

            Assert.IsTrue(progressMonitor.IsCanceled);
        }
    }
}
