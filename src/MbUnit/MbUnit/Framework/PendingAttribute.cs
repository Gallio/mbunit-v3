using System;
using Gallio.Framework.Explorer;
using Gallio.Framework.Patterns;
using Gallio.Model;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Indicates that a test has pending prerequisites so it will not be run.
    /// The test will still appear in test reports along with an explanation of the
    /// reason it it pending, if provided.
    /// </para>
    /// <para>
    /// This attribute can be used to disable tests that cannot run because the
    /// subject under test is missing certain prerequisite functionality.  It may
    /// also serve as a placeholder for test that have yet to be implemented.
    /// </para>
    /// </summary>
    public class PendingAttribute : TestDecoratorPatternAttribute
    {
        private readonly string reason;

        /// <summary>
        /// Indicates that this test has pending prerequisites without providing a reason.
        /// </summary>
        public PendingAttribute()
            : this("")
        {
        }

        /// <summary>
        /// Indicates that this test has pending prerequisites and provides a reason.
        /// </summary>
        /// <param name="reason">The reason for which the test is pending</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reason"/>
        /// is null</exception>
        public PendingAttribute(string reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            this.reason = reason;
        }

        /// <summary>
        /// Gets the reason that the test is pending.
        /// </summary>
        public string Reason
        {
            get { return reason; }
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternTestBuilder builder, ICodeElementInfo codeElement)
        {
            builder.Test.Metadata.Add(MetadataKeys.PendingReason, reason);
        }
    }
}