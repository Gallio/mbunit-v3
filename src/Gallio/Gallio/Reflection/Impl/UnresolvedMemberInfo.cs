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
using System.Reflection;
using Gallio.Collections;

/*
 * This compilation unit contains MemberInfo overrides that must be duplicated for each
 * of the unresolved reflection types because C# does not support multiple inheritance.
 */

namespace Gallio.Reflection.Impl
{
    internal static class UnresolvedMemberInfo
    {
        public static Type GetDeclaringType(IMemberInfo member)
        {
            return ResolveType(member.DeclaringType);
        }

        public static MethodInfo ResolveMethod(IMethodInfo method, bool nonPublic)
        {
            return method != null && (nonPublic || method.IsPublic) ? method.Resolve(false) : null;
        }

        public static Type ResolveType(ITypeInfo type)
        {
            return type != null ? type.Resolve(false) : null;
        }
    }

    public partial class UnresolvedType
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedType other = o as UnresolvedType;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedFieldInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedFieldInfo other = o as UnresolvedFieldInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedPropertyInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedPropertyInfo other = o as UnresolvedPropertyInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedEventInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedEventInfo other = o as UnresolvedEventInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedMethodInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedMethodInfo other = o as UnresolvedMethodInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    public partial class UnresolvedConstructorInfo
    {
        /// <inheritdoc />
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        /// <inheritdoc />
        public override int MetadataToken
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Module Module
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedConstructorInfo other = o as UnresolvedConstructorInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }
}