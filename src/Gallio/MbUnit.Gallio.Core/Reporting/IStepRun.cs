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
using MbUnit.Framework.Kernel.Results;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// Summarizes the execution of a single test step for reporting purposes.
    /// </summary>
    public interface IStepRun
    {
        ///<summary>
        /// Gets or sets the id of the step.
        ///</summary>
        string StepId { get; set;}
        ///<summary>
        /// Gets or sets the name of the step.
        ///</summary>
        string StepName { get; set;}
        ///<summary>
        /// Gets or sets the time when the test run started.
        ///</summary>
        DateTime StartTime { get; set;}
        ///<summary>
        /// Gets or sets the time when the test run ended.
        ///</summary>
        DateTime EndTime { get; set;}
        ///<summary>
        /// Gets the list of child steps.
        ///</summary>
        List<IStepRun> Children { get;}
        ///<summary>
        /// Gets or sets the test result from the run.
        ///</summary>
        TestResult Result { get; set;}
    }
}
