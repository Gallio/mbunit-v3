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
