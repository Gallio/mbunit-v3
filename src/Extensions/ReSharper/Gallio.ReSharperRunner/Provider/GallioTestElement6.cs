// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Xml;
using Gallio.Common;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Schema;
using Gallio.ReSharperRunner.Provider.Facade;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace Gallio.ReSharperRunner.Provider
{
    /// <summary>
    /// Represents a Gallio test.
    /// </summary>
    public class GallioTestElement : IUnitTestElement, IEquatable<GallioTestElement>, IComparable<GallioTestElement>, IComparable
    {
        private readonly string testName;
        private readonly GallioTestProvider provider;
        private readonly bool isTestCase;

        private readonly IProject project;
        private readonly IDeclaredElementResolver declaredElementResolver;

        private readonly string assemblyPath;
        private readonly string typeName;
        private readonly string namespaceName;
        private Memoizer<string> shortNameMemoizer = new Memoizer<string>();

        private GallioTestElement(GallioTestProvider provider, IUnitTestElement parent, string testId, string testName, string kind, bool isTestCase,
            IProject project, IDeclaredElementResolver declaredElementResolver, string assemblyPath, string typeName, string namespaceName)
        {
            this.provider = provider;
            Id = testId;
            TestId = testId;
            this.testName = testName;
            
            Parent = parent;
            if (Parent != null)
                parent.Children.Add(this);
            
            Kind = kind;
            this.isTestCase = isTestCase;
            this.project = project;
            this.declaredElementResolver = declaredElementResolver;
            this.assemblyPath = assemblyPath;
            this.typeName = typeName;
            this.namespaceName = namespaceName;
            Children = new List<IUnitTestElement>();
        }

        public string TestId { get; private set; }

        public static GallioTestElement CreateFromTest(TestData test, ICodeElementInfo codeElement, GallioTestProvider provider, GallioTestElement parent)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            // The idea here is to generate a test element object that does not retain any direct
            // references to the code element info and other heavyweight objects.  A test element may
            // survive in memory for quite a long time so we don't want it holding on to all sorts of
            // irrelevant stuff.  Basically we flatten out the test to just those properties that we
            // need to keep.
            var element = new GallioTestElement(provider, parent,
                test.Id,
                test.Name,
                test.Metadata.GetValue(MetadataKeys.TestKind) ?? "Unknown",
                test.IsTestCase,
                ReSharperReflectionPolicy.GetProject(codeElement),
                ReSharperReflectionPolicy.GetDeclaredElementResolver(codeElement),
                GetAssemblyPath(codeElement),
                GetTypeName(codeElement),
                GetNamespaceName(codeElement));

            var categories = test.Metadata[MetadataKeys.Category];
            element.Categories = UnitTestElementCategory.Create(categories);

            var reason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
            if (reason != null)
            {
                element.Explicit = true;
                element.ExplicitReason = reason;
            }

            return element;
        }

        private static string GetAssemblyPath(ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
            return assembly != null ? assembly.Path : null;
        }

        private static string GetTypeName(ICodeElementInfo codeElement)
        {
            ITypeInfo type = ReflectionUtils.GetType(codeElement);
            return type != null ? type.FullName : "";
        }

        private static string GetNamespaceName(ICodeElementInfo codeElement)
        {
            INamespaceInfo @namespace = ReflectionUtils.GetNamespace(codeElement);
            return @namespace != null ? @namespace.Name : "";
        }

        public string GetAssemblyLocation()
        {
            return assemblyPath;
        }

        public string TestName
        {
            get { return testName; }
        }

        public ICollection<IUnitTestElement> Children { get; private set; }

        // R# uses this name as a filter for declared elements so that it can quickly determine
        // whether a given declared element is likely to be a test before asking the provider about
        // it.  The result must be equal to IDeclaredElement.ShortName.
        public string ShortName
        {
            get
            {
                return shortNameMemoizer.Memoize(() =>
                {
                    IDeclaredElement declaredElement = GetDeclaredElement();
                    return declaredElement != null && declaredElement.IsValid()
                        ? declaredElement.ShortName
                        : testName;
                });
            }
        }

        public bool Explicit { get; private set; }

        public UnitTestElementState State { get; set; }

        public bool IsTestCase
        {
            get { return isTestCase; }
        }

        private string GetTitle()
        {
            return testName;
        }

        public string GetTypeClrName()
        {
            return typeName;
        }

        public string GetPresentation()
        {
            return "";
        }

#if RESHARPER_70
        public string GetPresentation(IUnitTestElement parent = null)
        {
            throw new NotImplementedException();
        }
#endif

        public UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(namespaceName);
        }

        public IProject GetProject()
        {
            return project;
        }

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            ITypeElement declaredType = GetDeclaredType();
            if (declaredType == null)
            {
                return EmptyArray<IProjectFile>.Instance;
            }

            return declaredType.GetSourceFiles().Select(x => x.ToProjectFile());
        }

        private ITypeElement GetDeclaredType()
        {
            var psiModule = provider.PsiModuleManager.GetPrimaryPsiModule(project);
            if (psiModule == null)
            {
                return null;
            }

            var declarationsCache = provider.CacheManager.GetDeclarationsCache(psiModule, true, true);
            return declarationsCache.GetTypeElementByCLRName(typeName);
        }

#if RESHARPER_60
        public IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements)
#elif RESHARPER_61
        public IList<UnitTestTask> GetTaskSequence(IList<IUnitTestElement> explicitElements)
