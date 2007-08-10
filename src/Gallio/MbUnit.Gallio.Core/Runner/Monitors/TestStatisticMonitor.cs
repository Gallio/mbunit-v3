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

using System;
using System.Collections.Generic;
using System.Text;

using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Results;

namespace MbUnit.Core.Runner.Monitors
{
    /// <summary>
    /// A test statistic monitor tracks <see cref="ITestRunner" /> events and builds
    /// a report of the test outcomes.
    /// </summary>
    /// <todo author="graham">
    /// Tentative.  Subject to change!!
    /// </todo>
    public class TestStatisticMonitor : BaseTestRunnerMonitor
    {
        #region Private members

        private int run = 0;
        private int passed = 0;
        private int failed = 0;
        private int inconclusive = 0;
        private int ignored = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a test summary tracker initially with no contents.
        /// </summary>
        public TestStatisticMonitor()
        { }

        #endregion

        /// <summary>
        /// Gets a dictionary of test summaries indexed by test id.
        /// </summary>
        public string Summary
        {
            get
            {
                return String.Format("Run: {0}, Passed: {1}, Failed: {2}, Inconclusive: {3}, Ignored: {4}.", 
                    run, passed, failed, inconclusive, ignored);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttach()
        {
            base.OnAttach();
            Runner.EventDispatcher.TestLifecycle += HandleTestLifecycleEvent;
        }

        private void HandleTestLifecycleEvent(object sender, TestLifecycleEventArgs e)
        {
            switch (e.EventType)
            {
                case TestLifecycleEventType.Start:
                case TestLifecycleEventType.Step:
                    break;

                case TestLifecycleEventType.Finish:
                    lock (this)
                    {
                        run++;
                        switch (e.Result.Outcome)
                        {
                            case TestOutcome.Passed:
                                passed++;
                                break;
                            case TestOutcome.Inconclusive:
                                if (e.Result.State == TestState.Ignored)
                                {
                                    ignored++;
                                }
                                else
                                {
                                    inconclusive++;
                                }
                                break;
                            case TestOutcome.Failed:
                                failed++;
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
