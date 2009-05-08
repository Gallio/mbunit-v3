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
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;

namespace Gallio.ReSharperRunner.Provider
{
    /// <summary>
    /// Represents a Gallio test.
    /// </summary>
    public class GallioTestElement : UnitTestElement, IEquatable<GallioTestElement>, IComparable<GallioTestElement>, IComparable
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

        private GallioTestElement(IUnitTestProvider provider, GallioTestElement parent, string testId, string testName, string kind, bool isTestCase,
            IProject project, IDeclaredElementResolver declaredElementResolver, string assemblyPath, string typeName, string namespaceName)
            : base(provider, parent)
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

        public static GallioTestElement CreateFromTest(ITest test, IUnitTestProvider provider, GallioTestElement parent)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            // The idea here is to generate a test element object that does not retain any direct
            // references to ITest and other heavyweight objects.  A test element may survive in memory
            // for quite a long time so we don't want it holding on to all sorts of irrelevant stuff.
            // Basically we flatten out the ITest to just those properties that we need to keep.
            ICodeElementInfo codeElement = test.CodeElement;
            GallioTestElement element = new GallioTestElement(provider, parent,
                test.Id,
                test.Name,
                test.Metadata.GetValue(MetadataKeys.TestKind) ?? "Unknown",
                test.IsTestCase,
                ReSharperReflectionPolicy.GetProject(codeElement),
                ReSharperReflectionPolicy.GetDeclaredElementResolver(codeElement),
                GetAssemblyPath(codeElement),
                GetTypeName(codeElement),
                GetNamespaceName(codeElement));

            IList<string> categories = test.Metadata[MetadataKeys.Category];
            if (categories.Count != 0)
                element.AssignCategories(categories);

            string reason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
            if (reason != null)
                element.SetExplicit(reason);

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

        public string TestId
        {
            get { return testId; }
        }

        public bool IsTestCase
        {
            get { return isTestCase; }
        }

        public override string GetTitle()
        {
            return testName;
        }

        public override string GetTypeClrName()
        {
            return typeName;
        }

        public override UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(namespaceName);
        }

        public override IProject GetProject()
        {
            return project;
        }

        public override string GetKind()
        {
            return kind;
        }

#if RESHARPER_31
        public override IList<IProjectItem> GetProjectItems()
        {
            IDeclaredElement declaredElement = GetDeclaredElement();

            if (declaredElement != null && declaredElement.IsValid())
                return declaredElement.GetProjectFiles();

            return EmptyArrays.ProjectItems;
        }
#else
        public override IList<IProjectFile> GetProjectFiles()
        {
            IDeclaredElement declaredElement = GetDeclaredElement();

            if (declaredElement != null && declaredElement.IsValid())
                return declaredElement.GetProjectFiles();

            return EmptyArray<IProjectFile>.Instance;
        }
#endif

        public override UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = GetDeclaredElement();
            if (element == null || !element.IsValid())
            {
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
                return UnitTestElementDisposition.ourInvalidDisposition;
#else
                return UnitTestElementDisposition.InvalidDisposition;
#endif
            }

            List<UnitTestElementLocation> locations = new List<UnitTestElementLocation>();

            foreach (IDeclaration declaration in element.GetDeclarations())
            {
                IFile file = declaration.GetContainingFile();

                if (file != null)
                {
                    locations.Add(new UnitTestElementLocation(
#if RESHARPER_31
                        file.ProjectItem,
#else
                        file.ProjectFile,
#endif
                        declaration.GetNameRange(), declaration.GetDocumentRange().TextRange));
                }
            }

            return new UnitTestElementDisposition(locations, this);
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return declaredElementResolver.ResolveDeclaredElement();
        }

#if RESHARPER_31
        public override bool Matches(string filter)
        {
            if (testName.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0)
                return true;

            GallioTestElement parent = Parent as GallioTestElement;
            return parent != null && parent.Matches(filter);
        }
#endif

        public bool Equals(GallioTestElement other)
        {
            return other != null && testId == other.testId;
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
            Dictionary<GallioTestElement, GallioTestElement> branches = new Dictionary<GallioTestElement, GallioTestElement>();
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
                    int thisOrder = thisBranch != null ? otherAncestor.Children.IndexOf(thisBranch) : -1;
                    int otherOrder = otherBranch != null ? otherAncestor.Children.IndexOf(otherBranch) : -1;

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
    }
}