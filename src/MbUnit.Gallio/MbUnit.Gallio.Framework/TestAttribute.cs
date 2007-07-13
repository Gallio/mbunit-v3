using System;
using MbUnit.Framework.Model;
using MbUnit.Framework.Core.Attributes;
using MbUnit.Framework.Core.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares an MbUnit test method.
    /// Must appear within a test fixture class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TestAttribute : MethodPatternAttribute
    {
    }
}
