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
using System.IO;
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Reflection;
using JetBrains.ProjectModel;
using JetBrains.UI;
using JetBrains.Util;
using MbUnit.Framework;
using Action = Gallio.Common.Action;

#if RESHARPER_31
using JetBrains.Shell;
using JetBrains.Shell.Progress;
using JetBrains.Shell.Test;
#else
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.Application.Test;
#if RESHARPER_40 || RESHARPER_41
using Resources;
#endif
#endif
using Gallio.Common.Diagnostics;

namespace Gallio.ReSharperRunner.Tests
{
    public static class GallioTestShellHandler
    {
        public static readonly Assembly TestAssembly = typeof(ReSharperTestHarness).Assembly;
        public static readonly string VersionSuffix = typeof(ReSharperTestHarness).Assembly.GetName().Name.Substring(22, 2);

        // We need to use our own configuration because the standard JetBrains.resources.config.AllAssemblies.xml
        // resource in JetBrains.ReSharper.Resources refers to actual testing assemblies that JetBrains does not
        // publish.  This is problematic because it causes the TestShell to fail to initialize.
        //
        // Our configuration is basically a stripped down version of the standard one. -- Jeff.
        public static readonly string ConfigName = typeof(ReSharperTestHarness).Namespace + ".TestAssemblies" + VersionSuffix + ".xml";

        public static void Initialize()
        {
            if (!IsShellInitialized)
            {
                RunWithWriteLock(() =>
                {
#if ! RESHARPER_50_OR_NEWER
                    new GallioTestShell();
#else
                    TestsApplicationDescriptor desc = new TestsApplicationDescriptor(TestAssembly, ConfigName);
                    desc.StartShell(TestAssembly);
#endif
                });
            }
        }

        public static void ShutDown()
        {
            if (IsShellInitialized)
            {
                RunWithWriteLock(() =>
                {
                    TestShell shell = Shell.Instance as TestShell;
                    if (shell != null)
                    {
                        shell.TearDown();
                        shell.Dispose();
                    }
                });
            }
        }

        public static void RunWithWriteLock(Action action)
        {
            Exception actionException = null;

#if !RESHARPER_31
            TestShell.RunGuarded(delegate
            {
                using (WriteLockCookie.Create())
                {
#endif
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        actionException = ex;
                    }
#if !RESHARPER_31
                }
            });
#endif

            if (actionException != null)
                ExceptionUtils.RethrowWithNoStackTraceLoss(actionException);
        }

        public static bool IsShellInitialized
        {
            get
            {
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
                return Shell.Instance != null;
#else
                return Shell.HasInstance;
#endif
            }
        }
    }
}
