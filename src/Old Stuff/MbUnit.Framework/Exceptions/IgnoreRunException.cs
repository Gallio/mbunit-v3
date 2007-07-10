namespace MbUnit.Framework
{
    using System;
    using TestDriven.UnitTesting.Exceptions;
    //using TestDriven.UnitTesting.Compatibility;

    //[CompatibleException("MbUnit.Core.Exceptions.IgnoreRunException", "MbUnit.Framework")]
    public class IgnoreRunException : AssertIgnoreExceptionBase
    {
        public IgnoreRunException(string message)
            : base(message)
        {
        }
    }
}
