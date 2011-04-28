// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.ComponentModel;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// The <see cref="CompositePatternAttribute" /> class is a base class that can be used to define "composite" patterns. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// A composite pattern is a single pattern that is composed of (and equivelent to) a defined set of "component" patterns.
    /// </para>
    /// <para>
    /// Composite patterns allow arbitrary sets of individual <see cref="IPattern"/>s to be grouped together 
    /// and encapsulated so that the entire group can be treated as a single pattern.
    /// </para>
    /// <para>
    /// Grouping patterns that are commonly used together into a composite pattern offers multiple benefits 
    /// vs. using the component patterns separately.
    /// Benefits of composite patterns include:
    /// <list type="bullet">
    /// <item>Composite patterns are easier to apply than the equivelent set of component patterns.</item>
    /// <item>Composite patterns ensure that the entire set of component patterns is applied consistently.</item>
    /// <item>Composite patterns can reduce code duplication (e.g. for declaratively applied <see cref="PatternAttribute"/>s).</item>
    /// <item>
    /// Composite patterns encapsulate and centralize the definition of the pattern group.
    /// (This makes it easier and safer to maintain test code, 
    /// such as when adding, removing, or modifying the component patterns of the composite pattern.)
    /// </item>
    /// <item>
    /// Composite patterns allow meaningful names to be assigned to pattern groups. 
    /// (This often improves test code readability, understandability, and maintainability.)
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// This C# example shows a derived class that encapsulates 3 separate <see cref="PatternAttribute"/>s 
    /// as a named composite attribute.
    /// <code><![CDATA[
    /// /// <summary>
    /// /// This attribute composites 3 separate <see cref="MetadataPatternAttribute"/>s into a single attribute.
    /// /// </summary>
    /// public class SampleCompositePatternAttribute: CompositePatternAttribute
    /// {
    ///     private readonly PatternAttribute[] _componentPatterns = new[]
    ///     {
    ///         new CategoryAttribute("Samples"),
    ///         new AuthorAttribute("John"),
    ///         new ImportanceAttribute(Importance.Critical)
    ///     };
    /// 
    ///     protected override IEnumerable<IPattern> GetPatterns()
    ///     {
    ///         return _componentPatterns;
    ///     }
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// This C# example shows 2 derived classes that each encapsulates multiple separate MbUnit.Framework.CatchExceptionAttributes 
    /// as a named composite attribute.
    /// <code><![CDATA[
    /// /// <summary>
    /// /// This attribute composites 4 separate <see cref="MbUnit.Framework.CatchExceptionAttributes"/>s into a single attribute.
    /// /// </summary>
    /// public class CompositeCatchArgumentExceptionsAttribute : CompositePatternAttribute
    /// {
    ///     //NOTE: The Order properties of the attributes are intentionally different than the array sequence.
    ///     private readonly PatternAttribute[] _componentPatterns = new PatternAttribute[]
    ///     {
    ///         new CatchExceptionAttribute(typeof(ArgumentNullException)) { StandardOutcome = "TestOutcome.Skipped", Order = 1},
    ///         new CatchExceptionAttribute(typeof(ArgumentOutOfRangeException)) { StandardOutcome = "TestOutcome.Passed", Order = 4},
    ///         new CatchExceptionAttribute(typeof(ArgumentOutOfRangeException)) { StandardOutcome = "TestOutcome.Ignored", ExceptionMessage = "expectedmessage", Order = 2},
    ///         new CatchExceptionAttribute(typeof(ArgumentException)) { StandardOutcome = "TestOutcome.Inconclusive", Order = 3}
    ///     };
    /// 
    ///     protected override IEnumerable<IPattern> GetPatterns()
    ///     {
    ///         return _componentPatterns;
    ///     }
    /// }
    /// 
    /// /// <summary>
    /// /// This attribute composites 2 separate <see cref="MbUnit.Framework.CatchExceptionAttributes"/>s into a single attribute.
    /// /// </summary>
    /// public class CompositeCatchMiscExceptionsAttribute : CompositePatternAttribute
    /// {
    ///     private readonly PatternAttribute[] _componentPatterns = new PatternAttribute[]
    ///     {
    ///         new CatchExceptionAttribute(typeof(InvalidCastException)) { StandardOutcome = "TestOutcome.Ignored", Order = 1},
    ///         new CatchExceptionAttribute(typeof(Exception)) { StandardOutcome = "TestOutcome.Pending", ExceptionMessage = "expectedmessage", Order = 2}
    ///     };
    /// 
    ///     protected override IEnumerable<IPattern> GetPatterns()
    ///     {
    ///         return _componentPatterns;
    ///     }
    /// }
    /// 
    /// /// <summary>
    /// /// This test illustrates a potential usage scenario for the 2 composite attributes above.
    /// /// </summary>
    /// [Test]
    /// [CompositeCatchArgumentExceptions]
    /// [CompositeCatchMiscExceptions]
    /// public void ExpectInconclusiveFromFirstOfMultipleCompositeCatchAttributesWhenThrowArgEx()
    /// {
    ///     throw new ArgumentException();
    /// }
    /// ]]></code>
    /// </para>
    /// </example>
    /// <seealso cref="IPattern"/>
    /// <seealso cref="PatternAttribute"/>
    [SystemInternal]
    public abstract class CompositePatternAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            base.Process(scope, codeElement);

            foreach (var pattern in this.GetPatterns())
            {
                pattern.Process(scope, codeElement);
            }
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            base.Consume(containingScope, codeElement, skipChildren);

            foreach (var pattern in this.GetPatterns())
            {
                pattern.Consume(containingScope, codeElement, skipChildren);
            }
        }

        /// <summary>
        /// Gets a set of individual patterns that are collectively equivelent to the composite pattern represented by the instance.
        /// </summary>
        /// <remarks>
        /// Note to inheritors: A common implementation of this method is to simply return an array containing 
        /// programmatically instantiated instances of various Attributes that implement <see cref="IPattern"/>.
        /// See the class documentation for examples of this type of implementation.
        /// </remarks>
        /// <returns>The set of patterns that collectively define the composite pattern.</returns>
        protected abstract IEnumerable<IPattern> GetPatterns();
    }

}