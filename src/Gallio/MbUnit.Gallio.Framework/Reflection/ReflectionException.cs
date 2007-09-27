using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Reflection
{
    public class ReflectionException : ApplicationException
    {
        public ReflectionException(string message)
            : base(message)
        {
        }

        public ReflectionException(string memberName, MemberType type, object obj) 
            : this(string.Format("Fail to find {0} {1} in {2}."
                , memberName
                , Enum.GetName(typeof(MemberType), type)
                , obj.ToString()))
        {
        }
    }
}
