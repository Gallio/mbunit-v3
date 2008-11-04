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
using System.Text;
using System.Reflection;
using Gallio;

namespace MbUnit.Framework.ContractVerifiers.Patterns.Comparison
{
    /// <summary>
    /// Data container which exposes necessary data required to
    /// run the test pattern <see cref="ComparisonPattern{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the results provided by
    /// the comparison method. Usually a Int32 or a Boolean.</typeparam>
    internal class ComparisonPatternSettings<T> where T : struct
    {
        /// <summary>
        /// Gets the target evaluated type.
        /// </summary>
        public Type TargetType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets information about the comparison method.
        /// </summary>
        public MethodInfo ComparisonMethodInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a friendly description of the equality method signature.
        /// </summary>
        public string SignatureDescription
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets an equivalent comparison method which gives the same 
        /// results with Int32 parameters.
        /// </summary>
        public Func<int, int, T> Refers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a function which formats the result into a friendly text.
        /// </summary>
        public Func<T, string> Formats
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a post-processor function for the result of the comparison method, 
        /// in order to make it comparable with the result of the reference.
        /// </summary>
        public Func<T, T> PostProcesses
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a friendly name for the test pattern.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs the data container which exposes necessary data required to
        /// run the test pattern <see cref="ComparisonPattern{T}"/>.
        /// </summary>
        /// <param name="targetType">The target evaluated type.</param>
        /// <param name="comparisonMethodInfo">Information about the comparison method.</param>
        /// <param name="signatureDescription">A friendly description of the equality method signature.</param>
        /// <param name="refers"></param>
        /// <param name="formats"></param>
        /// <param name="postProcesses"></param>
        /// <param name="name">A friendly name for the test pattern.</param>
        public ComparisonPatternSettings(Type targetType, MethodInfo comparisonMethodInfo,
            string signatureDescription, Func<int, int, T> refers,
            Func<T, string> formats, Func<T, T> postProcesses, string name)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            
            if (refers == null)
            {
                throw new ArgumentNullException("refers");
            }

            if (formats == null)
            {
                throw new ArgumentNullException("formats");
            }

            if (postProcesses == null)
            {
                throw new ArgumentNullException("postProcesses");
            }

            if (signatureDescription == null)
            {
                throw new ArgumentNullException("signatureDescription");
            }

            if (name == null)
            {
                throw new ArgumentNullException("friendlyName");
            }

            this.TargetType = targetType;
            this.SignatureDescription = signatureDescription;
            this.Refers = refers;
            this.Formats = formats;
            this.PostProcesses = postProcesses;
            this.Name = name;
            this.ComparisonMethodInfo = comparisonMethodInfo;
        }
    }
}
