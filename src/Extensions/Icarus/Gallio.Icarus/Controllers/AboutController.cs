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
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Model;
using System.Windows.Forms;

namespace Gallio.Icarus.Controllers
{
    internal class AboutController : IAboutController
    {
        private readonly ITestFrameworkManager testFrameworkManager;

        public IList<TestFrameworkTraits> TestFrameworks
        {
            get
            {
                var frameworks = new List<TestFrameworkTraits>();

                foreach (var frameworkHandle in testFrameworkManager.FrameworkHandles)
                    frameworks.Add(frameworkHandle.GetTraits());

                return frameworks;
            }
        }

        public string Version
        {
            get
            {
                Version appVersion = AssemblyUtils.GetApplicationVersion(Assembly.GetExecutingAssembly());
                return String.Format(CultureInfo.CurrentCulture, "Gallio Icarus - Version {0}.{1}.{2} build {3}",
                    appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);
            }
        }

        public AboutController(ITestFrameworkManager testFrameworkManager)
        {
            if (testFrameworkManager == null) 
                throw new ArgumentNullException("testFrameworkManager");

            this.testFrameworkManager = testFrameworkManager;
        }
    }
}
