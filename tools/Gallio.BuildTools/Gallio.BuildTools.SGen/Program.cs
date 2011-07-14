using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Gallio.BuildTools.SGen
{
    class Program
    {
        public Program()
        {
            References = new List<string>();
        }

        public string BuildAssemblyName { get; set; }
        public string BuildAssemblyPath { get; set; }
        public IList<string> References { get; private set; }
        public string KeyFile { get; set; }

        private bool ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (i + 1 >= args.Length)
                {
                    Console.WriteLine("Missing value for argument.");
                    return false;
                }

                string @switch = args[i];
                string value = args[++i];

                switch (@switch)
                {
                    case "-an":
                        if (BuildAssemblyName != null)
                        {
                            Console.WriteLine("Assembly name specified multiple times.");
                            return false;
                        }

                        BuildAssemblyName = value;
                        break;

                    case "-ap":
                        if (BuildAssemblyPath != null)
                        {
                            Console.WriteLine("Assembly path specified multiple times.");
                            return false;
                        }

                        BuildAssemblyPath = value;
                        break;

                    case "-r":
                        References.Add(value);
                        break;

                    case "-k":
                        if (KeyFile != null)
                        {
                            Console.WriteLine("Key file specified multiple times.");
                            return false;
                        }

                        KeyFile = value;
                        break;
                    
                    default:
                        Console.WriteLine("Unrecognized switch {0}.", @switch);
                        return false;
                }
            }

            if (BuildAssemblyName == null || BuildAssemblyPath == null)
            {
                Console.WriteLine("Assembly name and path required.");
                return false;
            }

            return true;
        }

        static int Main(string[] args)
        {
            try
            {
                var program = new Program();
                program.BuildAssemblyName = "Gallio40.dll";
                program.BuildAssemblyPath = @"C:\Users\Aleks\Desktop\Dev4\mb-unit\v3\build\modules\Gallio\temp";
                program.KeyFile = @"C:\Users\Aleks\Desktop\Dev4\mb-unit\v3\src\Key.snk";
                //program.References.Add();
                
                if (!program.ParseArguments(args))
                {
                    Console.WriteLine(
                        "Invalid arguments.  This tool is meant to be called from the SGenMultipleTypes MSBuild task.");
                    return 1;
                }

                program.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 2;
            }
        }

        public void Run()
        {
            string absoluteAssemblyDir = Path.GetFullPath(BuildAssemblyPath);
            string assemblyPath = Path.Combine(absoluteAssemblyDir, BuildAssemblyName);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            {
                // FIXME: Imprecise
                var requestedAssemblyName = new AssemblyName(e.Name).Name;
                if (! requestedAssemblyName.Contains("System"))
                {
                    string requestedAssemblyDll = requestedAssemblyName + ".dll";

                    foreach (string reference in References)
                    {
                        if (reference.ToLowerInvariant().Contains(requestedAssemblyDll.ToLowerInvariant()))
                            return Assembly.LoadFrom(reference);
                    }

                    var candidateAssemblyPath = Path.Combine(absoluteAssemblyDir, requestedAssemblyDll);
                    if (File.Exists(candidateAssemblyPath))
                        return Assembly.LoadFrom(candidateAssemblyPath);
                }

                return null;
            };
            
            string serializersAssemblyPath = Path.ChangeExtension(assemblyPath, ".XmlSerializers.dll");
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

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

            if (KeyFile != null)
                compilerParameters.CompilerOptions += " /keyfile:\"" + KeyFile + "\"";

            XmlSerializer.GenerateSerializer(serializableTypes.ToArray(),
                mappings.ToArray(), compilerParameters);
        }
    }
}
