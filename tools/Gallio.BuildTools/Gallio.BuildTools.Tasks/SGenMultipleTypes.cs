using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Gallio.BuildTools.Tasks
{
    /// <summary>
    /// Generates an XmlSerializers assemblies for multiple types in an assembly.
    /// </summary>
    public class SGenMultipleTypes : Task
    {
        public string BuildAssemblyName { get; set; }

        public string BuildAssemblyPath { get; set; }

        public string[] References { get; set; }

        public string KeyFile { get; set; }

        [Output]
        public string SerializationAssembly { get; private set; }

        public override bool Execute()
        {
            // Use separate AppDomain so that we can unload the assemblies we generated
            // the XmlSerializers from.  Otherwise when this task is used inside Visual Studio
            // then the assembly will remain locked in memory.
            AppDomain domain = AppDomain.CreateDomain("RemoteTask");
            try
            {
                Type remoteTaskType = typeof(RemoteTask);
                var remoteTask = (RemoteTask)domain.CreateInstanceFromAndUnwrap(
                    new Uri(remoteTaskType.Assembly.CodeBase).LocalPath, remoteTaskType.FullName);

                SerializationAssembly = remoteTask.Execute(BuildAssemblyPath, BuildAssemblyName, References, KeyFile);
            }
            finally
            {
                AppDomain.Unload(domain);
            }

            return true;
        }

        public sealed class RemoteTask : MarshalByRefObject
        {
            private string assemblyDir;

            public string Execute(string assemblyDir, string assemblyFileName, string[] references, string keyFile)
            {
                this.assemblyDir = assemblyDir;
                string assemblyPath = Path.GetFullPath(Path.Combine(assemblyDir, assemblyFileName));

                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
                try
                {
                    string serializersAssemblyPath = Path.ChangeExtension(assemblyPath, ".XmlSerializers.dll");

                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    //string serializersAssemblyName = BuildAssemblyName + ".XmlSerializers";

                    List<Type> serializableTypes = new List<Type>();
                    foreach (Type type in assembly.GetExportedTypes())
                    {
                        if (type.IsDefined(typeof(XmlRootAttribute), false))
                            serializableTypes.Add(type);
                    }

                    List<XmlMapping> mappings = new List<XmlMapping>();
                    XmlReflectionImporter importer = new XmlReflectionImporter();

                    foreach (Type type in serializableTypes)
                    {
                        XmlMapping mapping = importer.ImportTypeMapping(type);
                        mappings.Add(mapping);
                    }

                    CompilerParameters compilerParameters = new CompilerParameters();

                    compilerParameters.TempFiles = new TempFileCollection();
                    compilerParameters.IncludeDebugInformation = false;
                    compilerParameters.GenerateInMemory = false;
                    compilerParameters.OutputAssembly = serializersAssemblyPath;

                    foreach (var reference in references)
                    {
                        if (!reference.Contains("System.Xml"))
                            compilerParameters.ReferencedAssemblies.Add(reference);
                    }

                    if (keyFile != null)
                        compilerParameters.CompilerOptions += " /keyfile:\"" + keyFile + "\"";

                    XmlSerializer.GenerateSerializer(serializableTypes.ToArray(),
                        mappings.ToArray(), compilerParameters);

                    return serializersAssemblyPath;
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolver;
                }
            }

            private Assembly AssemblyResolver(object sender, ResolveEventArgs e)
            {
                var assemblyName = new AssemblyName(e.Name).Name + ".dll";
                var assemblyPath = Path.Combine(assemblyDir, assemblyName);

                if (File.Exists(assemblyPath))
                    return Assembly.LoadFrom(assemblyPath);

                return null;
            }
        }
    }
}
