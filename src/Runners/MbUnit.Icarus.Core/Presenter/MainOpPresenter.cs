using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Icarus.Core.Interfaces;

namespace MbUnit.Icarus.Core.Presenter
{
    public class MainOpPresenter
    {
        private IMainOpAdapter _View;
        private IMainOpModel _Model;

        public MainOpPresenter(IMainOpAdapter view, IMainOpModel model)
        {
            _View = view;
            _Model = model;
        }

    }
}
