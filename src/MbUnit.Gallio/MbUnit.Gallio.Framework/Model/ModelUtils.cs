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
