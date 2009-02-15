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
