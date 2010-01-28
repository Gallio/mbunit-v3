// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Collections;

namespace Gallio.Common.Reflection.Impl
{
    ///<summary>
    /// Wrapper to represent a namespace.
    ///</summary>
    public sealed class NativeNamespaceWrapper : NativeWrapper, INamespaceInfo
    {
        private readonly string name;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the namespace.</param>
        public NativeNamespaceWrapper(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public CodeElementKind Kind
        {
            get { return CodeElementKind.Namespace; }
        }

        /// <inheritdoc />
        public CodeReference CodeReference
        {
            get { return CodeReference.CreateFromNamespace(name); }
        }

        /// <inheritdoc />
        public IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            return EmptyArray<IAttributeInfo>.Instance;
        }

        /// <inheritdoc />
        public bool HasAttribute(ITypeInfo attributeType, bool inherit)
        {
            return false;
        }

        /// <inheritdoc />
        public IEnumerable<object> GetAttributes(ITypeInfo attributeType, bool inherit)
        {
            return EmptyArray<object>.Instance;
        }

        /// <inheritdoc />
        public string GetXmlDocumentation()
        {
            return null;
        }

        /// <inheritdoc />
        public CodeLocation GetCodeLocation()
        {
            return CodeLocation.Unknown;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return name;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            NativeNamespaceWrapper other = obj as NativeNamespaceWrapper;
            return other != null && name == other.name;
        }

        /// <inheritdoc />
        public bool Equals(ICodeElementInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public bool Equals(INamespaceInfo other)
        {
            return Equals((object)other);
        }
    }
}