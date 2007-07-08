using System;
using MbUnit.Framework.Core.Attributes;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares an MbUnit test fixture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TestFixtureAttribute : FixturePatternAttribute
    {
    }
}
