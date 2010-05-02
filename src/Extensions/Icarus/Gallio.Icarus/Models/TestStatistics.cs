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

using Gallio.Icarus.Events;
using Gallio.Model;
using Gallio.UI.DataBinding;
using Gallio.UI.Events;

namespace Gallio.Icarus.Models
{
    public class TestStatistics : ITestStatistics, Handles<TestStepFinished>, Handles<TestsReset>
    {
        public Observable<int> Passed { get; private set; }
        public Observable<int> Failed { get; private set; }
        public Observable<int> Skipped { get; private set; }
        public Observable<int> Inconclusive { get; private set; }

        public TestStatistics()
        {
            Passed = new Observable<int>();
            Failed = new Observable<int>();
            Skipped = new Observable<int>();
            Inconclusive = new Observable<int>();
        }

        public void Handle(TestStepFinished @event)
        {
            if (false == @event.TestStepRun.Step.IsTestCase)
                return;

            switch (@event.TestStepRun.Result.Outcome.Status)
            {
                case TestStatus.Passed:
                    Passed.Value++;
                    break;
                case TestStatus.Failed:
                    Failed.Value++;
                    break;
                case TestStatus.Skipped:
                    Skipped.Value++;
                    break;
                case TestStatus.Inconclusive:
                    Inconclusive.Value++;
                    break;
            }
        }

        public void Handle(TestsReset @event)
        {
            Passed.Value = 0;
            Failed.Value = 0;
            Skipped.Value = 0;
            Inconclusive.Value = 0;
        }
    }
}
