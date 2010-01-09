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
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using System.Collections.Generic;

namespace MbUnit.Framework
{
    /// <summary>
    /// Base class for all the patterns defining an extension point of the framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Extension point attributes act through a static method.
    /// </para>
    /// </remarks>
    [SystemInternal]
    public abstract class ExtensionPointPatternAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override bool IsPrimary
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Protected constructor.
        /// </summary>
        protected ExtensionPointPatternAttribute()
        {
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            var methodInfo = (IMethodInfo)codeElement;

            if (!methodInfo.IsStatic)
                ThrowUsageErrorException(String.Format("Expected the custom extensibility method '{0}' to be static.", methodInfo.Name));

            Verify(methodInfo);
            Extend(containingScope, methodInfo);
        }

        /// <summary>
        /// Verifies that the method has a compatible signature.
        /// </summary>
        /// <param name="methodInfo">The method to verify</param>
        protected abstract void Verify(IMethodInfo methodInfo);

        /// <summary>
        /// Extends the framework.
        /// </summary>
        /// <param name="containingScope">The containing scope.</param>
        /// <param name="methodInfo">The method to verify</param>
        protected abstract void Extend(IPatternScope containingScope, IMethodInfo methodInfo);
    }
}
