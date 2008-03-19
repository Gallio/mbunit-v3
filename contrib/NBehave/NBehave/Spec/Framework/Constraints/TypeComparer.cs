using System;

namespace NBehave.Spec.Framework.Constraints
{
    public class TypeComparer : Criteria<object>
    {
        private readonly Type expectedType;

        public TypeComparer(Type expectedType, object actualValue)
            : base(actualValue)
        {
            this.expectedType = expectedType;
        }

        public override string Description
        {
            get { return String.Format("Value's type should be {0}.", expectedType); }
        }

        protected override bool Verify()
        {
            if (ActualValue == null)
                return false;

            return expectedType.IsInstanceOfType(ActualValue);
        }
    }
}