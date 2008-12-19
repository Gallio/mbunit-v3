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
    /// Builder for the test pattern <see cref="ComparisonPattern{TTarget, TResult}"/>
    /// </summary>
    /// <typeparam name="TTarget">The target comparable type.</typeparam>
    /// <typeparam name="TResult">The type of the results provided by the comparison method. 
    /// Usually a Int32 or a Boolean.</typeparam>
    internal class ComparisonPatternBuilder<TTarget, TResult> : ContractVerifierPatternBuilder 
        where TTarget : IComparable<TTarget>
        where TResult : struct
    {
        private MethodInfo comparisonMethodInfo;
        private string signatureDescription;
        private Func<int, int, TResult> refers;
        private Func<TResult, string> formats = x => x.ToString();
        private Func<TResult, TResult> postProcesses = x => x;
        private string name;
        private PropertyInfo equivalenceClassSource;

        /// <summary>
        /// Sets the reflection descriptor for the comparison method.
        /// </summary>
        /// <param name="comparisonMethodInfo">The reflection descriptor
        /// for the comparison method.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ComparisonPatternBuilder<TTarget, TResult> SetComparisonMethodInfo(MethodInfo comparisonMethodInfo)
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
        internal ComparisonPatternBuilder<TTarget, TResult> SetSignatureDescription(string signatureDescription)
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
        internal ComparisonPatternBuilder<TTarget, TResult> SetFunctionRefers(Func<int, int, TResult> refers)
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
        internal ComparisonPatternBuilder<TTarget, TResult> SetFunctionFormats(Func<TResult, string> formats)
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
        internal ComparisonPatternBuilder<TTarget, TResult> SetFunctionPostProcesses(Func<TResult, TResult> postProcesses)
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
        internal ComparisonPatternBuilder<TTarget, TResult> SetName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("friendlyName");
            }

            this.name = name;
            return this;
        }

        /// <summary>
        /// Sets the source of equivalence classes.
        /// </summary>
        /// <param name="equivalenceClassSource">Information about the contract verifier
        /// property providing a collection of equivalence classes.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal ComparisonPatternBuilder<TTarget, TResult> SetEquivalenceClassSource(PropertyInfo equivalenceClassSource)
        {
            if (equivalenceClassSource == null)
            {
                throw new ArgumentNullException("source");
            }

            this.equivalenceClassSource = equivalenceClassSource;
            return this;
        }

        /// <inheritdoc />
        public override ContractVerifierPattern ToPattern()
        {
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

            if (equivalenceClassSource == null)
            {
                throw new InvalidOperationException("The source of equivalence classes must be specified.");
            }

            return new ComparisonPattern<TTarget, TResult>(new ComparisonPatternSettings<TResult>(
                comparisonMethodInfo, signatureDescription, refers, 
                formats, postProcesses, name, equivalenceClassSource));
        }
    }
}