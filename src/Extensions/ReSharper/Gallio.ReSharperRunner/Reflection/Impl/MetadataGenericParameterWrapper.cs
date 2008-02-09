// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.Metadata.Reader.API;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataGenericParameterWrapper : MetadataConstructedTypeWrapper<IMetadataGenericArgumentReferenceType>, IGenericParameterInfo
    {
        public MetadataGenericParameterWrapper(MetadataReflector reflector, IMetadataGenericArgumentReferenceType target)
            : base(reflector, target)
        {
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.GenericParameter; }
        }

        public override ITypeInfo BaseType
        {
            get { return Gallio.Reflection.Reflector.Wrap(typeof(Object)); }
        }

        public override ITypeInfo EffectiveClassType
        {
            get { return Gallio.Reflection.Reflector.Wrap(typeof(Object)); }
        }

        public override bool IsGenericParameter
        {
            get { return true; }
        }

        public override bool ContainsGenericParameters
        {
            get { return true; }
        }

        public GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                // Note: The values are defined in exactly the same way, it's just the type that's different.
                return (GenericParameterAttributes)Target.Argument.Attributes;
            }
        }

        public override IAssemblyInfo Assembly
        {
            get { return GetUltimateDeclaringType().Assembly; }
        }

        public override INamespaceInfo Namespace
        {
            get { return GetUltimateDeclaringType().Namespace; }
        }

        public override ITypeInfo DeclaringType
        {
            get { return Reflector.WrapOpenType(Target.Argument.TypeOwner); }
        }

        public IMethodInfo DeclaringMethod
        {
            get { return Reflector.WrapMethod(Target.Argument.MethodOwner); }
        }

        public ITypeInfo[] Constraints
        {
            get { return Array.ConvertAll<IMetadataType, ITypeInfo>(Target.Argument.TypeConstraints, Reflector.Wrap); }
        }

        public ITypeInfo ValueType
        {
            get { return Gallio.Reflection.Reflector.Wrap(typeof(Type)); }
        }

        public int Position
        {
            get { return (int) Target.Argument.Index; }
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
        {
            return EmptyArray<IConstructorInfo>.Instance;
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            return ReflectorAttributeUtils.EnumerateGenericParameterAttributes(this, attributeType, inherit, delegate(IGenericParameterInfo genericParameter)
            {
                return EnumerateAttributesForEntity(((MetadataGenericParameterWrapper)genericParameter).Target.Argument);
            });
        }

        private ITypeInfo GetUltimateDeclaringType()
        {
            return DeclaringType ?? DeclaringMethod.DeclaringType;
        }

        protected override string SimpleName
        {
            get { return Target.Argument.Name; }
        }
    }
}