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

using Gallio.Runner;
using Gallio.NAntTasks;
using NAnt.Core;

namespace Gallio.NAntTasks.Tests
{
    /// <summary>
    /// Makes possible to unit test the <see cref="GallioTask" /> class.
    /// In particular we need to disable the initialization of a new runtime
    /// because it will conflict with the test execution environment.
    /// </summary>
    /// <remarks>
    /// The NAnt.Core.Task class is hard to unit test for a number of reasons. The
    /// main is that the Execute() method uses a Logger and a Project member. The
    /// Project property therefore must be assigned, but Project class itself
    /// requires a lot of stuff to be instantiated, and since it's a concrete class
    /// is hard to be mocked. This class therefore exposes the protected ExecuteTask
    /// method that don't use those properties.
    /// Also, for some reason the the Log method of the task will fail with a
    /// NullReference exception in the IsLogEnabledFor(Level messageLevel) method,
    /// so we need to avoid calling the instance Log methods directly and use a
    /// interface too.
    /// </remarks>
    public class InstrumentedGallioTask : GallioTask
    {
        // We need to instantiate our own dictionary of properties
        // WARNING: This could break in the future if a check for null is added
        // in the PropertyDictionary constructor
        private readonly PropertyDictionary properties = new PropertyDictionary(null);

        public InstrumentedGallioTask(INAntLogger nantLogger)
            : base(nantLogger)
        {
        }

        public new void Execute()
        {
            // We call the ExecuteTask directly instead of the Execute method
            base.ExecuteTask();
        }

        public override PropertyDictionary Properties
        {
            get { return properties; }
        }

        protected override TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            launcher.RuntimeSetup = null;
            launcher.TestRunnerFactory = TestRunnerFactory.CreateLocalTestRunner;
            return base.RunLauncher(launcher);
        }
    }
}
