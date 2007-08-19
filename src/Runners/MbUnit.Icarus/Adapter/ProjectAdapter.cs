using System;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Icarus.Core.Interfaces;
using MbUnit.Icarus.Interfaces;

namespace MbUnit.Icarus.Adapter
{
    public class ProjectAdapter : IProjectAdapter
    {

        private IProjectAdapterView _View;
        private IProjectAdapterModel _Model;
        private TestModel testCollection;
   
        public ProjectAdapter(IProjectAdapterView view, IProjectAdapterModel model)
        {
            _View = view;
            _Model = model;
        }

        event EventHandler<EventArgs> IProjectAdapter.GetTestTree
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public TestModel TestCollection
        {
            set { testCollection = value; }
        }

        public void DataBind()
        {
            throw new NotImplementedException();
        }
    }
}
