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

using System.ComponentModel;
using System.Threading;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Utilities;
using MbUnit.Framework;
using Gallio.Icarus.Tests.Utilities;

namespace Gallio.Icarus.Tests.Controllers
{
    internal class NotifyControllerTest
    {
        private class TestController : NotifyController
        {
            public void Notify(string propertyName)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        [Test]
        public void OnPropertyChanged_without_synchronization_context()
        {
            var testController = new TestController();
            bool propertyChangedFlag = false;
            testController.PropertyChanged += delegate { propertyChangedFlag = true; };

            testController.Notify("test");

            Assert.AreEqual(false, propertyChangedFlag);
        }

        [Test]
        public void OnPropertyChanged_with_synchronization_context()
        {
            var testController = new TestController();
            bool propertyChangedFlag = false;
            string propertyName = "test";
            testController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == propertyName)
                    propertyChangedFlag = true;
            };
            testController.SynchronizationContext = new TestSynchronizationContext();

            testController.Notify(propertyName);

            Assert.AreEqual(true, propertyChangedFlag);
        }
    }
}
