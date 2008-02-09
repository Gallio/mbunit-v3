using System.IO;
using System.Reflection;
using Gallio.Hosting;
using JetBrains.Metadata.Access;
using IAssemblyResolver=JetBrains.Metadata.Reader.API.IAssemblyResolver;

namespace Gallio.ReSharperRunner.Tests.Reflection
{
    public class BuiltInMetadataAssemblyResolver : IAssemblyResolver
    {
        public static readonly BuiltInMetadataAssemblyResolver Instance = new BuiltInMetadataAssemblyResolver();

        private BuiltInMetadataAssemblyResolver()
        {
        }

        public IMetadataAccess ResolveAssembly(AssemblyName name, out string assemblyLocation)
        {
            try
            {
                Assembly assembly = Assembly.Load(name);
                assemblyLocation = Loader.GetAssemblyLocalPath(assembly);
                return MetadataProvider.GetFromFile(assemblyLocation);
            }
            catch
            {
                assemblyLocation = null;
                return null;
            }
        }
    }
}
