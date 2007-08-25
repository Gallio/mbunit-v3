using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.DataBinding
{
    public class DataBindingSet
    {
        private readonly DataBinding[] bindings;

        public DataBindingSet(DataBinding[] bindings)
        {
            this.bindings = bindings;
        }

        /// <summary>
        /// Groups the specified bindings together so that they are generated
        /// from data providers together as a set.  Ungrouped bindings are
        /// generated combinatorially.
        /// </summary>
        /// <remarks>
        /// Grouping is transitive.  If binding A is grouped with binding B and
        /// binding B is grouped with binding C then A, B, and C are grouped
        /// together.
        /// </remarks>
        /// <param name="bindings">The bindings to group</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bindings"/>
        /// is null</exception>
        public void Group(IList<DataBinding> bindings)
        {
            throw new NotImplementedException();
        }
    }
}
