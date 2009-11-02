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
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Application;
using JetBrains.ComponentModel;
using JetBrains.ProjectModel.Platforms.Impl;
#if ! RESHARPER_50_OR_NEWER
using JetBrains.ProjectModel.Platforms;
#else
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.impl;
using Gallio.Common.Collections;
#endif

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
#if RESHARPER_50_OR_NEWER
            typeof(PlatformManagerImpl).GetField("myAllPlatforms",
                BindingFlags.Instance | BindingFlags.NonPublic).SetValue(impl, new List<PlatformInfo>());
#endif
            typeof(PlatformManagerImpl).GetMethod("SearchForPlatforms",
                BindingFlags.Instance | BindingFlags.NonPublic).Invoke(impl, new object[] { platforms });
        }
    }
}
