// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.ReSharperRunner.Hosting;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.Shell;
using JetBrains.UI.TreeView;
using JetBrains.Util.DataStructures.TreeModel;

namespace Gallio.ReSharperRunner
{
    /// <summary>
    /// This is the main entry point into the Gallio test runner for ReSharper.
    /// </summary>
    /// <remarks>
    /// The implementation of this class must not use Gallio types since
    /// it is possible that those types could not be loaded.  Service location
    /// should be performed via the <see cref="RuntimeProxy"/>.
    /// </remarks>
    [UnitTestProvider]
    public class UnitTestProviderProxy : IUnitTestProvider
    {
        private const string ProviderId = "Gallio";

        private IUnitTestProviderDelegate @delegate;

        /// <summary>
        /// Explores the "world", i.e. retrieves tests not associated with current solution.
        /// </summary>
        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
            if (consumer == null)
                throw new ArgumentNullException("consumer");

            Delegate.ExploreExternal(consumer);
        }

        /// <summary>
        /// Explores given solution, but not containing projects.
        /// </summary>
        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");
            if (consumer == null)
                throw new ArgumentNullException("consumer");

            Delegate.ExploreSolution(solution, consumer);
        }

        /// <summary>
        /// Explores given compiled project.
        /// </summary>
        public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (project == null)
                throw new ArgumentNullException("project");
            if (consumer == null)
                throw new ArgumentNullException("consumer");

            Delegate.ExploreAssembly(assembly, project, consumer);
        }

        /// <summary>
        /// Explores given PSI file.
        /// </summary>
        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            if (psiFile == null)
                throw new ArgumentNullException("psiFile");
            if (consumer == null)
                throw new ArgumentNullException("consumer");

            Delegate.ExploreFile(psiFile, consumer, interrupted);
        }

        /// <summary>
        /// Checks if given declared element is UnitTestElement.
        /// </summary>
        public bool IsUnitTestElement(IDeclaredElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return Delegate.IsUnitTestElement(element);
        }

        /// <summary>
        /// Present unit test.
        ///</summary>
        public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (item == null)
                throw new ArgumentNullException("item");
            if (node == null)
                throw new ArgumentNullException("node");
            if (state == null)
                throw new ArgumentNullException("state");

            Delegate.Present(element, item, node, state);
        }

        /// <summary>
        /// Gets instance of <see cref="T:JetBrains.ReSharper.TaskRunnerFramework.RemoteTaskRunnerInfo" />.
        /// </summary>
        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return Delegate.GetTaskRunnerInfo();
        }

        /// <summary>
        /// Serializes element for persistence.
        /// </summary>
        public string Serialize(UnitTestElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return null;
        }

        /// <summary>
        /// Deserializes element from persisted string.
        /// </summary>
        public UnitTestElement Deserialize(ISolution solution, string elementString)
        {
            if (solution == null)
                throw new ArgumentNullException("solution");
            if (elementString == null)
                throw new ArgumentNullException("elementString");

            return null;
        }

        /// <summary>
        /// Gets task information for <see cref="T:JetBrains.ReSharper.TaskRunnerFramework.RemoteTaskRunner" /> from element.
        /// </summary>
        public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (explicitElements == null)
                throw new ArgumentNullException("explicitElements");

            return Delegate.GetTaskSequence(element, explicitElements);
        }

        /// <summary>
        /// Compares unit tests elements to determine relative sort order.
        /// </summary>
        public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
        {
            if (x == null)
                throw new ArgumentNullException("x");
            if (y == null)
                throw new ArgumentNullException("y");

            return Delegate.CompareUnitTestElements(x, y);
        }

        /// <summary>
        /// Obtains configuration data.
        /// </summary>
        public void ProfferConfiguration(TaskExecutorConfiguration configuration, UnitTestSession session)
        {
        }

        /// <summary>
        /// Gets the ID of the provider.
        /// </summary>
        public string ID
        {
            get { return ProviderId; }
        }

        private IUnitTestProviderDelegate Delegate
        {
            get
            {
                if (@delegate == null)
                {
                    @delegate = RuntimeProxy.Resolve<IUnitTestProviderDelegate>();
                    @delegate.SetProvider(this);
                }

                return @delegate;
            }
        }
    }
}