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
using Gallio.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal abstract class PsiCodeElementWrapper<TTarget> : PsiWrapper<TTarget>, IReSharperCodeElementInfo
        where TTarget : class
    {
        public PsiCodeElementWrapper(PsiReflector reflector, TTarget target)
            : base(reflector, target)
        {
        }

        public virtual IProject Project
        {
            get
            {
                IDeclaredElement declaredElement = DeclaredElement;
                return declaredElement != null ? declaredElement.Module as IProject : null;
            }
        }

        public virtual IDeclaredElement DeclaredElement
        {
            get { return null; }
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

        public virtual SourceLocation GetSourceLocation()
        {
            return null;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(ICodeElementInfo other)
        {
            return Equals((object) other);
        }

        protected IEnumerable<IAttributeInfo> EnumerateAttributesForModule(IModuleAttributes moduleAttributes)
        {
            foreach (IAttributeInstance attrib in moduleAttributes.AttributeInstances)
                yield return Reflector.Wrap(attrib);
        }

        protected IEnumerable<IAttributeInfo> EnumerateAttributesForElement(IAttributesOwner element, bool inherit)
        {
            foreach (IAttributeInstance attrib in element.GetAttributeInstances(inherit))
                yield return Reflector.Wrap(attrib);
        }
    }
}