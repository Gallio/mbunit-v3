// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Reflection;
using JetBrains.Metadata.Reader.API;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataArrayTypeWrapper : MetadataConstructedTypeWrapper<IMetadataArrayType>
    {
        public MetadataArrayTypeWrapper(MetadataReflector reflector, IMetadataArrayType target)
            : base(reflector, target)
        {
        }

        public override string Name
        {
            get { return ElementType.Name + "[]"; }
        }

        public override ITypeInfo ElementType
        {
            get { return Reflector.Wrap(Target.ElementType); }
        }

        public override int ArrayRank
        {
            get { return checked((int) Target.Rank); }
        }

        public override bool IsArray
        {
            get { return true; }
        }

        public override ITypeInfo EffectiveClassType
        {
            get { return Gallio.Reflection.Reflector.Wrap(typeof(Array)); }
        }
    }
}