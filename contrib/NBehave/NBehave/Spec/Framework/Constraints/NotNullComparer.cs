using System;

namespace NBehave.Spec.Framework.Constraints
{
    public class NotNullComparer : Criteria<object>
    {
        public NotNullComparer(object actualValue)
            : base(actualValue)
        {
        }

        public override string Description
        {
            get { return "Value should not be null."; }
        }

        protected override bool Verify()
        {
            return ActualValue != null;
        }
    }
}