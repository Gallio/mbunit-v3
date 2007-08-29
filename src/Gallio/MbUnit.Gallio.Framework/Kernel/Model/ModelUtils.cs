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
using System.Globalization;
using System.Reflection;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Provides utility functions for manipulating the object model.
    /// </summary>
    public static class ModelUtils
    {
        /// <summary>
        /// Links a node into the list of children managed by a given parent.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="child">The child to add</param>
        /// <exception cref="InvalidOperationException">Thrown if the child already has a parent</exception>
        public static void Link<T>(T parent, T child)
            where T : class, IModelTreeNode<T>
        {
            if (child.Parent != null)
                throw new InvalidOperationException(Resources.ModelUtils_NodeAlreadyHasAParent);

            child.Parent = parent;
            parent.Children.Add(child);
        }

        /// <summary>
        /// Gets all children of the node that have the specified type.
        /// </summary>
        /// <typeparam name="S">The node type</typeparam>
        /// <typeparam name="T">The type to filter by</typeparam>
        /// <param name="node">The node whose children are to be scanned</param>
        /// <returns>The filtered list of children</returns>
        public static IList<T> FilterChildrenByType<S, T>(IModelTreeNode<S> node)
            where S : class, IModelTreeNode<S> where T : class, S
        {
            List<T> filteredChildren = new List<T>();
            foreach (S child in node.Children)
            {
                T filteredChild = child as T;
                if (filteredChild != null)
                    filteredChildren.Add(filteredChild);
            }

            return filteredChildren;
        }

        /// <summary>
        /// Checks that the method has the specified signature otherwise throws a <see cref="ModelException" />.
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="signature">The list of parameter types (all input parameters)</param>
        /// <exception cref="ModelException">Thrown if the method has a different signature</exception>
        public static void CheckMethodSignature(MethodInfo method, params Type[] signature)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == signature.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterInfo parameter = parameters[i];
                    if (parameter.ParameterType != signature[i] || !parameter.IsIn || parameter.IsOut)
                        goto Fail;
                }

                return;
            }

        Fail:
            string[] expectedTypeNames = Array.ConvertAll<Type, string>(signature, delegate(Type parameterType)
            {
                return parameterType.FullName;
            });
            string[] actualTypeNames = Array.ConvertAll<ParameterInfo, string>(parameters, delegate(ParameterInfo parameter)
            {
                if (parameter.IsOut)
                {
                    string prefix = parameter.IsIn ? @"ref " : @"out ";
                    return prefix + parameter.ParameterType.FullName;
                }

                return parameter.ParameterType.FullName;
            });

            throw new ModelException(String.Format(CultureInfo.CurrentCulture,
                Resources.ModelUtils_InvalidSignature,
                string.Join(@", ", expectedTypeNames),
                string.Join(@", ", actualTypeNames)));
        }
    }
}
