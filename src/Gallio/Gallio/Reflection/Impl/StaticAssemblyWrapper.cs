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
using System.Reflection;
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> assembly wrapper.
    /// </summary>
    public sealed class StaticAssemblyWrapper : StaticCodeElementWrapper, IAssemblyInfo
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
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
            get { return Policy.GetAssemblyPath(this); }
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
            return Policy.GetAssemblyName(this);
        }

        /// <inheritdoc />
        public IList<AssemblyName> GetReferencedAssemblies()
        {
            return Policy.GetAssemblyReferences(this);
        }

        /// <inheritdoc />
        public IList<ITypeInfo> GetExportedTypes()
        {
            return new CovariantList<StaticDeclaredTypeWrapper, ITypeInfo>(Policy.GetAssemblyExportedTypes(this));
        }

        /// <inheritdoc />
        public IList<ITypeInfo> GetTypes()
        {
            return new CovariantList<StaticDeclaredTypeWrapper, ITypeInfo>(Policy.GetAssemblyTypes(this));
        }

        /// <inheritdoc />
        public ITypeInfo GetType(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            return Policy.GetAssemblyType(this, typeName);
        }

        /// <inheritdoc />
        public Assembly Resolve()
        {
            try
            {
                return Assembly.Load(FullName);
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(this, ex);
            }
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
        /// <param name="other">The other assembly</param>
        /// <returns>True if the other assembly can see internal members of this assembly</returns>
        public bool IsAssemblyVisibleTo(StaticAssemblyWrapper other)
        {
            // FIXME: Should check InternalsVisibleTo.
            return Equals(other);
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticAttributeWrapper> GetCustomAttributes()
        {
            return Policy.GetAssemblyCustomAttributes(this);
        }

        /// <inheritdoc />
        protected override IEnumerable<Attribute> GetPseudoCustomAttributes()
        {
            // TODO: Handle code access security.
            return EmptyArray<Attribute>.Instance;
        }
    }
}
