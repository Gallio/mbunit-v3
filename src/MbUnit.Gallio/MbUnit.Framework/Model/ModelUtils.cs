using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// Provides utility functions for manipulating the object model.
    /// </summary>
    public static class ModelUtils
    {
        /// <summary>
        /// Links a template into the list of children managed by a given parent template.
        /// </summary>
        /// <param name="parent">The parent template</param>
        /// <param name="childrenOfParent">The mutable list of children owned by the parent</param>
        /// <param name="child">The child template</param>
        /// <exception cref="InvalidOperationException">Thrown if the child already has a parent</exception>
        public static void LinkTemplate<T>(ITestTemplate parent, IList<T> childrenOfParent, T child)
            where T : ITestTemplate
        {
            if (child.Parent != null)
                throw new InvalidOperationException("The template to be added is already a child of another template.");

            child.Parent = parent;
            childrenOfParent.Add(child);
        }
    }
}
