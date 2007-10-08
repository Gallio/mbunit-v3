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
using MbUnit.Model.Events;

namespace MbUnit.Model.Execution
{
    /// <summary>
    /// A factory for test plans.
    /// </summary>
    public interface ITestPlanFactory
    {
        /// <summary>
        /// Creates a test plan.
        /// </summary>
        /// <param name="listener">The test listener</param>
        /// <returns>The test plan</returns>
        ITestPlan CreateTestPlan(ITestListener listener);
    }
}
