using Gallio.Common;

namespace Gallio.Tests.Common
{
    public abstract class MockableConditionContext : ConditionContext
    {
        public abstract bool HasPropertyImplMock(string @namespace, string identifier);

        sealed protected override bool HasPropertyImpl(string @namespace, string identifier)
        {
            return HasPropertyImplMock(@namespace, identifier);
        }
    }
}