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
using System.Text;
using Gallio.Common.Reflection;
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Collections;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// A fake type info that represents an MbUnitCpp test repository.
    /// </summary>
    internal class UnmanagedAssemblyInfo : IAssemblyInfo
    {
        private readonly string fileName;
        private Memoizer<string> assemblyPathMemoizer = new Memoizer<string>();
        private Memoizer<string> assemblyNameMemoizer = new Memoizer<string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the MbUnitCpp test repository file.</param>
        public UnmanagedAssemblyInfo(string fileName)
        {
            this.fileName = fileName;
        }

        /// <inheritdoc />
        public string Path
        {
            get { return assemblyPathMemoizer.Memoize(() => System.IO.Path.GetFullPath(fileName)); }
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return assemblyNameMemoizer.Memoize(() => System.IO.Path.GetFileNameWithoutExtension(fileName)); }
        }

        /// <inheritdoc />
        public AssemblyName GetName()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IList<AssemblyName> GetReferencedAssemblies()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IList<ITypeInfo> GetExportedTypes()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IList<ITypeInfo> GetTypes()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ITypeInfo GetType(string typeName)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Assembly Resolve(bool throwOnError)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public string Name
        {
            get { return FullName; }
        }

        /// <inheritdoc />
        public CodeElementKind Kind
        {
            get { return CodeElementKind.Assembly; }
        }

        /// <inheritdoc />
        public CodeReference CodeReference
        {
            get { return new CodeReference(FullName, null, null, null, null); }
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
            return String.Empty;
        }

        /// <inheritdoc />
        public CodeLocation GetCodeLocation()
        {
            return new CodeLocation(Path, 0, 0);
        }

        /// <inheritdoc />
        public IReflectionPolicy ReflectionPolicy
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool Equals(ICodeElementInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public bool Equals(IAssemblyInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return FullName;
        }
    }
}