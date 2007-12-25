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

using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Build;
using JetBrains.ReSharper.Psi.Caches;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class PsiProjectWrapper : PsiModuleWrapper<IProject>
    {
        public PsiProjectWrapper(PsiReflector reflector, IProject target)
            : base(reflector, target)
        {
        }

        public override IProject Project
        {
            get { return Target; }
        }

        public override IList<AssemblyName> GetReferencedAssemblies()
        {
            ICollection<IModuleReference> moduleRefs = Target.GetModuleReferences();
            return GenericUtils.ConvertAllToArray<IModuleReference, AssemblyName>(moduleRefs, delegate(IModuleReference moduleRef)
            {
                return Reflector.Wrap(moduleRef.ResolveReferencedModule()).GetName();
            });
        }

        protected override IAssemblyFile GetAssemblyFile()
        {
            return BuildSettingsManager.GetInstance(Target).GetOutputAssemblyFile();
        }

        protected override IDeclarationsCache GetDeclarationsCache()
        {
            return PsiManager.GetDeclarationsCache(DeclarationsCacheScope.ProjectScope(Target, false), true);
        }
    }
}