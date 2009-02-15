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
using System.IO;

namespace Gallio.AutoCAD.Plugin
{
    /// <summary>
    /// Provides access to the <c>GallioLoader</c> type via reflection.
    /// </summary>
    internal class ReflectGallioLoader
    {
        private const string GallioLoaderTypeName = "Gallio.Loader.GallioLoader";
        private const string IGallioLoaderTypeName = "Gallio.Loader.IGallioLoader";
        
        private const  string          InitializeMethodName    = "Initialize";
        private const  BindingFlags    InitializeBindingFlags  = BindingFlags.Public | BindingFlags.Static;
        private static readonly Type[] InitializeArgumentTypes = new Type[] { typeof(string) };
        
        private const  string          SetupRuntimeMethodName    = "SetupRuntime";
        private const  BindingFlags    SetupRuntimeBindingFlags  = BindingFlags.Public | BindingFlags.Instance;
        private static readonly Type[] SetupRuntimeArgumentTypes = new Type[0];

        private object actual;

        private ReflectGallioLoader(object actual)
        {
            this.actual = actual;
        }

        public static ReflectGallioLoader Initialize(string runtimePath)
        {
            Assembly assembly = GetLoaderAssembly(runtimePath);
            Type gallioLoaderType = assembly.GetType(GallioLoaderTypeName, true);
            MethodInfo initializeMethod = GetMethod(gallioLoaderType, InitializeMethodName, InitializeBindingFlags, InitializeArgumentTypes);
            object actualLoader = initializeMethod.Invoke(null, new object[] { runtimePath });
            return new ReflectGallioLoader(actualLoader);
        }

        private static Assembly GetLoaderAssembly(string runtimePath)
        {
            try
            {
                // We're assuming here that the full assembly names for Gallio.Loader.dll
                // and Gallio.dll will share everything except the simple name portion.
                AssemblyName gallioDll = AssemblyName.GetAssemblyName(Path.Combine(runtimePath, "Gallio.dll"));
                AssemblyName gallioLoaderDll = new AssemblyName(gallioDll.FullName);
                gallioLoaderDll.Name = "Gallio.Loader";
                return Assembly.Load(gallioLoaderDll.FullName); // Need to use the full name to probe the GAC.
            }
            catch (FileNotFoundException)
            {
                return Assembly.LoadFrom(Path.Combine(runtimePath, "Gallio.Loader.dll"));
            }
        }

        public void SetupRuntime()
        {
            Type loaderInterface = GetInterface(actual.GetType(), IGallioLoaderTypeName);
            MethodInfo setupMethod = GetMethod(loaderInterface, SetupRuntimeMethodName, SetupRuntimeBindingFlags, SetupRuntimeArgumentTypes);
            setupMethod.Invoke(actual, null);
        }

        private static MethodInfo GetMethod(Type type, string methodName, BindingFlags bindingFlags, Type[] argumentTypes)
        {
            MethodInfo info = type.GetMethod(methodName, bindingFlags, null, argumentTypes, null);
            if (info == null)
                throw new MissingMethodException(type.Name, methodName);
            return info;
        }

        private static Type GetInterface(Type type, string interfaceName)
        {
            Type interfaceType = type.GetInterface(interfaceName);
            if (interfaceType == null)
                throw new TypeLoadException(String.Concat("Unable to find interface \"", interfaceName, "\" on type \"", type.Name, "\""));
            return interfaceType;
        }
    }
}
