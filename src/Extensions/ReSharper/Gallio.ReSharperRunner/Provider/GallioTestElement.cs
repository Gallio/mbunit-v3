// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Collections;
using Gallio.Model;
using Gallio.Reflection;
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
        private readonly ITest test;

        public GallioTestElement(ITest test, IUnitTestProvider provider, UnitTestElement parent)
            : base(provider, parent)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (provider == null)
                throw new ArgumentNullException("provider");

            this.test = test;

            PopulateMetadata();
        }

        public string GetAssemblyLocation()
        {
            IAssemblyInfo assembly = ReflectionUtils.GetAssembly(test.CodeElement);
            return assembly != null ? assembly.Path : null;
        }

        public ITest Test
        {
            get { return test; }
        }

        public override string GetTitle()
        {
            return test.Name;
        }

        public override string GetTypeClrName()
        {
            ITypeInfo type = ReflectionUtils.GetType(test.CodeElement);
            return type != null ? type.FullName : "";
        }

        public override UnitTestNamespace GetNamespace()
        {
            INamespaceInfo @namespace = ReflectionUtils.GetNamespace(test.CodeElement);
            return new UnitTestNamespace(@namespace != null ? @namespace.Name : "");
        }

        public override string GetKind()
        {
            return test.Metadata.GetValue(MetadataKeys.TestKind) ?? "Unknown";
        }

        public override IProject GetProject()
        {
            return ReSharperReflectionPolicy.GetProject(test.CodeElement);
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
                return UnitTestElementDisposition.ourInvalidDisposition;

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
            return ReSharperReflectionPolicy.GetDeclaredElement(test.CodeElement);
        }

#if RESHARPER_31
        public override bool Matches(string filter)
        {
            if (test.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0)
                return true;

            GallioTestElement parent = Parent as GallioTestElement;
            return parent != null && parent.Matches(filter);
        }
#endif

        public bool Equals(GallioTestElement other)
        {
            return other != null && test.Id == other.test.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GallioTestElement);
        }

        public override int GetHashCode()
        {
            return test.Id.GetHashCode();
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
            return test.Id.CompareTo(other.Test.Id);
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

        private void PopulateMetadata()
        {
            IList<string> categories = test.Metadata[MetadataKeys.CategoryName];
            if (categories.Count != 0)
                AssignCategories(categories);

            string reason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
            if (reason != null)
                SetExplicit(reason);
        }
    }
}