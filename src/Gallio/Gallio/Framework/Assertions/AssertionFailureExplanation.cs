namespace Gallio.Framework.Assertions
{
    /// <summary>
    /// A delegate for the <see cref="AssertionHelper.Explain" /> decorator method which 
    /// combines the specified inner failures into a single outer failure with a common explanation.
    /// </summary>
    /// <param name="innerFailures">The inner failures to combine together.</param>
    /// <returns>The composite assertion failure.</returns>
    public delegate AssertionFailure AssertionFailureExplanation(AssertionFailure[] innerFailures);
}