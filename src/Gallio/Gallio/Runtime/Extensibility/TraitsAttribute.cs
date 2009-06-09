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

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Specifies the traits class associated with a service contract.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute enables the runtime to bind a service contract to its traits.
    /// If no attribute is specified then it is presumed that the service has an empty
    /// traits class (represented by <see cref="Traits" />).
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class TraitsAttribute : Attribute
    {
        private readonly Type traitsType;

        /// <summary>
        /// Binds a traits interface to a service contract.
        /// </summary>
        /// <param name="traitsType">The traits type, a subclass of <see cref="Traits" /></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="traitsType"/>
        /// is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="traitsType"/>
        /// is not a class or if it is not derived from <see cref="Traits" /></exception>
        public TraitsAttribute(Type traitsType)
        {
            if (traitsType == null)
                throw new ArgumentNullException("traitsType");
            if (!typeof(Traits).IsAssignableFrom(traitsType))
                throw new ArgumentException("The associated traits type must be derived from the Traits class.", "traitsType");

            this.traitsType = traitsType;
        }

        /// <summary>
        /// Gets the traits type, a subclass of <see cref="Traits"/>.
        /// </summary>
        public Type TraitsType
        {
            get { return traitsType; }
        }
    }
}
