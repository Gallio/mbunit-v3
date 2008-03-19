using System;

namespace NBehave.Spec.Framework.Constraints
{
    /// <summary>
    /// A constraint for double precision floating point values with a set tolerance.
    /// </summary>
    public class DoubleComparer : Comparer<double>
    {
        private double tolerance;

        /// <summary>
        /// Creates a comparison based constraint for double precision floating point values.
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        public DoubleComparer(double expectedValue, object actualValue)
            : base(expectedValue, actualValue)
        {
        }

        /// <inheritdoc />
        public override string Description
        {
            get { return String.Format("Values should be equal within a tolerance of {0}.", tolerance); }
        }

        /// <summary>
        /// Sets the tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance</param>
        public void WithAToleranceOf(double tolerance)
        {
            this.tolerance = tolerance;
        }

        /// <inheritdoc />
        protected override bool Verify()
        {
            if (base.Verify())
                return true;

            return IsActualValueOfCorrectType
                && Math.Abs(ExpectedValue - TypedActualValue) <= tolerance;
        }
    }
}