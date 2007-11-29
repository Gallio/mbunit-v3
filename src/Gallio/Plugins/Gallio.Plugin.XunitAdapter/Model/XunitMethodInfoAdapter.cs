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
    /// An adapter for converting <see cref="Gallio.Model.Reflection.ITypeInfo" /> into <see cref="XunitMethodInfo" />.
    /// </summary>
    public class XunitMethodInfoAdapter : XunitMethodInfo
    {
        private readonly IMethodInfo target;

        public XunitMethodInfoAdapter(IMethodInfo target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
        }

        public IMethodInfo Target
        {
            get { return target; }
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

        public string DeclaringTypeName
        {
            get { return target.DeclaringType.FullName; }
        }

        public bool IsAbstract
        {
            get { return (target.Modifiers & MethodAttributes.Abstract) != 0; }
        }

        public bool IsStatic
        {
            get { return (target.Modifiers & MethodAttributes.Static) != 0; }
        }

        public MethodInfo MethodInfo
        {
            get { return target.Resolve(); }
        }

        public string Name
        {
            get { return target.Name; }
        }

        public string ReturnType
        {
            get { return target.ReturnType.FullName; }
        }

        public override string ToString()
        {
            return target.Name;
        }
    }
}
