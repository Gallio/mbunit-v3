using System;

namespace Gallio.Icarus
{
    /// <summary>
    /// Shameless rip-off of Reflector add-in API.
    /// </summary>
    public interface IPackage
    {
        void Load(IServiceProvider serviceProvider);
        void Unload();
    }
}
