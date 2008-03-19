using System;

namespace NBehave.Spec.Framework.Constraints
{
    public class ReferentialEqualityComparer : Comparer<object>
    {
        public ReferentialEqualityComparer(object expectedValue, object actualValue)
            : base(expectedValue, actualValue)
        {
        }

        public override string Description
        {
            get { return "Values should have the same referential identity."; }
        }

        protected override bool Verify()
        {
            return ReferenceEquals(ExpectedValue, ActualValue);
        }
    }
}