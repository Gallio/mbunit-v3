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
        private const string GallioLoaderDll = "Gallio.Loader.dll";
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
            Assembly assembly = Assembly.LoadFrom(Path.Combine(runtimePath, GallioLoaderDll));
            Type gallioLoaderType = assembly.GetType(GallioLoaderTypeName, true);
            MethodInfo initializeMethod = GetMethod(gallioLoaderType, InitializeMethodName, InitializeBindingFlags, InitializeArgumentTypes);
            object actualLoader = initializeMethod.Invoke(null, new object[] { runtimePath });
            return new ReflectGallioLoader(actualLoader);
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
