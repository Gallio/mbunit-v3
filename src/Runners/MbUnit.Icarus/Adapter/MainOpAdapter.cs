using MbUnit.Icarus.Core.Interfaces;
using MbUnit.Icarus.Interfaces;

namespace MbUnit.Icarus.Adapter
{
    public class MainOpAdapter : IMainOpAdapter
    {

        private IMainOpAdapterView _View;
        private IMainOpAdapterModel _Model;

        public MainOpAdapter(IMainOpAdapterView view, IMainOpAdapterModel model)
        {
            _View = view;
            _Model = model;
        }

    }
}
