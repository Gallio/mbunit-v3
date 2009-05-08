// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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

namespace Gallio.Common.Reflection.Impl
{
    internal abstract class NativeCodeElementWrapper<TTarget> : ICodeElementInfo
        where TTarget : class, ICustomAttributeProvider
    {
        private readonly TTarget target;

        protected NativeCodeElementWrapper(TTarget target)
        {
            if (target == null)
                throw new ArgumentNullException(@"target");

            this.target = target;
        }

        public TTarget Target
        {
            get { return target; }
        }

        public abstract string Name { get; }

        public abstract CodeElementKind Kind { get; }

        public abstract CodeReference CodeReference { get; }

        public IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            foreach (Attribute attrib in GetAttributes(attributeType, inherit))
                yield return Reflector.Wrap(attrib);
        }

        public bool HasAttribute(ITypeInfo attributeType, bool inherit)
        {
            return Target.IsDefined(attributeType != null ? attributeType.Resolve(true) : typeof(object), inherit);
        }

        public IEnumerable<object> GetAttributes(ITypeInfo attributeType, bool inherit)
        {
            return Target.GetCustomAttributes(attributeType != null ? attributeType.Resolve(true) : typeof(object), inherit);
        }

        public abstract string GetXmlDocumentation();

        public virtual CodeLocation GetCodeLocation()
        {
            return CodeLocation.Unknown;
        }

        public override string ToString()
        {
            return Target.ToString();
        }

        public override bool Equals(object obj)
        {
            NativeCodeElementWrapper<TTarget> other = obj as NativeCodeElementWrapper<TTarget>;
            return other != null && target.Equals(other.target);
        }

        public override int GetHashCode()
        {
            return target.GetHashCode();
        }

        public bool Equals(ICodeElementInfo other)
        {
            return Equals((object) other);
        }
    }
}