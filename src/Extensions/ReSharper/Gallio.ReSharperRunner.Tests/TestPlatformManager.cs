using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Application;
using JetBrains.ComponentModel;
using JetBrains.ProjectModel.Platforms;
using JetBrains.ProjectModel.Platforms.Impl;

namespace Gallio.ReSharperRunner.Tests
{
    [ShellComponentImplementation(ProgramConfigurations.TEST)]
    public class TestPlatformManager : PlatformManagerTestImpl
    {
        // HACK to find installed platforms instead of using the built-in test
        // behavior of searching a framework snapshot.
        protected override void SearchForPlatforms(IList<PlatformInfo> platforms)
        {
            PlatformManagerImpl impl = new PlatformManagerImpl();
            typeof(PlatformManagerImpl).GetMethod("SearchForPlatforms",
                BindingFlags.Instance | BindingFlags.NonPublic).Invoke(impl, new object[] { platforms });
        }
    }
}
