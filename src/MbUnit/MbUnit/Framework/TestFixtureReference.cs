using System;
using Gallio;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Describes a reference to another test fixture.  This is used
    /// to enable test suites to include tests that are defined using attributes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Refer to the examples on the <see cref="Test" /> class for more information.
    /// </para>
    /// <para>
    /// Running referenced test fixtures dynamically is not supported at this time.  Run it
    /// statically instead using <see cref="StaticTestFactoryAttribute" />.
    /// </para>
    /// </remarks>
    /// <seealso cref="Test"/>
    public class TestFixtureReference : Test
    {
        private readonly Type testFixtureType;

        /// <summary>
        /// Creates a reference to a test fixture.
        /// </summary>
        /// <param name="testFixtureType">The test fixture type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testFixtureType"/> is null</exception>
        public TestFixtureReference(Type testFixtureType)
        {
            if (testFixtureType == null)
                throw new ArgumentNullException("testFixtureType");

            this.testFixtureType = testFixtureType;
        }

        /// <summary>
        /// Gets the referenced test fixture type, or null if the reference is on a
        /// test method of the containing test fixture.
        /// </summary>
        public Type TestFixtureType
        {
            get { return testFixtureType; }
        }

        /// <inheritdoc />
        protected override void BuildStaticTest(PatternEvaluationScope containingScope, ICodeElementInfo declaringCodeElement)
        {
            containingScope.Evaluator.Consume(containingScope, Reflector.Wrap(testFixtureType), false, TestTypePatternAttribute.AutomaticInstance);
        }

        /// <inheritdoc />
        protected override TestOutcome RunDynamicTest(ICodeElementInfo declaringCodeElement, Action setUp, Action tearDown)
        {
            throw new NotSupportedException("MbUnit does not currently support running a referenced test dynamically.");
        }
    }
}
