// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Gallio.Reflection.Impl;

namespace Gallio.Reflection.Impl
{
    internal abstract class NativeMemberWrapper<TTarget> : NativeCodeElementWrapper<TTarget>, IMemberInfo
        where TTarget : MemberInfo
    {
        public NativeMemberWrapper(TTarget target)
            : base(target)
        {
        }

        public override string Name
        {
            get { return Target.Name; }
        }

        public MemberTypes MemberType
        {
            get { return Target.MemberType; }
        }

        public override CodeReference CodeReference
        {
            get { return CodeReference.CreateFromMember(Target); }
        }

        public override string GetXmlDocumentation()
        {
            return XmlDocumentationUtils.GetXmlDocumentation(Target);
        }

        public override CodeLocation GetCodeLocation()
        {
            return DeclaringType.GetCodeLocation();
        }

        public ITypeInfo DeclaringType
        {
            get { return Reflector.Wrap(Target.DeclaringType); }
        }

        public MemberInfo Resolve(bool throwOnError)
        {
            return Target;
        }


        public override bool Equals(object obj)
        {
            // Note: We can't compare most MemberInfo objects directly (except for Types).
            //       So we have to settle for comparing their metadata tokens and declaring types instead.
            NativeMemberWrapper<TTarget> other = obj as NativeMemberWrapper<TTarget>;
            return other != null
                && Target.MetadataToken == other.Target.MetadataToken
                && CompareDeclaringTypeWorkaround(Target.DeclaringType, other.Target.DeclaringType);
        }

        public override int GetHashCode()
        {
            return Target.MetadataToken;
        }

        public bool Equals(IMemberInfo other)
        {
            return Equals((object)other);
        }

        /*
         * There seems to be a bug in the .Net framework recovering type information
         * from the declaring method of a generic method parameter.
         * 
         * Given the following:
         *     class Class<T>
         *     {
         *         void Method<S>(T t);
         *     }
         * 
         * Generic type instantiation information is conserved here:
         *     typeof(Class<int>).GetMethod("Method").DeclaringType.ToString() == "Class[System.Int32]"
         * 
         * And it can be indirectly observed here as part of the method's first parameter:
         *     typeof(Class<int>).GetMethod("Method").GetGenericArguments()[0].DeclaringMember.ToString() == "Void Method[S](System.Int32)"
         * 
         * However it is not really preserved:
         *     typeof(Class<int>).GetMethod("Method").GetGenericArguments()[0].DeclaringMember.DeclaringType.ToString() == "Class[T]"
         * 
         * Consequently we cannot rely on always having generic type instantiation information.
         * 
         * So we discard this information for equality comparisons to avoid breaking clients that
         * are not aware of the limitations.
         * 
         * If we should start to rely on this information someday, then we may need to find a way to
         * work around the loss of the information somehow.  -- Jeff.
         */
        private static bool CompareDeclaringTypeWorkaround(Type x, Type y)
        {
            if (x.IsGenericType)
                x = x.GetGenericTypeDefinition();
            if (y.IsGenericType)
                y = y.GetGenericTypeDefinition();

            return x == y;
        }
    }
}