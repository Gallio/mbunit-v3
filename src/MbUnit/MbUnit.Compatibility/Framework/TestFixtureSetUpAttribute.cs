using System;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides compatibility with MbUnit v2 test fixture set up.
    /// </summary>
    [Obsolete("Use the MbUnit v3 [FixtureSetUp] attribute instead.  This attribute has been renamed to be more general purpose.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestFixtureSetUpAttribute : FixtureSetUpAttribute
    {
    }
}
