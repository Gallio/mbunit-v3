// Copyright 2005-2011 Gallio Project - http://www.gallio.org/
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

using Gallio.ReSharperRunner.Provider.Tasks;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    public static class FacadeTaskFactory
    {
        public static RemoteTask CreateRootTask()
        {
            return new FacadeTaskWrapper(GallioTestRunTask.Instance);
        }

        public static RemoteTask CreateAssemblyTaskFrom(GallioTestElement element)
        {
#if RESHARPER_50_OR_NEWER
            var assemblyTask = new GallioTestAssemblyTask(element.GetAssemblyLocation(), 
                element.GetTypeClrName(), element.ShortName);
#else
            var assemblyTask = new GallioTestAssemblyTask(element.GetAssemblyLocation(),
                element.GetTypeClrName(), "");
#endif

#if RESHARPER_51
            return new TestContainerFacadeTaskWrapper(assemblyTask);
#else
            return new FacadeTaskWrapper(assemblyTask);
#endif
        }

        public static RemoteTask CreateExplicitTestTaskFrom(GallioTestElement element)
        {
#if RESHARPER_50_OR_NEWER
            var explicitTask = new GallioTestExplicitTask(element.TestId, element.GetTypeClrName(), 
                element.ShortName);
#else
            var explicitTask = new GallioTestExplicitTask(element.TestId, element.GetTypeClrName(), "");
#endif
            return CreateTestTask(element, explicitTask);
        }

        public static RemoteTask CreateTestTaskFrom(GallioTestElement element)
        {
#if RESHARPER_50_OR_NEWER
            var testItemTask = new GallioTestItemTask(element.TestId, element.GetTypeClrName(),
                element.ShortName);
#else
            var testItemTask = new GallioTestItemTask(element.TestId, element.GetTypeClrName(), "");
#endif
            return CreateTestTask(element, testItemTask);
        }

        private static RemoteTask CreateTestTask(GallioTestElement element, FacadeTask facadeTask)
        {
#if RESHARPER_51
            if (element.IsTestCase)
            {
                return new UnitTestFacadeTaskWrapper(facadeTask);
            }
            return new TestContainerFacadeTaskWrapper(facadeTask);
#else
            return new FacadeTaskWrapper(facadeTask);
#endif
        }
    }
}