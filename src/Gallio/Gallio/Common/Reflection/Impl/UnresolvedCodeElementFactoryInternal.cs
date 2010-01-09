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
using System.Diagnostics;
using System.Reflection;
using System.Threading;

#if DOTNET40
namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
    internal sealed class UnresolvedCodeElementFactoryInternal : UnresolvedCodeElementFactory
    {
        public override Assembly Wrap(IAssemblyInfo adapter)
        {
            return new UnresolvedAssembly(adapter);
        }

        public override ConstructorInfo Wrap(IConstructorInfo adapter)
        {
            return new UnresolvedConstructorInfo(adapter);
        }

        public override EventInfo Wrap(IEventInfo adapter)
        {
            return new UnresolvedEventInfo(adapter);
        }

        public override FieldInfo Wrap(IFieldInfo adapter)
        {
            return new UnresolvedFieldInfo(adapter);
        }

        public override MethodInfo Wrap(IMethodInfo adapter)
        {
            return new UnresolvedMethodInfo(adapter);
        }

        public override ParameterInfo Wrap(IParameterInfo adapter)
        {
            return new UnresolvedParameterInfo(adapter);
        }

        public override PropertyInfo Wrap(IPropertyInfo adapter)
        {
            return new UnresolvedPropertyInfo(adapter);
        }

        public override Type Wrap(ITypeInfo adapter)
        {
            return new UnresolvedType(adapter);
        }
    }
}