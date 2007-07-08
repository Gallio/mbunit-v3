using System;
using MbUnit.Framework.Core.Attributes;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SetUpAttribute : TestPatternAttribute
    {
    }
}
