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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Policy;
using Gallio.Common.Collections;
using Gallio.Common.Platform;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Creates instances of <see cref="Assembly" /> to represent <see cref="IAssemblyInfo" />
    /// instances that could not be resolved.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation is a hack intended to provide minimal compatibility for legacy code
    /// that does not understand <see cref="IAssemblyInfo"/> or provide any other similar
    /// affordance for reflection adapters.  This code is not guaranteed to work with
    /// all implementations of the .Net framework because the <see cref="Assembly"/> class
    /// was not designed to be subclassed in the first place.
    /// </para>
    /// </remarks>
    public sealed partial class UnresolvedAssembly : AssemblyShim, IUnresolvedCodeElement, IEquatable<UnresolvedAssembly>
    {
        private readonly IAssemblyInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
        public UnresolvedAssembly(IAssemblyInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;

            DeepInitialize();
        }

        /// <summary>
        /// Gets the underlying reflection adapter.
        /// </summary>
        public IAssemblyInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        /// <inheritdoc />
        public override string CodeBase
        {
            get { return new Uri(adapter.Path).ToString(); }
        }

        /// <inheritdoc />
        public override MethodInfo EntryPoint
        {
            get
            {
                throw new NotSupportedException("Cannot get entry point of an unresolved assembly.");
            }
        }

        /// <inheritdoc />
        public override Evidence Evidence
        {
            get
            {
                throw new NotSupportedException("Cannot get evidence of unresolved assembly.");
            }
        }

        /// <inheritdoc />
        public override string FullName
        {
            get
            {
                return adapter.FullName;
            }
        }

        /// <inheritdoc />
        public override Type[] GetExportedTypes()
        {
            return GenericCollectionUtils.ConvertAllToArray(adapter.GetExportedTypes(),
                type => type.Resolve(false));
        }

        /// <inheritdoc />
        public override FileStream GetFile(string name)
        {
            return null;
        }

        /// <inheritdoc />
        public override FileStream[] GetFiles(bool getResourceModules)
        {
            return EmptyArray<FileStream>.Instance;
        }

        /// <inheritdoc />
        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            return null;
        }

        /// <inheritdoc />
        public override string[] GetManifestResourceNames()
        {
            return EmptyArray<string>.Instance;
        }

        /// <inheritdoc />
        public override Stream GetManifestResourceStream(string name)
        {
            return null;
        }

        /// <inheritdoc />
        public override Stream GetManifestResourceStream(Type type, string name)
        {
            return null;
        }

        /// <inheritdoc />
        public override AssemblyName GetName(bool copiedName)
        {
            return adapter.GetName();
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException("Cannot serialize an unresolved assembly.");
        }

        /// <inheritdoc />
        public override Type GetType(string name)
        {
            return GetType(name, false);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override Type[] GetTypes()
        {
            return GenericCollectionUtils.ConvertAllToArray(adapter.GetTypes(),
                type => type.Resolve(false));
        }

        /// <inheritdoc />
        public override string ImageRuntimeVersion
        {
            get
            {
                return RuntimeEnvironment.GetSystemVersion();
            }
        }

        /// <inheritdoc />
        public override string Location
        {
            get { return adapter.Path; }
        }

        /// <inheritdoc />
        public override bool ReflectionOnly
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            return Equals(o as UnresolvedAssembly);
        }

        /// <inheritdoc />
        public bool Equals(UnresolvedAssembly other)
        {
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        private void DeepInitialize()
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
    }
}