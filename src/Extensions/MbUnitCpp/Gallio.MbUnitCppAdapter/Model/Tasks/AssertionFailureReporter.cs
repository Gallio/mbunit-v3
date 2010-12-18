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

using System;
using System.Collections.Generic;
using System.Text;
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Model.Contexts;
using Gallio.Model;
using Gallio.Framework.Assertions;

namespace Gallio.MbUnitCppAdapter.Model.Tasks
{
    /// <summary>
    /// Reports an MbUnitCpp assertion failure.
    /// </summary>
    internal class AssertionFailureReporter
    {
        private readonly UnmanagedTestRepository repository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="repository">The unmanaged test repository.</param>
        public AssertionFailureReporter(UnmanagedTestRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Reports an assertion failure.
        /// </summary>
        /// <param name="testContext"></param>
        /// <param name="testInfoData"></param>
        /// <param name="testStepResult"></param>
        public void Run(ITestContext testContext, TestInfoData testInfoData, TestStepResult testStepResult)
        {
            if (testStepResult.TestOutcome == TestOutcome.Failed)
            {
                MbUnitCppAssertionFailure failure = testStepResult.Failure;
                var builder = new AssertionFailureBuilder(failure.Description);

                if (failure.HasExpectedValue && failure.HasActualValue && failure.Diffing)
                {
                    builder.AddRawExpectedAndActualValuesWithDiffs(failure.ExpectedValue, failure.ActualValue);
                }
                else if (failure.HasUnexpectedValue && failure.HasActualValue && failure.Diffing)
                {
                    builder.AddRawLabeledValuesWithDiffs("Unexpected Value", failure.UnexpectedValue, "Actual Value", failure.ActualValue);
                }
                else
                {
                    if (failure.HasExpectedValue)
                        builder.AddRawExpectedValue(failure.ExpectedValue);

                    if (failure.HasActualValue)
                        builder.AddRawActualValue(failure.ActualValue);

                    if (failure.HasUnexpectedValue)
                        builder.AddRawLabeledValue("Unexpected Value", failure.UnexpectedValue);
                }

                foreach (var extra in failure.ExtraLabeledValues)
                {
                    builder.AddRawLabeledValue(extra.First, extra.Second);
                }

                if (failure.Message.Length > 0)
                    builder.SetMessage(failure.Message);

                builder.SetStackTrace(testInfoData.GetStackTraceData());
                builder.ToAssertionFailure().WriteTo(testContext.LogWriter.Failures);
            }
        }
    }
}
