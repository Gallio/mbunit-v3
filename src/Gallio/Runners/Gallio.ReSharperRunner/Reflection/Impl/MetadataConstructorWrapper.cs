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
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using JetBrains.Metadata.Reader.API;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataConstructorWrapper : MetadataFunctionWrapper, IConstructorInfo
    {
        public MetadataConstructorWrapper(MetadataReflector reflector, IMetadataMethod target)
            : base(reflector, target)
        {
            if (!MetadataReflector.IsConstructor(target))
                throw new ArgumentException("target");
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Constructor; }
        }

        public override MethodBase ResolveMethodBase(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        public ConstructorInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveConstructor(this, throwOnError);
        }

        public bool Equals(IConstructorInfo other)
        {
            return Equals((object)other);
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
        {
            return ReflectorAttributeUtils.EnumerateConstructorAttributes(this, inherit, delegate(IConstructorInfo member)
            {
                return EnumerateAttributesForEntity(((MetadataConstructorWrapper)member).Target);
            });
        }
    }
}