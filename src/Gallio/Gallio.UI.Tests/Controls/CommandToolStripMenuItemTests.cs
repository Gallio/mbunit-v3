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

using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Controls;
using Gallio.UI.DataBinding;
using Gallio.UI.Menus;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.UI.Tests.Controls
{
    public class CommandToolStripMenuItemTests
    {
        [Test]
        public void Menu_item_text_should_match_command()
        {
            var menuCommand = new MenuCommand { Text = "someText" };

            var menuItem = new CommandToolStripMenuItem(menuCommand);

            Assert.AreEqual(menuItem.Text, menuCommand.Text);
        }

        [Test]
        public void Menu_item_should_be_enabled_if_command_can_execute()
        {
            var menuCommand = new MenuCommand { CanExecute = new Observable<bool>(true) };

            var menuItem = new CommandToolStripMenuItem(menuCommand);

            Assert.IsTrue(menuItem.Enabled);
        }

        [Test]
        public void Menu_item_should_become_enabled_if_command_can_execute()
        {
            var canExecute = new Observable<bool>();
            var menuCommand = new MenuCommand { CanExecute = canExecute };
            var menuItem = new CommandToolStripMenuItem(menuCommand);
            Assert.IsFalse(menuItem.Enabled);

            canExecute.Value = true;

            Assert.IsTrue(menuItem.Enabled);
        }

        [Test]
        public void Menu_item_should_run_command_when_clicked()
        {
            var command = MockRepository.GenerateStub<ICommand>();
            var menuCommand = new MenuCommand { Command = command };
            var menuItem = new CommandToolStripMenuItem(menuCommand);

            menuItem.PerformClick();

            command.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Menu_item_should_run_command_with_task_manager_if_provided_when_clicked()
        {
            var command = MockRepository.GenerateStub<ICommand>();
            var menuCommand = new MenuCommand { Command = command };
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var menuItem = new CommandToolStripMenuItem(menuCommand, taskManager);

            menuItem.PerformClick();

            taskManager.AssertWasCalled(tm => tm.QueueTask(command));
        }

        [Test]
        public void Menu_shortcut_should_be_set_if_provided()
        {
            const string shortcut = "S";
            var menuCommand = new MenuCommand { Shortcut = shortcut };
            var keysParser = MockRepository.GenerateStub<IKeysParser>();

            new CommandToolStripMenuItem(menuCommand, null, keysParser);

            keysParser.AssertWasCalled(kp => kp.Parse(shortcut));
        }
    }
}
