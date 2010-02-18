using System;
using Gallio.Icarus.Models;
using Gallio.Icarus.Specifications;

namespace Gallio.Icarus.Search
{
    public class TreeNodeSpecification : Specification<TestTreeNode>
    {
        private readonly string searchText;
        private readonly string metadataType;

        public TreeNodeSpecification(string searchText, string metadataType)
        {
            this.searchText = searchText;
            this.metadataType = metadataType;
        }

        public override bool Matches(TestTreeNode item)
        {
            return CaseInsensitiveContains(item.Text, searchText);
        }

        private static bool CaseInsensitiveContains(string toSearch, string toFind)
        {
            return toSearch.IndexOf(toFind, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}