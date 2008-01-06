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
using Gallio.Hosting;

namespace Gallio.Reflection.Impl
{
    internal sealed class NativeAssemblyWrapper : NativeCodeElementWrapper<Assembly>, IAssemblyInfo
    {
        public NativeAssemblyWrapper(Assembly target)
            : base(target)
        {
        }

        public override string Name
        {
            get { return Target.GetName().Name; }
        }

        public override CodeReference CodeReference
        {
            get { return CodeReference.CreateFromAssembly(Target); }
        }

        public string Path
        {
            get { return Loader.GetFriendlyAssemblyCodeBase(Target); }
        }

        public string FullName
        {
            get { return Target.FullName; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Assembly; }
        }

        public AssemblyName GetName()
        {
            return Target.GetName();
        }

        public IList<AssemblyName> GetReferencedAssemblies()
        {
            return Target.GetReferencedAssemblies();
        }

        public IList<ITypeInfo> GetExportedTypes()
        {
            Type[] types = Target.GetExportedTypes();
            return Array.ConvertAll<Type, ITypeInfo>(types, Reflector.Wrap);
        }

        public IList<ITypeInfo> GetTypes()
        {
            Type[] types = Target.GetTypes();
            return Array.ConvertAll<Type, ITypeInfo>(types, Reflector.Wrap);
        }

        public ITypeInfo GetType(string typeName)
        {
            return Reflector.Wrap(Target.GetType(typeName));
        }

        public Assembly Resolve()
        {
            return Target;
        }

        public override string GetXmlDocumentation()
        {
            return null;
        }

        public override CodeLocation GetCodeLocation()
        {
            return new CodeLocation(Path, 0, 0);
        }

        public bool Equals(IAssemblyInfo other)
        {
            return Equals((object)other);
        }
    }
}