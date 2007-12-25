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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using JetBrains.Metadata.Access;
using JetBrains.Metadata.Reader.API;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataAssemblyWrapper : MetadataCodeElementWrapper<IMetadataAssembly>, IAssemblyInfo
    {
        public MetadataAssemblyWrapper(MetadataReflector reflector, IMetadataAssembly target)
            : base(reflector, target)
        {
        }

        public override string Name
        {
            get { return Target.AssemblyName.Name; }
        }

        public override CodeReference CodeReference
        {
            get { return new CodeReference(FullName, null, null, null, null); }
        }

        public string Path
        {
            get { return Target.Location; }
        }

        public string FullName
        {
            get { return Target.AssemblyName.FullName; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Assembly; }
        }

        public AssemblyName GetName()
        {
            return Target.AssemblyName;
        }

        public IList<AssemblyName> GetReferencedAssemblies()
        {
            AssemblyReference[] references = Target.ReferencedAssembliesNames;
            return Array.ConvertAll<AssemblyReference, AssemblyName>(references, delegate(AssemblyReference reference)
            {
                return reference.AssemblyName;
            });
        }

        public IList<ITypeInfo> GetExportedTypes()
        {
            List<ITypeInfo> types = new List<ITypeInfo>();

            foreach (IMetadataTypeInfo type in Target.GetTypes())
            {
                if (type.IsPublic || type.IsNestedPublic)
                    types.Add(Reflector.WrapOpenType(type));
            }

            return types;
        }

        public IList<ITypeInfo> GetTypes()
        {
            IMetadataTypeInfo[] types = Target.GetTypes();
            return Array.ConvertAll<IMetadataTypeInfo, ITypeInfo>(types, Reflector.WrapOpenType);
        }

        public ITypeInfo GetType(string typeName)
        {
            return Reflector.Wrap(Target.GetTypeFromQualifiedName(typeName, false));
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
        {
            return ReflectorAttributeUtils.EnumerateAssemblyAttributes(this, inherit, delegate(IAssemblyInfo member)
            {
                return EnumerateAttributesForEntity(((MetadataAssemblyWrapper)member).Target);
            });
        }

        public Assembly Resolve()
        {
            return Reflector.ResolveAssembly(this);
        }

        public bool Equals(IAssemblyInfo other)
        {
            return Equals((object)other);
        }
    }
}