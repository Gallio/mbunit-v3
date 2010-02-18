using System.Collections.Generic;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.Search
{
    public interface IModel
    {
        Observable<IList<string>> Metadata { get; }
    }
}