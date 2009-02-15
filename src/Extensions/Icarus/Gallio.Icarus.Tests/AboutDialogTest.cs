// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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

using System.Collections.Generic;
using Gallio.Icarus.Controllers.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    class AboutDialogTest : MockTest
    {
        [Test]
        public void Constructor_Test()
        {
            ITestController testController = mocks.StrictMock<ITestController>();
            Expect.Call(testController.TestFrameworks).Return(new List<string>(new[] {"test"}));
            mocks.ReplayAll();
            AboutDialog aboutDialog = new AboutDialog(testController);
        }
    }
}
