using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runner.Extensions;

namespace Gallio.TeamCityIntegration
{
    public class TeamCityExtensionFactory : ITestRunnerExtensionFactory
    {
        public ITestRunnerExtension CreateExtension()
        {
            return new TeamCityExtension();
        }
    }
}
