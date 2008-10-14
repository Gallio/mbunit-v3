using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides compatibility with MbUnit v2 rollback.
    /// </summary>
    [Obsolete("Use the MbUnit v3 [Rollback] attribute instead.  This attribute has been renamed to resolve incorrect casing and upgraded for .Net Framework 2.0.")]
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class RollBackAttribute : RollbackAttribute
    {
        /// <summary>
        /// Tags the test for rollback.
        /// </summary>
        public RollBackAttribute()
        {
        }

        /// <summary>
        /// Tags the test for rollback.
        /// </summary>
        /// <param name="timeout">Ignored.  The implementation uses the test's own timeout instead.</param>
        public RollBackAttribute(int timeout)
        {
        }
    }
}
