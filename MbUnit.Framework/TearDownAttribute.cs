namespace MbUnit.Framework
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TearDownAttribute : Attribute { }
}
