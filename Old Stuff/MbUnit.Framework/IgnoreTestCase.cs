namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using TestDriven.UnitTesting.Exceptions;
    using TestDriven.UnitTesting;

    class IgnoreTestCase : TestCaseBase
    {
        string name;
        string reason;

        public IgnoreTestCase(string fixtureName, string name, string reason)
            : base(fixtureName, name)
        {
            this.name = name;
            this.reason = reason;
        }

        public override void Run(object fixtureInstance)
        {
            throw new IgnoreRunException(this.reason);
        }
    }
}
