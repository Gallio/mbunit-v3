using System.Collections.Generic;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.Search
{
    public class Model : IModel
    {
        public Observable<IList<string>> Metadata { get; private set; }

        public Model()
        {
            Metadata = new Observable<IList<string>>(new List<string>());
        }
    }
}
