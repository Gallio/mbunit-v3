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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Policy;
using Gallio.Common.Collections;

#if DOTNET40
using System.Linq;
using System.Security;

namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
#if DOTNET40
    internal sealed partial class UnresolvedAssembly : Assembly, IUnresolvedCodeElement, IEquatable<UnresolvedAssembly>
#else
    internal sealed partial class UnresolvedAssembly : AssemblyShim, IUnresolvedCodeElement, IEquatable<UnresolvedAssembly>
#endif
    {
        private readonly IAssemblyInfo adapter;

        internal UnresolvedAssembly(IAssemblyInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;

#if ! DOTNET40
            DeepInitializeForDotNet20();
#endif
        }

        public IAssemblyInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        public override string CodeBase
        {
            get { return new Uri(adapter.Path).ToString(); }
        }

        public override MethodInfo EntryPoint
        {
            get
            {
                throw new NotSupportedException("Cannot get entry point of an unresolved assembly.");
            }
        }

        public override Evidence Evidence
        {
            get
            {
                throw new NotSupportedException("Cannot get evidence of unresolved assembly.");
            }
        }

        public override string FullName
        {
            get
            {
                return adapter.FullName;
            }
        }

        public override Type[] GetExportedTypes()
        {
            return GenericCollectionUtils.ConvertAllToArray(adapter.GetExportedTypes(),
                type => type.Resolve(false));
        }

        public override FileStream GetFile(string name)
        {
            return null;
        }

        public override FileStream[] GetFiles(bool getResourceModules)
        {
            return EmptyArray<FileStream>.Instance;
        }

        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            return null;
        }

        public override string[] GetManifestResourceNames()
        {
            return EmptyArray<string>.Instance;
        }

        public override Stream GetManifestResourceStream(string name)
        {
            return null;
        }

        public override Stream GetManifestResourceStream(Type type, string name)
        {
            return null;
        }

        public override AssemblyName GetName(bool copiedName)
        {
            return adapter.GetName();
        }

#if DOTNET40
        [SecurityCritical]
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException("Cannot serialize an unresolved assembly.");
        }

        public override Type GetType(string name)
        {
            return GetType(name, false);
        }

        public override Type GetType(string name, bool throwOnError)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            ITypeInfo type = adapter.GetType(name);
            if (type == null)
            {
                if (throwOnError)
                    throw new TypeLoadException(string.Format("Cannot find type '{0}'.", name));
                return null;
            }

            return type.Resolve(false);
        }

        public override Type[] GetTypes()
        {
            return GenericCollectionUtils.ConvertAllToArray(adapter.GetTypes(),
                type => type.Resolve(false));
        }

        public override string ImageRuntimeVersion
        {
            get
            {
                return RuntimeEnvironment.GetSystemVersion();
            }
        }

        public override string Location
        {
            get { return adapter.Path; }
        }

        public override bool ReflectionOnly
        {
            get { return true; }
        }

        public override bool Equals(object o)
        {
            return Equals(o as UnresolvedAssembly);
        }

        public bool Equals(UnresolvedAssembly other)
        {
            return other != null && adapter.Equals(other.adapter);
        }

        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        #region .Net 2.0 Only
#if ! DOTNET40
        // Hacks the assembly object to ensure it is completely initialized.
        // This is only required for .Net 2.0.
        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        private void DeepInitializeForDotNet20()
        {
            Assembly donorAssembly = typeof(UnresolvedAssembly).Assembly;

            // The Assembly object contains internal pointers (such as 'm__assembly')
            // that must be initialized to prevent exceptions if client code calls methods
            // that we have been unable to override as part of this hack.  Of course those
            // methods still won't work but at least we should not get an execution engine error.
            foreach (FieldInfo field in typeof(Assembly).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object value = field.GetValue(donorAssembly);
                field.SetValue(this, value);
            }
        }
#endif
        #endregion

        #region .Net 4.0 Only
#if DOTNET40
        public override object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
        {
            throw new NotSupportedException("Cannot create instance of type in unresolved assembly.");
        }

        public override Assembly GetSatelliteAssembly(CultureInfo culture)
        {
            throw new NotSupportedException("Cannot get satellite assemblies of an unresolved assembly.");
        }

        public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
        {
            throw new NotSupportedException("Cannot get satellite assemblies of an unresolved assembly.");
        }

        public override Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            if (ignoreCase)
            {
                foreach (ITypeInfo type in adapter.GetTypes())
                {
                    if (string.Compare(name, type.Name, true) == 0)
                        return type.Resolve(false);
                }

                if (throwOnError)
                    throw new TypeLoadException(string.Format("Cannot find type '{0}' case-insensitively.", name));
                return null;
            }

            return GetType(name, throwOnError);
        }

        public override bool GlobalAssemblyCache
        {
            get { return false; }
        }

        public override long HostContext
        {
            get
            {
                throw new NotSupportedException("Cannot get host context of unresolved assembly.");
            }
        }

        public override AssemblyName[] GetReferencedAssemblies()
        {
            return adapter.GetReferencedAssemblies().ToArray();
        }

        public override bool IsDynamic
        {
            get
            {
                return false;
            }
        }

        public override Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
        {
            throw new NotSupportedException("Cannot load module of unresolved assembly.");
        }

        public override Module[] GetLoadedModules(bool getResourceModules)
        {
            return GetModules(getResourceModules);
        }

        public override Module GetModule(string name)
        {
            if (name == adapter.GetName().Name)
                return ManifestModule;
            return null;
        }

        public override Module[] GetModules(bool getResourceModules)
        {
            return new[] { ManifestModule };
        }

        public override Module ManifestModule
        {
            get
            {
                // TODO: Return an unresolved Module as an emulated wrapper around the IAssemblyInfo.
                throw new NotImplementedException();
            }
        }

        public override event ModuleResolveEventHandler ModuleResolve
        {
            [SecurityCritical]
            add { }
            [SecurityCritical]
            remove { }
        }

        public override PermissionSet PermissionSet
        {
            [SecurityCritical]
            get
            {
                throw new NotSupportedException("Cannot get permission set of unresolved assembly.");
            }
        }
#endif
        #endregion
    }
}