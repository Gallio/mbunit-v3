// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
    /// Extension point attributes act through a container class that 
    /// must have a default parameterless constructor.
    /// </para>
    /// </remarks>
    [SystemInternal]
    public abstract class ExtensionPointPatternAttribute : PatternAttribute
    {
        private object containerInstance;
 
        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }
       
        /// <summary>
        /// Gets an instance of the container type.
        /// </summary>
        protected object ContainerInstance
        {
            get
            {
                if (Object.ReferenceEquals(null, containerInstance))
                {
                    try
                    {
                        containerInstance = Activator.CreateInstance(ContainerType);
                    }
                    catch (MissingMethodException exception)
                    {
                        throw new PatternUsageErrorException(String.Format(
                            "[{0}] - The extension container class '{1}' should have a default parameterless constructor.",
                            GetType().Name, ContainerType), exception);
                    }
                }

                return containerInstance;
            }
        }

        /// <summary>
        /// Gets the type of the container class.
        /// </summary>
        protected Type ContainerType
        {
            get; 
            private set;
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
            ContainerType = ((ITypeInfo)codeElement).Resolve(true);
            Extend(containingScope);
        }

        /// <summary>
        /// Extends the framework.
        /// </summary>
        /// <param name="containingScope">The containing scope.</param>
        protected abstract void Extend(IPatternScope containingScope);
    }
}
