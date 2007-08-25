using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A step is a delimited region of a test.  Each step appears in the report as
    /// if it were a dynamically generated test nested within the body of the test
    /// (or some other step) that spawned it.  The step has its own execution log,
    /// pass/fail/inconclusive result and in all other respects behaves much like
    /// an ordinary test would.
    /// </summary>
    public interface IStep
    {
        /// <summary>
        /// Gets the unique identifier of the step.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the name of the step.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the step that contains this one, or null if this instance represents the
        /// root step of a test.
        /// </summary>
        IStep Parent { get; }

        /// <summary>
        /// Gets the test that contains this step.
        /// </summary>
        ITest Test { get; }
    }
}
