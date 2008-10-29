using System;
using Gallio;
using Gallio.Model;
using Gallio.Model.Diagnostics;

namespace MbUnit.Framework
{
    /// <summary>
    /// Describes a test case generated either at test exploration time or at test
    /// execution time by a test factory.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Refer to the examples on the <see cref="Test" /> class for more information.
    /// </para>
    /// </remarks>
    /// <seealso cref="Test"/>
    public class TestCase : TestDefinition
    {
        /// <summary>
        /// Creates a test case with a delegate to execute as its main body.
        /// </summary>
        /// <param name="name">The test case name</param>
        /// <param name="execute">The main body of the test case</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="execute"/>
        /// is null</exception>
        public TestCase(string name, Action execute)
            : base(name)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            Execute = execute;
        }

        /// <summary>
        /// Gets the delegate to run as the main body of the test case.
        /// </summary>
        public Action Execute { get; private set; }

        /// <inheritdoc />
        protected override bool IsTestCase
        {
            get { return true; }
        }

        /// <inheritdoc />
        protected override string Kind
        {
            get { return TestKinds.Test; }
        }

        /// <inheritdoc />
        [TestFrameworkInternal]
        protected override void OnExecuteSelf()
        {
            Execute();
        }
    }
}
