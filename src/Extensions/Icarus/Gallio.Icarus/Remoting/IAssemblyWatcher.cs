using System.Collections.Generic;

namespace Gallio.Icarus.Remoting
{
    public interface IAssemblyWatcher
    {
        event AssemblyWatcher.AssemblyChangedHandler AssemblyChangedEvent;
        void Add(IList<string> files);
        void Add(string filePath);
        void Remove(string filePath);
        void Clear();
        void Start();
        void Stop();
    }
}