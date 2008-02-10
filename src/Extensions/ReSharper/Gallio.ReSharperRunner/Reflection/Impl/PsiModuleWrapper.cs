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

using System.Collections.Generic;
using System.Reflection;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal abstract class PsiModuleWrapper<TTarget> : PsiCodeElementWrapper<TTarget>, IAssemblyInfo
        where TTarget : class, IModule
    {
        public PsiModuleWrapper(PsiReflector reflector, TTarget target)
            : base(reflector, target)
        {
        }

        public override string Name
        {
            get { return GetName().Name; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Assembly; }
        }

        public override CodeReference CodeReference
        {
            get { return new CodeReference(FullName, null, null, null, null); }
        }

        public string FullName
        {
            get { return GetName().FullName; }
        }

        public Assembly Resolve()
        {
            return Reflector.ResolveAssembly(this);
        }

        public string Path
        {
            get { return GetAssemblyFile().Location.FullPath; }
        }

        public override CodeLocation GetCodeLocation()
        {
            return new CodeLocation(Path, 0, 0);
        }

        public virtual AssemblyName GetName()
        {
            return GetAssemblyFile().AssemblyName;
        }

        public ITypeInfo GetType(string typeName)
        {
            return Reflector.Wrap(GetDeclarationsCache().GetTypeElementByCLRName(typeName));
        }

        public IList<ITypeInfo> GetExportedTypes()
        {
            return GetTypes(false);
        }

        public IList<ITypeInfo> GetTypes()
        {
            return GetTypes(true);
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            return ReflectorAttributeUtils.EnumerateAssemblyAttributes(this, attributeType, inherit, delegate(IAssemblyInfo member)
            {
                return EnumerateAttributesForModule(PsiManager.GetModuleAttributes(Target));
            });
        }

        private IList<ITypeInfo> GetTypes(bool includeNonPublicTypes)
        {
            INamespace ns = PsiManager.GetNamespace("");
            IDeclarationsCache cache = GetDeclarationsCache();

            List<ITypeInfo> types = new List<ITypeInfo>();
            PopulateTypes(types, ns, cache, includeNonPublicTypes);

            return types;
        }

        private void PopulateTypes(IList<ITypeInfo> types, INamespace ns, IDeclarationsCache cache, bool includeNonPublicTypes)
        {
            foreach (IDeclaredElement element in ns.GetNestedElements(cache))
            {
                ITypeElement type = element as ITypeElement;
                if (type != null)
                {
                    PopulateTypes(types, type, includeNonPublicTypes);
                }
                else
                {
                    INamespace nestedNamespace = element as INamespace;
                    if (nestedNamespace != null)
                        PopulateTypes(types, nestedNamespace, cache, includeNonPublicTypes);
                }
            }
        }

        private void PopulateTypes(IList<ITypeInfo> types, ITypeElement type, bool includeNonPublicTypes)
        {
            IModifiersOwner modifiers = type as IModifiersOwner;
            if (modifiers != null && (includeNonPublicTypes || modifiers.GetAccessRights() == AccessRights.PUBLIC))
            {
                types.Add(Reflector.Wrap(type));

                foreach (ITypeElement nestedType in type.NestedTypes)
                    PopulateTypes(types, nestedType, includeNonPublicTypes);
            }
        }

        public abstract IList<AssemblyName> GetReferencedAssemblies();

        protected abstract IAssemblyFile GetAssemblyFile();

        protected abstract IDeclarationsCache GetDeclarationsCache();

        protected PsiManager PsiManager
        {
            get { return PsiManager.GetInstance(Target.GetSolution()); }
        }

        public bool Equals(IAssemblyInfo other)
        {
            return Equals((object)other);
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}