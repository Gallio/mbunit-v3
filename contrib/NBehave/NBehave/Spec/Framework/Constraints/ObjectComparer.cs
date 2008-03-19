using System;

namespace NBehave.Spec.Framework.Constraints
{
    public class ObjectComparer : Comparer<object>
    {
        public ObjectComparer(object expectedValue, object actualValue)
            : base(expectedValue, actualValue)
        {
        }

        protected override bool Verify()
        {
            return CompareObjects(ExpectedValue, ActualValue);
        }

        private bool CompareObjects(object expectedObj, object actualObj)
        {
            if (expectedObj != null && actualObj != null)
            {
                if (expectedObj.GetType().IsArray && actualObj.GetType().IsArray)
                {
                    Array expectedArray = expectedObj as Array;
                    Array actualArray = actualObj as Array;

                    return CompareArrays(expectedArray, actualArray);
                }
                else
                {
                    return expectedObj.Equals(actualObj);
                }
            }

            return expectedObj == actualObj;
        }

        private bool CompareArrays(Array expectedArray, Array actualArray)
        {
            if (expectedArray.Length == actualArray.Length)
            {
                for (int i = 0; i < expectedArray.Length; i++)
                {
                    if (! CompareObjects(expectedArray.GetValue(i), actualArray.GetValue(i)))
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}