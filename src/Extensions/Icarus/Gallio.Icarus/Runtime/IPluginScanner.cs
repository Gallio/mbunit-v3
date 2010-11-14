using System.Reflection;

namespace Gallio.Icarus.Runtime
{
    public interface IPluginScanner
    {
        void Scan(string pluginId, Assembly assembly);
    }
}