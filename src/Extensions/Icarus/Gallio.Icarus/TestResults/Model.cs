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

using System.Collections.Generic;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Icarus.TestResults
{
    public class Model : IModel {
        private readonly IDictionary<string, IList<TestStepRun>> testStepRuns;

        public Model()
        {
            testStepRuns = new Dictionary<string, IList<TestStepRun>>();
        }

        public void AddTestStepRun(string testId, TestStepRun testStepRun)
        {
            IList<TestStepRun> runs;
            if (!testStepRuns.ContainsKey(testId))
            {
                runs = new List<TestStepRun>();
                testStepRuns.Add(testId, runs);
            }
            else 
            {
                runs = testStepRuns[testId];
            }
            runs.Add(testStepRun);
        }

        public IList<TestStepRun> GetTestStepRuns(string testId)
        {
            if (testStepRuns.ContainsKey(testId)) 
            {
                return testStepRuns[testId];
            }
            return new List<TestStepRun>();
        }
    }
}
