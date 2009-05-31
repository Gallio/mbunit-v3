using System.Collections.Generic;

namespace Gallio.Copy
{
    internal interface ICopyController
    {
        IList<string> Plugins { get; }
        void CopyTo(string destinationFolder, IList<string> plugins);
    }
}