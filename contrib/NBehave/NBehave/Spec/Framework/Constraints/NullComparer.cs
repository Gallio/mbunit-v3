using System;

namespace NBehave.Spec.Framework.Constraints
{
    public class NullComparer : Criteria<object>
    {
        public NullComparer(object actualValue)
            : base(actualValue)
        {
        }

        public override string Description
        {
            get { return "Value should be null."; }
        }

        protected override bool Verify()
        {
            return ActualValue == null;
        }
    }
}