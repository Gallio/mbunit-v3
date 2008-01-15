using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataReturnValueWrapper : MetadataCodeElementWrapper<IMetadataReturnValue>, IParameterInfo
    {
        public MetadataReturnValueWrapper(MetadataReflector reflector, IMetadataReturnValue target)
            : base(reflector, target)
        {
        }

        protected override IDeclaredElement GetDeclaredElementWithLock()
        {
            return Reflector.GetDeclaredElementWithLock(Target);
        }

        public override string Name
        {
            get { return "<return>"; }
        }

        public override CodeReference CodeReference
        {
            get
            {
                CodeReference reference = Member.CodeReference;
                reference.ParameterName = Name;
                return reference;
            }
        }

        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(Target.Type); }
        }

        public int Position
        {
            get { return 0; }
        }

        public IMemberInfo Member
        {
            get { return Reflector.Wrap(Target.DeclaringMethod); }
        }

        public ParameterAttributes ParameterAttributes
        {
            get { return ParameterAttributes.Retval; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Parameter; }
        }

        public ParameterInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveParameter(this, throwOnError);
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(IParameterInfo other)
        {
            return Equals((object)other);
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
        {
            return ReflectorAttributeUtils.EnumerateParameterAttributes(this, inherit, delegate(IParameterInfo member)
            {
                return EnumerateAttributesForEntity(((MetadataReturnValueWrapper)member).Target);
            });
        }
    }
}