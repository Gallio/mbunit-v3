using System;
using System.Diagnostics;

namespace NBehave.Spec.Framework.Constraints
{
    /// <summary>
    /// Builds a constraint given an actual value and a condition.
    /// </summary>
    public class SpecificationConstraintBuilder : ISpecificationConstraint
    {
        private readonly object actualValue;
        private ISpecificationConstraint constraint;
        private StackTrace stackTrace;

        public SpecificationConstraintBuilder(object actualValue)
        {
            this.actualValue = actualValue;
        }

        public Comparer<bool> ShouldBeFalse()
        {
            return ShouldEvaluateToSuccessfulResult(new Comparer<bool>(false, actualValue));
        }

        public Comparer<bool> ShouldBeTrue()
        {
            return ShouldEvaluateToSuccessfulResult(new Comparer<bool>(true, actualValue));
        }

        public ObjectComparer ShouldEqual(object expectedValue)
        {
            return ShouldEvaluateToSuccessfulResult(new ObjectComparer(expectedValue, actualValue));
        }

        public DoubleComparer ShouldEqual(double expectedValue)
        {
            return ShouldEvaluateToSuccessfulResult(new DoubleComparer(expectedValue, actualValue));
        }

        public Comparer<int> ShouldEqual(int expectedValue)
        {
            return ShouldEvaluateToSuccessfulResult(new Comparer<int>(expectedValue, actualValue));
        }

        public NullComparer ShouldBeNull()
        {
            return ShouldEvaluateToSuccessfulResult(new NullComparer(actualValue));
        }

        public NotNullComparer ShouldNotBeNull()
        {
            return ShouldEvaluateToSuccessfulResult(new NotNullComparer(actualValue));
        }

        public ReferentialEqualityComparer ShouldBeTheSameAs(object expectedValue)
        {
            return ShouldEvaluateToSuccessfulResult(new ReferentialEqualityComparer(expectedValue, actualValue));
        }

        public ReferentialInequalityComparer ShouldNotBeTheSameAs(object expectedValue)
        {
            return ShouldEvaluateToSuccessfulResult(new ReferentialInequalityComparer(expectedValue, actualValue));
        }

        public TypeComparer ShouldBeOfType(Type type)
        {
            return ShouldEvaluateToSuccessfulResult(new TypeComparer(type, actualValue));
        }

        internal void SetStackTrace(StackTrace stackTrace)
        {
            this.stackTrace = stackTrace;
        }

        string ISpecificationConstraint.Description
        {
            get { return constraint != null ? constraint.Description : "Missing constraint"; }
        }

        SpecificationResult ISpecificationConstraint.Evaluate()
        {
            SpecificationResult result = constraint != null
                ? constraint.Evaluate()
                : SpecificationResult.CreateFailure("No constraint was specified for the actual value given to the specification constraint builder.");

            if (stackTrace != null)
                result.StackTrace = stackTrace;

            return result;
        }

        private T ShouldEvaluateToSuccessfulResult<T>(T constraint)
            where T : ISpecificationConstraint
        {
            if (this.constraint != null)
                throw new InvalidOperationException("A constraint was already specified for the actual value.");

            this.constraint = constraint;
            return constraint;
        }

        #region Prevent accidental usage of these members
        /// <summary>
        /// Do not use.
        /// </summary>
        public static new void Equals(object a, object b)
        {
            throw new NotSupportedException("Use Specify.That(x).ShouldEqual(y) instead.");
        }

        /// <summary>
        /// Do not use.
        /// </summary>
        public static new void ReferenceEquals(object a, object b)
        {
            throw new NotSupportedException("Use Specify.That(x).ShouldBeTheSameAs(y) instead.");
        }

        /// <summary>
        /// Do not use.
        /// </summary>
        public override bool Equals(object dummy)
        {
            throw new NotSupportedException("Use Specify.That(x).ShouldEqual(y) instead.");
        }

        /// <summary>
        /// Do not use.
        /// </summary>
        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
