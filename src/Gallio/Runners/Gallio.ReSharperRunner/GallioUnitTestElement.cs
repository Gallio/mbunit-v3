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
using Gallio.Model;
using Gallio.Model.Reflection;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;

namespace Gallio.ReSharperRunner
{
    /// <summary>
    /// Represents a Gallio test.
    /// </summary>
    public class GallioUnitTestElement : UnitTestElement, IEquatable<GallioUnitTestElement>, IComparable<GallioUnitTestElement>
    {
        private readonly ITest test;

        public GallioUnitTestElement(ITest test, IUnitTestProvider provider, UnitTestElement parent)
            : base(provider, parent)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (provider == null)
                throw new ArgumentNullException("provider");

            this.test = test;

            PopulateMetadata();
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
            return test.Metadata.GetValue(MetadataKeys.ComponentKind) ?? "Unknown";
        }

        public override IProject GetProject()
        {
            IProjectAccessor accessor = test.CodeElement as IProjectAccessor;
            return accessor != null ? accessor.Project : null;
        }

        public override IList<IProjectItem> GetProjectItems()
        {
            IDeclaredElement declaredElement = GetDeclaredElement();

            if (declaredElement != null && declaredElement.IsValid())
                return declaredElement.GetProjectFiles();

            return EmptyArrays.ProjectItems;
        }

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
                        file.ProjectItem, declaration.GetNameRange(), declaration.GetDocumentRange().TextRange));
                }
            }

            return new UnitTestElementDisposition(locations, this);
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            IDeclaredElementAccessor accessor = test.CodeElement as IDeclaredElementAccessor;
            return accessor != null ? accessor.DeclaredElement : null;
        }

        public override bool Matches(string filter)
        {
            if (test.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0)
                return true;

            GallioUnitTestElement parent = Parent as GallioUnitTestElement;
            return parent != null && parent.Matches(filter);
        }

        public bool Equals(GallioUnitTestElement other)
        {
            return other != null && test.Id == other.test.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GallioUnitTestElement);
        }

        public override int GetHashCode()
        {
            return test.Id.GetHashCode();
        }

        public int CompareTo(GallioUnitTestElement other)
        {
            int discriminator = GetTitle().CompareTo(other.GetTitle());
            if (discriminator != 0)
                return discriminator;

            return test.Id.CompareTo(other.Test.Id);
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
