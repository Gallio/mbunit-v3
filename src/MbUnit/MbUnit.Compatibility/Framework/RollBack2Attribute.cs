using System;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides compatibility with MbUnit v2 rollback.
    /// </summary>
    [Obsolete("Use the MbUnit v3 [Rollback] attribute instead.  This attribute has been renamed to resolve incorrect casing.")]
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class RollBack2Attribute : RollbackAttribute
    {
        /// <summary>
        /// Tags the test for rollback.
        /// </summary>
        public RollBack2Attribute()
        {
        }

        /// <summary>
        /// Tags the test for rollback.
        /// </summary>
        /// <param name="timeout">Ignored.  The implementation uses the test's own timeout instead.</param>
        public RollBack2Attribute(TimeSpan timeout)
        {
        }
    }
}