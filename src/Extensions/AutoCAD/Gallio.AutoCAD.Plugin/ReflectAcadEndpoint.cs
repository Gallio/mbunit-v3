using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Gallio.AutoCAD.Plugin
{
    /// <summary>
    /// Provides access to the <c>AcadEndpoint</c> type via reflection.
    /// </summary>
    internal class ReflectAcadEndpoint : IDisposable
    {
        private const string AcadEndpointTypeName = "Gallio.AutoCAD.AcadEndpoint";
        private static readonly Type[] ConstructorArgumentTypes = new Type[] { typeof(string), typeof(Process) };
        private static readonly Type[] RunArgumentTypes = new Type[] { typeof(TimeSpan) };
        private static Type endpointType;
        private IDisposable actual;

        public ReflectAcadEndpoint(string extensionPath, string ipcPortName, Process ownerProcess)
        {
            if (endpointType == null)
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(extensionPath);
                string typeName = Assembly.CreateQualifiedName(assemblyName.FullName, AcadEndpointTypeName);
                endpointType = Type.GetType(typeName);
                if (endpointType == null)
                    throw new InvalidOperationException("Unable to locate AcadEndpoint type.");
            }

            ConstructorInfo cinfo = endpointType.GetConstructor(ConstructorArgumentTypes);
            actual = (IDisposable)cinfo.Invoke(new object[] { ipcPortName, ownerProcess });
        }

        public void Run(TimeSpan pingTimeout)
        {
            MethodInfo minfo = endpointType.GetMethod("Run", RunArgumentTypes);
            minfo.Invoke(actual, new object[] { pingTimeout });
        }

        private static Type GetEndpointType()
        {
            if (endpointType != null)
                return endpointType;


            return endpointType;
        }

        public void Dispose()
        {
            if (actual != null)
            {
                actual.Dispose();
                actual = null;
            }
        }
    }
}
