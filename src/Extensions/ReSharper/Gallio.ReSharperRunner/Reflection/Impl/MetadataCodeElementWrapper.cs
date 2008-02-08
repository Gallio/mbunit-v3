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
using Gallio.Reflection;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Shell;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal abstract class MetadataCodeElementWrapper<TTarget> : MetadataWrapper<TTarget>, IReSharperCodeElementInfo
        where TTarget : class
    {
        public MetadataCodeElementWrapper(MetadataReflector reflector, TTarget target)
            : base(reflector, target)
        {
        }

        public IProject Project
        {
            get { return Reflector.ContextProject; }
        }

        public IDeclaredElement DeclaredElement
        {
            get
            {
                using (ReadLockCookie.Create())
                    return GetDeclaredElementWithLock(); 
            }
        }

        protected virtual IDeclaredElement GetDeclaredElementWithLock()
        {
            return null;
        }

        public abstract string Name { get; }

        public abstract CodeElementKind Kind { get; }

        public abstract CodeReference CodeReference { get; }

        public abstract IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit);

        public IEnumerable<IAttributeInfo> GetAttributeInfos(Type attributeType, bool inherit)
        {
            return AttributeUtils.FilterAttributesOfType(GetAttributeInfos(inherit), attributeType);
        }

        public bool HasAttribute(Type attributeType, bool inherit)
        {
            return AttributeUtils.ContainsAttributeOfType(GetAttributeInfos(inherit), attributeType);
        }

        public IEnumerable<object> GetAttributes(bool inherit)
        {
            return AttributeUtils.ResolveAttributes(GetAttributeInfos(inherit));
        }

        public IEnumerable<object> GetAttributes(Type attributeType, bool inherit)
        {
            return AttributeUtils.ResolveAttributesOfType(GetAttributeInfos(inherit), attributeType);
        }

        public string GetXmlDocumentation()
        {
            return null;
        }

        public virtual CodeLocation GetCodeLocation()
        {
            return null;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(ICodeElementInfo other)
        {
            return Equals((object)other);
        }

        protected IEnumerable<IAttributeInfo> EnumerateAttributesForEntity(IMetadataEntity entity)
        {
            foreach (IMetadataCustomAttribute attrib in entity.CustomAttributes)
                if (attrib.UsedConstructor != null) // Note: Can be null occasionally and R# itself will ignore it, why?
                    yield return Reflector.Wrap(attrib);
        }
    }
}