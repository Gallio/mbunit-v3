using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Model.Reflection;

using XunitReflector = Xunit.Sdk.Reflector;
using XunitTypeInfo = Xunit.Sdk.ITypeInfo;
using XunitMethodInfo = Xunit.Sdk.IMethodInfo;
using XunitAttributeInfo = Xunit.Sdk.IAttributeInfo;

namespace Gallio.Plugin.XunitAdapter.Model
{
    /// <summary>
    /// An adapter for converting <see cref="ITypeInfo" /> into <see cref="XunitTypeInfo" />.
    /// </summary>
    public class XunitTypeInfoAdapter : XunitTypeInfo
    {
        private readonly ITypeInfo target;

        public XunitTypeInfoAdapter(ITypeInfo target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
        }

        public ITypeInfo Target
        {
            get { return target; }
        }

        public IEnumerable<XunitMethodInfo> GetMethods()
        {
            foreach (IMethodInfo method in target.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                yield return new XunitMethodInfoAdapter(method);
        }

        public IEnumerable<XunitAttributeInfo> GetCustomAttributes(Type attributeType)
        {
            foreach (Attribute attribute in target.GetAttributes(attributeType, true))
                yield return XunitReflector.Wrap(attribute);
        }

        public bool HasAttribute(Type attributeType)
        {
            return target.HasAttribute(attributeType, true);
        }

        public bool HasInterface(Type interfaceType)
        {
            foreach (ITypeInfo @interface in target.GetInterfaces())
            {
                if (@interface.FullName == interfaceType.FullName)
                    return true;
            }

            return false;
        }

        public bool IsAbstract
        {
            get { return (target.Modifiers & TypeAttributes.Abstract) != 0; }
        }

        public Type Type
        {
            get { return target.Resolve(); }
        }

        public override string ToString()
        {
            return target.FullName;
        }
    }
}
