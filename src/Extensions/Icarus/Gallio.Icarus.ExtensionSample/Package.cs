using System;

namespace Gallio.Icarus.ExtensionSample
{
    public class MyPackage : IPackage
    {
        public void Dispose()
        {
            
        }

        public void Load()
        {
            Console.WriteLine("Hello!");
        }
    }
}
