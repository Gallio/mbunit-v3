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

using System;
using Gallio.Icarus.Controllers;
using Gallio.Model;
using Gallio.Runtime;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(AboutController))]
    internal class AboutControllerTest
    {
        [Test]
        public void TestFrameworks_should_come_from_TestFrameworkManager()
        {
            var testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
            var aboutController = new AboutController(testFrameworkManager);

            Assert.AreEqual(testFrameworkManager.FrameworkHandles.Count, aboutController.TestFrameworks.Count);
        }

        [Test]
        public void Version_string_()
        {
            var testFrameworkManager = MockRepository.GenerateStub<ITestFrameworkManager>();
            var aboutController = new AboutController(testFrameworkManager);

            Assert.StartsWith(aboutController.Version, "Gallio Icarus - Version");
        }

        [Test]
        public void Ctor_should_throw_if_TestFrameworkManager_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new AboutController(null));
        }
    }
}
