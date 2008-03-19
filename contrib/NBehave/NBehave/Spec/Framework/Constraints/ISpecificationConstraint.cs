using System;

namespace NBehave.Spec.Framework.Constraints
{
    /// <summary>
    /// A specified constraint represents a condition that must be satisfied in order to meet a specification.
    /// </summary>
    public interface ISpecificationConstraint
    {
        /// <summary>
        /// Gets a description of the constraint.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Evaluates the specification constraint and returns the result.
        /// </summary>
        /// <returns>The result of the evaluation</returns>
        SpecificationResult Evaluate();
    }
}
