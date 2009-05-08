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

using System.Collections.Generic;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Sorts an members that all belong to the same type
    /// such that the members declared by supertypes appear before those
    /// declared by subtypes.
    /// </summary>
    /// <example>
    /// If type A derives from types B and if type C derives from B then given methods
    /// A.Foo, A.Bar, B.Foo, C.Quux one possible sorted order will be:
    /// B.Foo, C.Quux, A.Bar, A.Foo.  The members are not sorted by name or
    /// by any other criterion except by relative specificity of the
    /// declaring types.
    /// </example>
    public sealed class DeclaringTypeComparer<T> : IComparer<T>
        where T : IMemberInfo
    {
        /// <summary>
        /// Gets the singleton instance of the comparer.
        /// </summary>
        public static readonly DeclaringTypeComparer<T> Instance = new DeclaringTypeComparer<T>();

        private DeclaringTypeComparer()
        {
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            ITypeInfo tx = x.DeclaringType, ty = y.DeclaringType;
            if (ty != null && (tx == null || ty.IsSubclassOf(tx)))
                return -1;
            if (tx != null && (ty == null || tx.IsSubclassOf(ty)))
                return 1;

            return 0;
        }
    }
}
