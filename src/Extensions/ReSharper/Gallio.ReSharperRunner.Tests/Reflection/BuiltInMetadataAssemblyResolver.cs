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

using System.IO;
using System.Reflection;
using Gallio.Common.Reflection;
using JetBrains.Metadata.Access;
using IAssemblyResolver=JetBrains.Metadata.Reader.API.IAssemblyResolver;

#if RESHARPER_50_OR_NEWER
using JetBrains.Util;
using JetBrains.Metadata.Utils;
#endif

namespace Gallio.ReSharperRunner.Tests.Reflection
{
    public class BuiltInMetadataAssemblyResolver : IAssemblyResolver
    {
        public static readonly BuiltInMetadataAssemblyResolver Instance = new BuiltInMetadataAssemblyResolver();

        private BuiltInMetadataAssemblyResolver()
        {
        }

#if ! RESHARPER_50_OR_NEWER
        public IMetadataAccess ResolveAssembly(AssemblyName name, out string assemblyLocation)
        {
            try
            {
                Assembly assembly = Assembly.Load(name);
                assemblyLocation = AssemblyUtils.GetAssemblyLocalPath(assembly);
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
                return MetadataProvider.GetFromFile(assemblyLocation);
#else
                return MetadataProvider.Instance.GetFromFile(assemblyLocation);
#endif
            }
            catch
            {
                assemblyLocation = null;
                return null;
            }
        }
#else
        public IMetadataAccess ResolveAssembly(AssemblyNameInfo name, out FileSystemPath assemblyLocation)
        {
            try
            {
                Assembly assembly = Assembly.Load(name.FullName);
                assemblyLocation = new FileSystemPath((AssemblyUtils.GetAssemblyLocalPath(assembly)));
                return MetadataProviderFactory.DefaultProvider.GetFromFile(assemblyLocation);
            }
            catch
            {
                assemblyLocation = null;
                return null;
            }
        }
#endif
    }
}
