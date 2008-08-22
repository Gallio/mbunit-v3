// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Linq;
using System.Text;
using Gallio.Tests.Integration;
using Gallio.Model;
using Gallio.Runner.Reports;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    /// <summary>
    /// Abstract base class for integration tests 
    /// on the contract verifier attributes.
    /// </summary>
    public abstract class VerifyContractAttributeBaseTest : BaseSampleTest
    {
        /// <summary>
        /// Launch the test runner on the specified fixture, then 
        /// verify that the specified test method has been run or not, and
        /// that it gave the expected result.
        /// </summary>
        /// <param name="testPatternName">Name of test pattern for the tested contract verifier.</param>
        /// <param name="fixtureType">Type of the sample fixture which must be run.</param>
        /// <param name="testMethodName">Name of evaluated test method.</param>
        /// <param name="expectedTestStatus">Expected test status. If <see cref="TestStatus.Inconclusive" />
        /// is specified, the test method is expected to not run.</param>
        protected void VerifySampleContract(string testPatternName, Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            RunFixtures(fixtureType);

            foreach (TestStepRun run in Report.TestPackageRun.AllTestStepRuns.Where(x =>
                x.Step.IsPrimary && x.Step.FullName.EndsWith(
                String.Format("/{0}/{1}/{2}/{3}", GetType().Name,
                fixtureType.Name, testPatternName, testMethodName))))
            {
                if (expectedTestStatus == TestStatus.Inconclusive)
                {
                    Assert.IsNull(run);
                }
                else
                {
                    Assert.IsNotNull(run);
                    Assert.AreEqual(expectedTestStatus, run.Result.Outcome.Status);
                }
            }
        }
    }
}
