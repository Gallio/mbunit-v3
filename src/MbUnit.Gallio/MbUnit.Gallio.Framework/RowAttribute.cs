using System;
using MbUnit.Framework.Core.Attributes;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class RowAttribute : DataPatternAttribute
    {
    }
}
