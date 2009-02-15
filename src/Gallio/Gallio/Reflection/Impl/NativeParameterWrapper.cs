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

using System.Reflection;
using Gallio.Reflection.Impl;

namespace Gallio.Reflection.Impl
{
    internal sealed class NativeParameterWrapper : NativeCodeElementWrapper<ParameterInfo>, IParameterInfo
    {
        public NativeParameterWrapper(ParameterInfo target)
            : base(target)
        {
        }

        public override string Name
        {
            get { return Target.Name; }
        }

        public override CodeReference CodeReference
        {
            get { return CodeReference.CreateFromParameter(Target); }
        }

        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(Target.ParameterType); }
        }

        public int Position
        {
            get { return Target.Position; }
        }

        public IMemberInfo Member
        {
            get { return Reflector.Wrap(Target.Member); }
        }

        public ParameterAttributes ParameterAttributes
        {
            get { return Target.Attributes; }
        }

        public bool IsIn
        {
            get { return Target.IsIn; }
        }

        public bool IsOptional
        {
            get { return Target.IsOptional; }
        }

        public bool IsOut
        {
            get { return Target.IsOut; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Parameter; }
        }

        public ParameterInfo Resolve(bool throwOnError)
        {
            return Target;
        }

        public override string GetXmlDocumentation()
        {
            return null;
        }

        public override bool Equals(object obj)
        {
            // Note: ParameterInfo objects can't be compared for equality directly.
            NativeParameterWrapper other = obj as NativeParameterWrapper;
            return other != null
                && Target.MetadataToken == other.Target.MetadataToken
                && Target.Member.Equals(other.Target.Member);
        }

        public override int GetHashCode()
        {
            return Target.MetadataToken;
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(IParameterInfo other)
        {
            return Equals((object)other);
        }
    }
}