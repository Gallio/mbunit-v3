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
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    internal sealed class NativeNamespaceWrapper : INamespaceInfo
    {
        private readonly string name;

        public NativeNamespaceWrapper(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public CodeElementKind Kind
        {
            get { return CodeElementKind.Namespace; }
        }

        public CodeReference CodeReference
        {
            get { return CodeReference.CreateFromNamespace(name); }
        }

        public IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            return EmptyArray<IAttributeInfo>.Instance;
        }

        public bool HasAttribute(ITypeInfo attributeType, bool inherit)
        {
            return false;
        }

        public IEnumerable<object> GetAttributes(ITypeInfo attributeType, bool inherit)
        {
            return EmptyArray<object>.Instance;
        }

        public string GetXmlDocumentation()
        {
            return null;
        }

        public CodeLocation GetCodeLocation()
        {
            return null;
        }

        public override string ToString()
        {
            return name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            NativeNamespaceWrapper other = obj as NativeNamespaceWrapper;
            return other != null && name == other.name;
        }

        public bool Equals(ICodeElementInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(INamespaceInfo other)
        {
            return Equals((object)other);
        }
    }
}