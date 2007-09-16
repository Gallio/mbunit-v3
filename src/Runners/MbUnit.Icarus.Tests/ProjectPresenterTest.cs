// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MbUnit.Framework;
using MbUnit.Icarus.Adapter;
using MbUnit.Icarus.AdapterModel;
using MbUnit.Icarus.Core.Model;
using MbUnit.Icarus.Core.Presenter;

namespace MbUnit.Icarus.Tests
{
    [TestFixture]
    public class ProjectPresenterTest
    {
        [Test]
        public void GetTestTreeTest()
        {
           
           Main main = new Main();

           ProjectAdapter pa = new ProjectAdapter(main, new ProjectAdapterModel());
           ProjectPresenter p = new ProjectPresenter(pa, new TestRunnerModel());

           //"C:\\Source\\MbUnitGoogle\\mb-unit\\v3\\src\\TestResources\\MbUnit.TestResources.MbUnit2\\bin\\MbUnit.TestResources.MbUnit2.dll"
            //main.GetTheTestTree();


          
        }
    }
}