#else
        public IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch launch)
#endif
        {
            // Add the run task.  Must always be first.
            var tasks = new List<UnitTestTask> { new UnitTestTask(null, FacadeTaskFactory.CreateRootTask()) };

            // Add the test case branch.
            AddTestTasksFromRootToLeaf(tasks, this);

            // Now that we're done with the critical parts of the task tree, we can add other
            // arbitrary elements.  We don't care about the structure of the task tree beyond this depth.

            // Add the assembly location.
            tasks.Add(new UnitTestTask(null, new AssemblyLoadTask(GetAssemblyLocation())));
            tasks.Add(new UnitTestTask(null, FacadeTaskFactory.CreateAssemblyTaskFrom(this)));

            if (explicitElements.Count() != 0)
            {
                // Add explicit element markers.
                foreach (GallioTestElement explicitElement in explicitElements)
                    tasks.Add(new UnitTestTask(null, FacadeTaskFactory.CreateExplicitTestTaskFrom(explicitElement)));
            }
            else
            {
                // No explicit elements but we must have at least one to filter by, so we consider
                // the top test explicitly selected.
                tasks.Add(new UnitTestTask(null, FacadeTaskFactory.CreateExplicitTestTaskFrom(this)));
            }

            return tasks;
        }

        private void AddTestTasksFromRootToLeaf(ICollection<UnitTestTask> tasks, GallioTestElement element)
        {
            var parentElement = element.Parent as GallioTestElement;
            if (parentElement != null)
                AddTestTasksFromRootToLeaf(tasks, parentElement);

            tasks.Add(new UnitTestTask(element, FacadeTaskFactory.CreateTestTaskFrom(element)));
        }

        public string Kind { get; private set; }

        public IEnumerable<UnitTestElementCategory> Categories { get; private set; }

        public string ExplicitReason { get; private set; }

        public string Id { get; private set; }

        public IUnitTestProvider Provider
        {
            get { return provider; }
        }

        public IUnitTestElement Parent { get; set; }

        public UnitTestElementDisposition GetDisposition()
        {
            var element = GetDeclaredElement();
            if (element == null || !element.IsValid())
            {
                return UnitTestElementDisposition.InvalidDisposition;
            }

            var locations = new List<UnitTestElementLocation>();
            element.GetDeclarations().ForEach(declaration =>
            {
                var file = declaration.GetContainingFile();
                if (file != null)
                {
                    var projectFile = file.GetSourceFile().ToProjectFile();
                    var navigationRange = declaration.GetNameDocumentRange().TextRange;
                    var containingRange = declaration.GetDocumentRange().TextRange;
                    locations.Add(new UnitTestElementLocation(projectFile, navigationRange, containingRange));
                }
            });

            return new UnitTestElementDisposition(locations, this);
        }

        public IDeclaredElement GetDeclaredElement()
        {
            return declaredElementResolver.ResolveDeclaredElement();
        }

        public bool Equals(GallioTestElement other)
        {
            return other != null && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GallioTestElement);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(GallioTestElement other)
        {
            // Find common ancestor.
            var branches = new Dictionary<GallioTestElement, GallioTestElement>();
            for (GallioTestElement thisAncestor = this, thisBranch = null;
                thisAncestor != null;
                thisBranch = thisAncestor, thisAncestor = thisAncestor.Parent as GallioTestElement)
                branches.Add(thisAncestor, thisBranch);

            for (GallioTestElement otherAncestor = other, otherBranch = null;
                otherAncestor != null;
                otherBranch = otherAncestor, otherAncestor = otherAncestor.Parent as GallioTestElement)
            {
                GallioTestElement thisBranch;
                if (!branches.TryGetValue(otherAncestor, out thisBranch)) 
                    continue;

                // Compare the relative ordering of the branches leading from
                // the common ancestor to each child.
                var children = new List<IUnitTestElement>(otherAncestor.Children);
                var thisOrder = thisBranch != null ? children.IndexOf(thisBranch) : -1;
                var otherOrder = otherBranch != null ? children.IndexOf(otherBranch) : -1;

                return thisOrder.CompareTo(otherOrder);
            }

            // No common ancestor, compare ids.
            return Id.CompareTo(other.Id);
        }

        public int CompareTo(object obj)
        {
            var other = obj as GallioTestElement;
            return other != null ? CompareTo(other) : 1; // sort gallio test elements after all other kinds
        }

        public bool Equals(IUnitTestElement other)
        {
            return Equals(other as GallioTestElement);
        }

        public override string ToString()
        {
            return GetTitle();
        }

        public void Serialize(XmlElement parent)
        {
            parent.SetAttribute("projectId", GetProject().GetPersistentID());
            parent.SetAttribute("testId", TestId);
        }

        public static IUnitTestElement Deserialize(XmlElement parent, IUnitTestElement parentElement, GallioTestProvider provider)
        {
			//var projectId = parent.GetAttribute("projectId");
			//var project = ProjectUtil.FindProjectElementByPersistentID(provider.Solution, projectId) as IProject;
			//if (project == null)
			//{
			//    return null;
			//}

            //var testId = parent.GetAttribute("testId");
            //var element = provider.UnitTestManager.GetElementById(project, testId) as GallioTestElement;
            //if (element != null)
            //{
            //    element.Parent = parentElement;
            //    element.State = UnitTestElementState.Valid;
            //    return element;
            //}

            return null;
        }
    }
}