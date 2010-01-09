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
using Gallio.Common.Collections;
using System.Reflection;
using System.Collections.Generic;

/*
 * This compilation unit contains ICustomAttributeProvider overrides that must be duplicated for each
 * of the unresolved reflection types because C# does not support multiple inheritance.
 */

#if DOTNET40
using System.Linq;

namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
    internal static class UnresolvedCustomAttributeProvider
    {
        public static object[] GetCustomAttributes(ICodeElementInfo adapter, bool inherit)
        {
            return GenericCollectionUtils.ToArray(adapter.GetAttributes(null, inherit));
        }

        public static object[] GetCustomAttributes(ICodeElementInfo adapter, Type attributeType, bool inherit)
        {
            return GenericCollectionUtils.ToArray(adapter.GetAttributes(Reflector.Wrap(attributeType), inherit));
        }

        public static bool IsDefined(ICodeElementInfo adapter, Type attributeType, bool inherit)
        {
            return adapter.HasAttribute(Reflector.Wrap(attributeType), inherit);
        }

#if DOTNET40
        public static IList<CustomAttributeData> GetCustomAttributesData(ICodeElementInfo adapter)
        {
            return (from attrib in adapter.GetAttributeInfos(null, false)
                select (CustomAttributeData) new UnresolvedCustomAttributeData(attrib))
                .ToList().AsReadOnly();
        }
#endif
    }

    internal partial class UnresolvedAssembly
    {
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }

#if DOTNET40
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributesData(adapter);
        }
#endif
    }

    internal partial class UnresolvedConstructorInfo
    {
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }

#if DOTNET40
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributesData(adapter);
        }
#endif
    }

    internal partial class UnresolvedEventInfo
    {
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }

#if DOTNET40
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributesData(adapter);
        }
#endif
    }

    internal partial class UnresolvedFieldInfo
    {
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }

#if DOTNET40
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributesData(adapter);
        }
#endif
    }

    internal partial class UnresolvedMethodInfo
    {
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }

#if DOTNET40
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributesData(adapter);
        }
#endif
    }

    internal partial class UnresolvedParameterInfo
    {
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }

#if DOTNET40
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributesData(adapter);
        }
#endif
    }

    internal partial class UnresolvedPropertyInfo
    {
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }

#if DOTNET40
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributesData(adapter);
        }
#endif
    }

    internal partial class UnresolvedType
    {
        public override object[] GetCustomAttributes(bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributes(adapter, attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return UnresolvedCustomAttributeProvider.IsDefined(adapter, attributeType, inherit);
        }

#if DOTNET40
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return UnresolvedCustomAttributeProvider.GetCustomAttributesData(adapter);
        }
#endif
    }
}
