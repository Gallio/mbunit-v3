// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace Gallio.PowerShellCommands.Tests
{
    /// <summary>
    /// Makes it possible to unit test the <see cref="RunGallioCommand" /> cmdlet.
    /// In particular we need to disable the initialization of a new runtime
    /// because it will conflict with the test execution environment.
    /// </summary>
    internal class InstrumentedRunGallioCommand : RunGallioCommand
    {
        public new TestLauncherResult Execute()
        {
            return base.Execute();
        }

        protected override TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            launcher.RuntimeSetup = null;
            return base.RunLauncher(launcher);
        }
    }
}
