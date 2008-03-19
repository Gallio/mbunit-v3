using System;

namespace NBehave.Spec.Framework.Constraints
{
    public class ReferentialInequalityComparer : Comparer<object>
    {
        public ReferentialInequalityComparer(object expectedValue, object actualValue)
            : base(expectedValue, actualValue)
        {
        }

        public override string Description
        {
            get { return "Values should not have the same referential identity."; }
        }

        protected override bool Verify()
        {
            return !ReferenceEquals(ExpectedValue, ActualValue);
        }
    }
}