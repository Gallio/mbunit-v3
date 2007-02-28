using System;
using MbUnit.Framework;
using TestFu.Operations;
using System.Reflection;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Parameter,AllowMultiple =true,Inherited =true)]
    public sealed class UsingEnumAttribute : UsingBaseAttribute
    {
        private Type enumType;

        public UsingEnumAttribute(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("emumType");
            if (!enumType.IsEnum)
                throw new ArgumentException("Type "+enumType.FullName+" is not a enum");
            this.enumType = enumType;
        }

        public Type EnumType
        {
            get { return this.enumType; }
        }

        public override void GetDomains(
            IDomainCollection domains, 
            ParameterInfo parameter, 
            object fixture)
        {
            ArrayDomain domain = new ArrayDomain(Enum.GetValues(this.EnumType));
            domains.Add(domain);
        }
    }
}
