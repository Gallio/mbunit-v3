using System;

namespace Gallio.Icarus.Options
{
    public interface IOptionsManager
    {
        void Load();
        void Save();
        Settings Settings { get; }
    }
}
