using System;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides compatibility with MbUnit v2 test fixture tear down.
    /// </summary>
    [Obsolete("Use the MbUnit v3 [FixtureTearDown] attribute instead.  This attribute has been renamed to be more general purpose.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestFixtureTearDownAttribute : FixtureTearDownAttribute
    {
    }
}
