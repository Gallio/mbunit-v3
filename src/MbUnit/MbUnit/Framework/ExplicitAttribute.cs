using System;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Indicates that a test should only be run explicitly.
    /// The test will still appear in the test results but it will not run unless it is explicitly
    /// selected for execution.
    /// </para>
    /// <para>
    /// A test is considered to be explicitly selected when the filter used to run the tests
    /// matches the test or its descendants but none of its ancestors.  For example, if the filter
    /// matches a test case but not its containing test fixture then the test case will be deemed
    /// to be explicitly selected.  Otherwise the test case will be implicitly selected by virtue
    /// of the fact that the filter matched one of its ancestors.
    /// </para>
    /// <para>
    /// This attribute can be used to exclude from normal execution any tests that are
    /// particularly expensive or require manual supervision by an operator.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method,
        AllowMultiple = false, Inherited = true)]
    public class ExplicitAttribute : TestDecoratorPatternAttribute
    {
        private readonly string reason;

        /// <summary>
        /// Indicates that this test should only run explicitly without providing a reason.
        /// </summary>
        public ExplicitAttribute()
            : this("")
        {
        }

        /// <summary>
        /// Indicates that this test should only run explicitly and provides a reason.
        /// </summary>
        /// <param name="reason">The reason for which the test should be run explicitly</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reason"/>
        /// is null</exception>
        public ExplicitAttribute(string reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            this.reason = reason;
        }

        /// <summary>
        /// Gets the reason that the test should only run explicitly.
        /// </summary>
        public string Reason
        {
            get { return reason; }
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternTestBuilder builder, ICodeElementInfo codeElement)
        {
            builder.Test.Metadata.Add(MetadataKeys.ExplicitReason, reason);

            builder.Test.TestActions.BeforeTestChain.Before(delegate(PatternTestState state)
            {
                if (!state.IsExplicit)
                {
                    string message = "The test will not run unless explicitly selected.";
                    if (reason.Length != 0)
                        message += "\nReason: " + reason;

                    throw new SilentTestException(TestOutcome.Explicit, message);
                }
            });
        }
    }
}