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
using System.Reflection;
using Gallio;

namespace MbUnit.Framework.ContractVerifiers.Patterns.Comparison
{
    /// <summary>
    /// Builder for the test pattern <see cref="ComparisonPattern{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the results provided by
    /// the comparison method. Usually a Int32 or a Boolean.</typeparam>
    internal class ComparisonPatternBuilder<T> : ContractVerifierPatternBuilder 
        where T : struct
    {
        private Type targetType;
        private MethodInfo comparisonMethodInfo;
        private string signatureDescription;
        private Func<int, int, T> refers;
        private Func<T, string> formats = x => x.ToString();
        private Func<T, T> postProcesses = x => x;
        private string name;

        /// <summary>
        /// Sets the target evaluated type.
        /// </summary>
        /// <param name="targetType">The target evaluated type.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ComparisonPatternBuilder<T> SetTargetType(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            this.targetType = targetType;
            return this;
        }

        /// <summary>
        /// Sets the reflection descriptor for the comparison method.
        /// </summary>
        /// <param name="comparisonMethodInfo">The reflection descriptor
        /// for the comparison method.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ComparisonPatternBuilder<T> SetComparisonMethodInfo(MethodInfo comparisonMethodInfo)
        {
            this.comparisonMethodInfo = comparisonMethodInfo;
            return this;
        }

        /// <summary>
        /// Sets a friendly displayable text explaining the signature of 
        /// the comparison method.
        /// </summary>
        /// <param name="signatureDescription">A friendly signature text.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ComparisonPatternBuilder<T> SetSignatureDescription(string signatureDescription)
        {
            if (signatureDescription == null)
            {
                throw new ArgumentNullException("signatureDescription");
            }

            this.signatureDescription = signatureDescription;
            return this;
        }

        /// <summary>
        /// Sets an equivalent comparison method which gives the same 
        /// results with Int32 parameters.
        /// </summary>
        /// <param name="refers">An equivalent comparison function.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ComparisonPatternBuilder<T> SetFunctionRefers(Func<int, int, T> refers)
        {
            if (refers == null)
            {
                throw new ArgumentNullException("refers");
            }

            this.refers = refers;
            return this;
        }

        /// <summary>
        /// Sets a function which formats the result into a friendly text.
        /// </summary>
        /// <param name="formats">An formatting function.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ComparisonPatternBuilder<T> SetFunctionFormats(Func<T, string> formats)
        {
            if (formats == null)
            {
                throw new ArgumentNullException("formats");
            }

            this.formats = formats;
            return this;
        }

        /// <summary>
        /// Sets a post-processor function for the result of the comparison method, 
        /// in order to make it comparable with the result of the reference.
        /// </summary>
        /// <param name="postProcesses">An post-processing function.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ComparisonPatternBuilder<T> SetFunctionPostProcesses(Func<T, T> postProcesses)
        {
            if (postProcesses == null)
            {
                throw new ArgumentNullException("postProcesses");
            }

            this.postProcesses = postProcesses;
            return this;
        }

        /// <summary>
        /// Sets a friendly name for the test pattern
        /// </summary>
        /// <param name="name">A friendly signature text.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ComparisonPatternBuilder<T> SetName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("friendlyName");
            }

            this.name = name;
            return this;
        }

        /// <inheritdoc />
        public override ContractVerifierPattern ToPattern()
        {
            if (targetType == null)
            {
                throw new InvalidOperationException("A target type must be specified.");
            }

            if (refers == null)
            {
                throw new InvalidOperationException("A reference function must be specified.");
            }

            if (name == null)
            {
                throw new InvalidOperationException("A friendly name must be specified.");
            }

            if (signatureDescription == null)
            {
                throw new InvalidOperationException("A signature description must be specified.");
            }

            return new ComparisonPattern<T>(new ComparisonPatternSettings<T>(
                targetType, comparisonMethodInfo, signatureDescription, refers, 
                formats, postProcesses, name));
        }
    }
}