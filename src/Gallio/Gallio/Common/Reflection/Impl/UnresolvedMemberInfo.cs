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
using Gallio.Common.Collections;

/*
 * This compilation unit contains MemberInfo overrides that must be duplicated for each
 * of the unresolved reflection types because C# does not support multiple inheritance.
 */

#if DOTNET40
namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
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

    internal partial class UnresolvedType
    {
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        public override int MetadataToken
        {
            get { throw new NotSupportedException("Cannot get metadata token of unresolved type."); }
        }

        public override Module Module
        {
            get { throw new NotSupportedException("Cannot get module of unresolved type."); }
        }

        public override string Name
        {
            get { return adapter.Name; }
        }

        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        public override bool Equals(object o)
        {
            UnresolvedType other = o as UnresolvedType;
            return other != null && adapter.Equals(other.adapter);
        }

        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    internal partial class UnresolvedFieldInfo
    {
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        public override int MetadataToken
        {
            get { throw new NotSupportedException("Cannot get metadata token of unresolved field."); }
        }

        public override Module Module
        {
            get { throw new NotSupportedException("Cannot get module of unresolved field."); }
        }

        public override string Name
        {
            get { return adapter.Name; }
        }

        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        public override bool Equals(object o)
        {
            UnresolvedFieldInfo other = o as UnresolvedFieldInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    internal partial class UnresolvedPropertyInfo
    {
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        public override int MetadataToken
        {
            get { throw new NotSupportedException("Cannot get metadata token of unresolved property."); }
        }

        public override Module Module
        {
            get { throw new NotSupportedException("Cannot get module of unresolved property."); }
        }

        public override string Name
        {
            get { return adapter.Name; }
        }

        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        public override bool Equals(object o)
        {
            UnresolvedPropertyInfo other = o as UnresolvedPropertyInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    internal partial class UnresolvedEventInfo
    {
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        public override int MetadataToken
        {
            get { throw new NotSupportedException("Cannot get metadata token of unresolved event."); }
        }

        public override Module Module
        {
            get { throw new NotSupportedException("Cannot get module of unresolved event."); }
        }

        public override string Name
        {
            get { return adapter.Name; }
        }

        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        public override bool Equals(object o)
        {
            UnresolvedEventInfo other = o as UnresolvedEventInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    internal partial class UnresolvedMethodInfo
    {
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        public override int MetadataToken
        {
            get { throw new NotSupportedException("Cannot get metadata token of unresolved method."); }
        }

        public override Module Module
        {
            get { throw new NotSupportedException("Cannot get module of unresolved method."); }
        }

        public override string Name
        {
            get { return adapter.Name; }
        }

        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        public override bool Equals(object o)
        {
            UnresolvedMethodInfo other = o as UnresolvedMethodInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        public override string ToString()
        {
            return adapter.ToString();
        }
    }

    internal partial class UnresolvedConstructorInfo
    {
        public override Type DeclaringType
        {
            get { return UnresolvedMemberInfo.GetDeclaringType(adapter); }
        }

        public override int MetadataToken
        {
            get { throw new NotSupportedException("Cannot get metadata token of unresolved constructor."); }
        }

        public override Module Module
        {
            get { throw new NotSupportedException("Cannot get module of unresolved constructor."); }
        }

        public override string Name
        {
            get { return adapter.Name; }
        }

        public override Type ReflectedType
        {
            get { return DeclaringType; }
        }

        public override bool Equals(object o)
        {
            UnresolvedConstructorInfo other = o as UnresolvedConstructorInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        public override string ToString()
        {
            return adapter.ToString();
        }
    }
}