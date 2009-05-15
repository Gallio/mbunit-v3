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
