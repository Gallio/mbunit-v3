using System;

namespace MbUnit.Core.Services.Assert
{
    /// <summary>
    /// A delegate used to evaluate an assertion condition.
    /// Returns the assertion result.  If an exception occurs, the framework
    /// will interpret it as a fatal assertion failure and generate a suitable
    /// result object containing the exception.  Even though it is tolerated,
    /// an assertion condition should generally not fail with an exception.
    /// </summary>
    /// <param name="assertion">The assertion being checked.</param>
    /// <returns>The result of having evaluated the assertion condition, never null.
    /// Upon return, the framework will automatically set the <see cref="AssertionResult.Assertion" />
    /// property of the result.</returns>
    /// <exception cref="Exception">Any exception thrown is interpreted as an assertion failure.</exception>
    public delegate AssertionResult AssertionCondition(Assertion assertion);
}
