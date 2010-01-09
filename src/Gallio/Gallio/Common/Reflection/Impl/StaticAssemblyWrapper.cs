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
using System.Collections.ObjectModel;
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Common;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> assembly wrapper.
    /// </summary>
    public sealed class StaticAssemblyWrapper : StaticCodeElementWrapper, IAssemblyInfo
    {
        private Memoizer<AssemblyName> assemblyNameMemoizer = new Memoizer<AssemblyName>();
        private Memoizer<string> assemblyPathMemoizer = new Memoizer<string>();
        private Memoizer<IList<AssemblyName>> referencedAssembliesMemoizer = new Memoizer<IList<AssemblyName>>();
        private Memoizer<IList<ITypeInfo>> exportedTypesMemoizer = new Memoizer<IList<ITypeInfo>>();
        private Memoizer<IList<ITypeInfo>> typesMemoizer = new Memoizer<IList<ITypeInfo>>();
        private KeyedMemoizer<string, ITypeInfo> getTypeMemoizer = new KeyedMemoizer<string, ITypeInfo>();
        private KeyedMemoizer<bool, Assembly> resolveMemoizer = new KeyedMemoizer<bool, Assembly>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy.</param>
        /// <param name="handle">The underlying reflection object.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null.</exception>
        public StaticAssemblyWrapper(StaticReflectionPolicy policy, object handle)
            : base(policy, handle)
        {
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return GetName().Name; }
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Assembly; }
        }

        /// <inheritdoc />
        public override CodeReference CodeReference
        {
            get { return new CodeReference(FullName, null, null, null, null); }
        }

        /// <inheritdoc />
        public string Path
        {
            get { return assemblyPathMemoizer.Memoize(() => ReflectionPolicy.GetAssemblyPath(this)); }
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return GetName().FullName; }
        }

        /// <inheritdoc />
        public override CodeLocation GetCodeLocation()
        {
            return new CodeLocation(Path, 0, 0);
        }

        /// <inheritdoc />
        public AssemblyName GetName()
        {
            return assemblyNameMemoizer.Memoize(() => ReflectionPolicy.GetAssemblyName(this));
        }

        /// <inheritdoc />
        public IList<AssemblyName> GetReferencedAssemblies()
        {
            return referencedAssembliesMemoizer.Memoize(() =>
                new ReadOnlyCollection<AssemblyName>(ReflectionPolicy.GetAssemblyReferences(this)));
        }

        /// <inheritdoc />
        public IList<ITypeInfo> GetExportedTypes()
        {
            return exportedTypesMemoizer.Memoize(() =>
                new CovariantList<StaticDeclaredTypeWrapper, ITypeInfo>(ReflectionPolicy.GetAssemblyExportedTypes(this)));
        }

        /// <inheritdoc />
        public IList<ITypeInfo> GetTypes()
        {
            return typesMemoizer.Memoize(() =>
                new CovariantList<StaticDeclaredTypeWrapper, ITypeInfo>(ReflectionPolicy.GetAssemblyTypes(this)));
        }

        /// <inheritdoc />
        public ITypeInfo GetType(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            return getTypeMemoizer.Memoize(typeName, () =>
                ReflectionPolicy.GetAssemblyType(this, typeName));
        }

        /// <inheritdoc />
        public Assembly Resolve(bool throwOnError)
        {
            return resolveMemoizer.Memoize(throwOnError, () =>
                ReflectorResolveUtils.ResolveAssembly(this, true, throwOnError));
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

        /// <summary>
        /// Returns true if the internal members of this assembly are visible to the specified assembly.
        /// </summary>
        /// <param name="other">The other assembly.</param>
        /// <returns>True if the other assembly can see internal members of this assembly.</returns>
        public bool IsAssemblyVisibleTo(StaticAssemblyWrapper other)
        {
            // FIXME: Should check InternalsVisibleTo.
            return Equals(other);
        }

        /// <excludedoc />
        protected override IEnumerable<StaticAttributeWrapper> GetCustomAttributes()
        {
            return ReflectionPolicy.GetAssemblyCustomAttributes(this);
        }

        /// <excludedoc />
        protected override IEnumerable<Attribute> GetPseudoCustomAttributes()
        {
            // TODO: Handle code access security.
            return EmptyArray<Attribute>.Instance;
        }
    }
}
