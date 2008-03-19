using System;
using System.Diagnostics;

namespace NBehave.Spec.Framework.Constraints
{
    /// <summary>
    /// An abstract base class of specification constraints expressed as a criteria
    /// over an actual value.
    /// </summary>
    /// <typeparam name="TExpected">The expected value type</typeparam>
    public abstract class Criteria<TExpected> : ISpecificationConstraint
    {
        private readonly object actualValue;

        /// <summary>
        /// Creates a criteria.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        protected Criteria(object actualValue)
        {
            this.actualValue = actualValue;
        }

        /// <inheritdoc />
        public abstract string Description { get; }

        /// <inheritdoc />
        public SpecificationResult Evaluate()
        {
            try
            {
                return EvaluateImpl();
            }
            catch (Exception ex)
            {
                return SpecificationResult.CreateExceptionFailure(ex);
            }
        }

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        protected object ActualValue
        {
            get { return actualValue; }
        }

        /// <summary>
        /// Gets the actual value cast to the expected type.
        /// </summary>
        protected TExpected TypedActualValue
        {
            get { return (TExpected)actualValue; }
        }

        /// <summary>
        /// Returns true if the actual value is of the correct type.
        /// </summary>
        protected virtual bool IsActualValueOfCorrectType
        {
            get { return actualValue is TExpected; }
        }

        /// <summary>
        /// <para>
        /// Internal implementation of <see cref="Evaluate" />.
        /// </para>
        /// <para>
        /// Any exceptions thrown by this method are interpreted as evaluation failures.
        /// </para>
        /// </summary>
        /// <returns>The specification result</returns>
        protected virtual SpecificationResult EvaluateImpl()
        {
            if (Verify())
                return SpecificationResult.CreateSuccess();
            else
                return SpecificationResult.CreateCriteriaFailure(actualValue);
        }

        /// <summary>
        /// Returns true if the constraint is satisfied.
        /// </summary>
        /// <returns>True if the constraint is satisfied</returns>
        protected abstract bool Verify();
    }
}