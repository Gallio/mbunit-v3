using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model;

namespace Gallio.RSpecAdapter.Model
{
    public class RSpecTestFramework : BaseTestFramework
    {
        public override void RegisterTestExplorers(IList<ITestExplorer> explorers)
        {
            base.RegisterTestExplorers(explorers);
        }
    }
}
