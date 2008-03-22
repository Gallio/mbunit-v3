using System;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using NBehave.Spec.Framework;

namespace NBehave.Specs.Spec.Framework.Constraints
{
    [Context]
    [Concering("Constraints")]
    public class Int32ComparisonSpec : SpecWithSample<Int32ComparisonSpecSample>
    {
        [Specification]
        public void ShouldPassWhenComparingOnes()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(Int32ComparisonSpecSample).GetMethod("CompareOnes")));
            Specify.That(run.Result.Outcome).ShouldEqual(TestOutcome.Passed);
        }

        [Specification]
        public void ShouldPassWhenComparingMaxValues()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(Int32ComparisonSpecSample).GetMethod("CompareMaxValues")));
            Specify.That(run.Result.Outcome).ShouldEqual(TestOutcome.Passed);
        }

        [Specification]
        public void ShouldPassWhenComparingMinValues()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(Int32ComparisonSpecSample).GetMethod("CompareMinValues")));
            Specify.That(run.Result.Outcome).ShouldEqual(TestOutcome.Passed);
        }

        [Specification]
        public void ShouldFailWhenComparingOneWithTwo()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(Int32ComparisonSpecSample).GetMethod("CompareOneWithTwo")));
            Specify.That(run.Result.Outcome).ShouldEqual(TestOutcome.Failed);
        }
    }

    [Explicit, Context]
    public class Int32ComparisonSpecSample
    {
        [Specification]
        public void CompareOnes()
        {
            Specify.That(1).ShouldEqual(1);
        }

        [Specification]
        public void CompareMaxValues()
        {
            Specify.That(int.MaxValue).ShouldEqual(int.MaxValue);
        }

        [Specification]
        public void CompareMinValues()
        {
            Specify.That(int.MinValue).ShouldEqual(int.MinValue);
        }

        [Specification]
        public void CompareOneWithTwo()
        {
            Specify.That(1).ShouldEqual(2);
        }
    }
}