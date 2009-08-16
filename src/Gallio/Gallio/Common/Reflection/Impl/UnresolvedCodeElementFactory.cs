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
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Gallio.Common.Platform;

#if DOTNET40
using Gallio.Common.Reflection.Impl.DotNet40;
#else
using Gallio.Common.Reflection.Impl.DotNet20;
#endif

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Creates instances of unresolved reflection objects based on <see cref="ICodeElementInfo"/> wrappers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All objects produced by this factory implement <see cref="IUnresolvedCodeElement" />.
    /// </para>
    /// </remarks>
    public abstract class UnresolvedCodeElementFactory
    {
        private static UnresolvedCodeElementFactory instance;

        /// <summary>
        /// Creates an instance of the factory.
        /// </summary>
        protected UnresolvedCodeElementFactory()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the factory.
        /// </summary>
        [DebuggerNonUserCode]
        public static UnresolvedCodeElementFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    UnresolvedCodeElementFactory newInstance = null;
                    
                    if (DotNetFrameworkSupport.FrameworkVersion >= DotNetFrameworkVersion.DotNet40)
                    {
                        try
                        {
                            AssemblyName assemblyName = typeof(UnresolvedCodeElementFactory).Assembly.GetName();
                            assemblyName.Name = "Gallio40";

                            Assembly assembly = Assembly.Load(assemblyName);
                            Type factoryType = assembly.GetType("Gallio.Common.Reflection.Impl.DotNet40.UnresolvedCodeElementFactoryInternal");
                            newInstance = (UnresolvedCodeElementFactory) Activator.CreateInstance(factoryType, true);
                        }
                        catch
                        {
                            // Ignore the error and fallback on .Net 2.0 compatible version.
                        }
                    }
                    
                    if (newInstance == null)
                        newInstance = new UnresolvedCodeElementFactoryInternal();

                    Interlocked.CompareExchange(ref instance, newInstance, null);
                }

                return instance;
            }
        }

        /// <summary>
        /// Creates an <see cref="Assembly" /> wrapper for <see cref="IAssemblyInfo" />.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>The unresolved assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
        public abstract Assembly Wrap(IAssemblyInfo adapter);

        /// <summary>
        /// Creates a <see cref="ConstructorInfo" /> wrapper for <see cref="IConstructorInfo" />.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>The unresolved constructor.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
        public abstract ConstructorInfo Wrap(IConstructorInfo adapter);

        /// <summary>
        /// Creates a <see cref="EventInfo" /> wrapper for <see cref="IEventInfo" />.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>The unresolved event.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
        public abstract EventInfo Wrap(IEventInfo adapter);

        /// <summary>
        /// Creates a <see cref="FieldInfo" /> wrapper for <see cref="IFieldInfo" />.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>The unresolved field.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
        public abstract FieldInfo Wrap(IFieldInfo adapter);

        /// <summary>
        /// Creates a <see cref="MethodInfo" /> wrapper for <see cref="IMethodInfo" />.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>The unresolved method.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
        public abstract MethodInfo Wrap(IMethodInfo adapter);

        /// <summary>
        /// Creates a <see cref="ParameterInfo" /> wrapper for <see cref="IParameterInfo" />.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>The unresolved parameter.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
        public abstract ParameterInfo Wrap(IParameterInfo adapter);

        /// <summary>
        /// Creates a <see cref="PropertyInfo" /> wrapper for <see cref="IPropertyInfo" />.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>The unresolved property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
        public abstract PropertyInfo Wrap(IPropertyInfo adapter);

        /// <summary>
        /// Creates a <see cref="Type" /> wrapper for <see cref="ITypeInfo" />.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>The unresolved type.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
        public abstract Type Wrap(ITypeInfo adapter);
    }
}