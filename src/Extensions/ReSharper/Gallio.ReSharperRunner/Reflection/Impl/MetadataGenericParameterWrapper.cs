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
using System.Reflection;
using Gallio.Reflection;
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

        public override string Name
        {
            get { return Target.Argument.Name; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.GenericParameter; }
        }

        public override bool IsGenericParameter
        {
            get { return true; }
        }

        public override ITypeInfo EffectiveClassType
        {
            get
            {
                // TODO: In actuality we can treat this case as producing a type whose members
                //       are the union of the classes or interfaces in the generic type parameter constraint.
                throw new NotImplementedException("Cannot perform this operation on a generic type parameter.");
            }
        }

        public GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                // Note: The values are exactly the same, it's just the type that's different.
                return (GenericParameterAttributes)Target.Argument.Attributes;
            }
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
    }
}