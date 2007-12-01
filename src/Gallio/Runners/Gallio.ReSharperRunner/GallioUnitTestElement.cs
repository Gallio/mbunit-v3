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
            PopulateChildren();
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
            IDeclaredElement declaredElement = GetDeclaredElement();

            if (declaredElement != null && declaredElement.IsValid())
                return declaredElement.Module as IProject;

            return null;
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

        private void PopulateChildren()
        {
            // Create the children (has the side-effect of enlisting them in the Children list).
            foreach (ITest child in test.Children)
                new GallioUnitTestElement(child, Provider, this);
        }
    }
}
