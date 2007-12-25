// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

using System.Reflection;
using Gallio.Hosting;

namespace Gallio.Reflection.Impl
{
    internal sealed class NativeFieldWrapper : NativeMemberWrapper<FieldInfo>, IFieldInfo
    {
        public NativeFieldWrapper(FieldInfo target)
            : base(target)
        {
        }

        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(Target.FieldType); }
        }

        public int Position
        {
            get { return 0; }
        }

        public FieldAttributes FieldAttributes
        {
            get { return Target.Attributes; }
        }

        public bool IsLiteral
        {
            get { return Target.IsLiteral; }
        }

        public bool IsPublic
        {
            get { return Target.IsPublic; }
        }

        public bool IsInitOnly
        {
            get { return Target.IsInitOnly; }
        }

        public bool IsStatic
        {
            get { return Target.IsStatic; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Field; }
        }

        new public FieldInfo Resolve()
        {
            return Target;
        }

        public override string GetXmlDocumentation()
        {
            return Loader.XmlDocumentationResolver.GetXmlDocumentation(Target);
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(IFieldInfo other)
        {
            return Equals((object)other);
        }
    }
}