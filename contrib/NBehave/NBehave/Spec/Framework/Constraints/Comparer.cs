using System;

namespace NBehave.Spec.Framework.Constraints
{
    /// <summary>
    /// A specification constraint expressed as a comparison of an expected and actual value.
    /// </summary>
    /// <typeparam name="TExpected">The expected value type</typeparam>
    public class Comparer<TExpected> : Criteria<TExpected>
    {
        private readonly TExpected expectedValue;

        /// <summary>
        /// Creates a comparison based constraint.
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        public Comparer(TExpected expectedValue, object actualValue)
            : base(actualValue)
        {
            this.expectedValue = expectedValue;
        }

        /// <inheritdoc />
        public override string Description
        {
            get { return "Values should be equal."; }
        }

        /// <summary>
        /// Gets the expected value.
        /// </summary>
        protected TExpected ExpectedValue
        {
            get { return expectedValue; }
        }

        /// <inheritdoc />
        protected override bool Verify()
        {
            if (!IsActualValueOfCorrectType)
                return false;

            if (expectedValue == null)
                return ActualValue == null;

            return expectedValue.Equals(TypedActualValue);
        }

        /// <inheritdoc />
        protected override SpecificationResult EvaluateImpl()
        {
            if (Verify())
                return SpecificationResult.CreateSuccess();
            else
                return SpecificationResult.CreateComparisonFailure(expectedValue, ActualValue);
        }
    }
}