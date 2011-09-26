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
using Gallio.Common;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Schema;
using Gallio.ReSharperRunner.Provider.Facade;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework.UnitTesting;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Provider.TestElements
{
    /// <summary>
    /// Represents a Gallio test.
    /// </summary>
    public class GallioTestElement : GallioTestElementBase, IUnitTestViewElement
    {
        private readonly string testName;
    	private readonly string testId;
        private readonly string kind;
        private readonly bool isTestCase;

        private readonly IProject project;
        private readonly IDeclaredElementResolver declaredElementResolver;

        private readonly string assemblyPath;
        private readonly string typeName;
        private readonly string namespaceName;

        private Memoizer<string> shortNameMemoizer;

    	protected GallioTestElement(IUnitTestProvider provider, GallioTestElement parent, string testId, string testName, 
			string kind, bool isTestCase, IProject project, IDeclaredElementResolver declaredElementResolver, 
			string assemblyPath, string typeName, string namespaceName)
			: base(provider, testId, parent)
        {
    		this.testId = testId;
            this.testName = testName;
            this.kind = kind;
            this.isTestCase = isTestCase;
            this.project = project;
            this.declaredElementResolver = declaredElementResolver;
            this.assemblyPath = assemblyPath;
            this.typeName = typeName;
            this.namespaceName = namespaceName;
        }

        public static GallioTestElement CreateFromTest(TestData test, ICodeElementInfo codeElement, IUnitTestProvider provider, 
			GallioTestElement parent)
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
            if (categories.Count != 0)
                element.Categories = UnitTestElementCategory.Create(categories);

            var reason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
            if (reason != null)
                element.ExplicitReason = reason;

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

    	public override void Invalidate()
    	{
    		Valid = false;
    	}

		public override IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements)
		{
			IList<UnitTestTask> testTasks;

			if (Parent != null)
			{
				testTasks = Parent.GetTaskSequence(explicitElements);
				testTasks.Add(new UnitTestTask(this, FacadeTaskFactory.CreateTestTaskFrom(this)));
			}
			else
			{
				testTasks = new List<UnitTestTask>();
				// Add the run task.  Must always be first.
				testTasks.Add(new UnitTestTask(null, FacadeTaskFactory.CreateRootTask()));
				testTasks.Add(new UnitTestTask(null, new AssemblyLoadTask(GetAssemblyLocation())));

				if (explicitElements != null && explicitElements.Count() != 0)
				{
					// Add explicit element markers.
					foreach (GallioTestElement explicitElement in explicitElements)
						testTasks.Add(new UnitTestTask(null, FacadeTaskFactory.CreateExplicitTestTaskFrom(explicitElement)));
				}
				else
				{
					// No explicit elements but we must have at least one to filter by, so we consider
					// the top test explicitly selected.
					testTasks.Add(new UnitTestTask(null, FacadeTaskFactory.CreateExplicitTestTaskFrom(this)));
				}
			}
			return testTasks;
		}

    	// R# uses this name as a filter for declared elements so that it can quickly determine
		// whether a given declared element is likely to be a test before asking the provider about
		// it.  The result must be equal to IDeclaredElement.ShortName.
    	public override string ShortName
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

    	public override bool Valid { get; set; }

    	public string TestId
        {
            get { return testId; }
        }

        public bool IsTestCase
        {
            get { return isTestCase; }
        }

        public string GetTitle()
        {
            return testName;
        }

        public string GetTypeClrName()
        {
            return typeName;
        }

        public UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(namespaceName);
        }

        public IProject GetProject()
        {
            return project;
        }

        public IDeclaredElement GetDeclaredElement()
        {
            return declaredElementResolver.ResolveDeclaredElement();
        }

        public bool Equals(GallioTestElement other)
        {
            return other != null && testId == other.testId;
        }

		public override bool Equals(IUnitTestElement other)
		{
			return Equals(other as GallioTestElement);
		}

		public bool Equals(IUnitTestViewElement other)
		{
			return Equals(other as GallioTestElement);
		}

        public override bool Equals(object obj)
        {
            return Equals(obj as GallioTestElement);
        }

        public override int GetHashCode()
        {
            return testId.GetHashCode();
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
                if (branches.TryGetValue(otherAncestor, out thisBranch))
                {
                    // Compare the relative ordering of the branches leading from
                    // the common ancestor to each child.
                    int thisOrder = thisBranch != null ? otherAncestor.children.IndexOf(thisBranch) : -1;
                    int otherOrder = otherBranch != null ? otherAncestor.children.IndexOf(otherBranch) : -1;

                    return thisOrder.CompareTo(otherOrder);
                }
            }

            // No common ancestor, compare test ids.
            return testId.CompareTo(other.testId);
        }

        public int CompareTo(object obj)
        {
            GallioTestElement other = obj as GallioTestElement;
            return other != null ? CompareTo(other) : 1; // sort gallio test elements after all other kinds
        }

    	public override string ToString()
        {
            return GetTitle();
        }

		public UnitTestElementDisposition GetDisposition()
		{
			var element = GetDeclaredElement();
			if (element == null || !element.IsValid())
				return UnitTestElementDisposition.InvalidDisposition;
			
			var locations = new List<UnitTestElementLocation>();
			foreach (var declaration in element.GetDeclarations())
			{
				var file = declaration.GetContainingFile();
				if (file != null)
				{
					locations.Add(new UnitTestElementLocation(file.GetSourceFile().ToProjectFile(), 
						declaration.GetNameDocumentRange().TextRange, 
						declaration.GetDocumentRange().TextRange));
				}
			}
			return new UnitTestElementDisposition(locations, this);
		}

		public string Kind
		{
			get { return kind; }
		}
	}
}
