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

using Gallio.PowerShellCommands;
using Gallio.Runner;
using MbUnit.Framework;

namespace Gallio.PowerShellCommands.Tests
{
    /// <summary>
    /// Makes it possible to unit test the <see cref="RunGallioCommand" /> cmdlet.
    /// </summary>
    internal class StubbedRunGallioCommand : RunGallioCommand
    {
        public delegate TestLauncherResult RunLauncherDelegate(TestLauncher launcher);
        private RunLauncherDelegate action;

        public void SetRunLauncherAction(RunLauncherDelegate action)
        {
            this.action = action;
        }

        protected override TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            Assert.IsNotNull(action, "The run launcher method should not have been called because no action was set.");
            return action(launcher);
        }
    }
}
